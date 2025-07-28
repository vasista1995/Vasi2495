using System;
using System.Collections.Generic;
using System.Linq;
using PDS.Queue.Api.Message;
using PDS.Space.Common.Data.PADSModel;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.Common.Data.Config;
using PDS.SpaceNew.PADS.Module.Data.PADSModel;
using BaseDataFlatMetaDataPads = PDS.SpaceNew.PADS.Module.Data.PADSModel.BaseDataFlatMetaDataPads;
using BaseStrangeDataFlatMetaDataPads = PDS.SpaceNew.PADS.Module.Data.PADSModel.BaseStrangeDataFlatMetaDataPads;

namespace PDS.SpaceNew.PADS.Module.Aggregations
{
    internal class SpaceWaferAggregation : SpaceBaseAggregation
    {
        private readonly SpaceAttributeMapper _attributeMapper = new SpaceAttributeMapper();
        private readonly IdCreationConfig _spaceIdCreationConfig;
        private readonly PropertyPlacementConfig _attributePADSStructureMappingConfig;

        public SpaceWaferAggregation(IdCreationConfig spaceIdCreationConfig, PropertyPlacementConfig attributePADSStructureMappingConfig, string siteKey) : base(siteKey)
        {
            _spaceIdCreationConfig = spaceIdCreationConfig;
            _attributePADSStructureMappingConfig = attributePADSStructureMappingConfig;
        }

        internal SpacePads CreateWaferSpacePADS(SpaceE4A renamedAttributesE4A, string waferAggregationId, string waferIdString, SpacePads lotSpacePADS,
            List<Data1ListRawValuesPads4Wafer> waferValueList, IDictionary<string, string> flexibleAttributeMappingConfig,
            IDictionary<string, string> customerFieldAttributesMappingConfig, IQueueMessage message)
        {
            string siteKey = (string) renamedAttributesE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SiteKey);
            var waferSpacePads = new SpacePads
            {
                Document = CreatePadsDocument(lotSpacePADS),
                ProductionAction = CreateProductionActionPads(lotSpacePADS),
                Item = CreateItemPads(lotSpacePADS, waferIdString),
                SystemLog = CreateSystemLog(renamedAttributesE4A, message),
                SearchPatterns = CreateSearchPatterns(waferAggregationId, siteKey)
            };

            // Create FlatMetaData & StrangeMetaData sections and add assign further originally base properties dynamically
            waferSpacePads = _attributeMapper.MapCertainAttributesToPADSStructure(waferSpacePads,
                _attributePADSStructureMappingConfig, renamedAttributesE4A, flexibleAttributeMappingConfig, customerFieldAttributesMappingConfig);
            EnhanceFlatMetaData(lotSpacePADS, waferSpacePads, renamedAttributesE4A, waferIdString, waferValueList);
            AddBaseProperties<BaseDataFlatMetaDataPads>(waferSpacePads.DataFlatMetaData, renamedAttributesE4A);
            AddBaseProperties<BaseStrangeDataFlatMetaDataPads>(waferSpacePads.StrangeDataFlatMetaData, renamedAttributesE4A);
            waferSpacePads.DataFlatMetaData.Add("SubOperation", waferSpacePads.DataFlatMetaData["SPSNumber"]);

            waferSpacePads.Data1ListParameters = CreateData1List(lotSpacePADS, waferIdString, waferValueList);

            return waferSpacePads;       
        }

        private DocumentPads CreatePadsDocument(SpacePads lotSpacePADS)
        {
            var repetition = new RepetitionPads()
            {
                Id = "0",
                IdBaseValue = (DateTime) lotSpacePADS.DataFlatMetaData[RequiredConvertedAttributes.EndTimestampUtc]
            };

            var document = new DocumentPads()
            {
                Type = "ProcessControl",
                Version = "1.0",
                Repetition = repetition
            };

            return document;
        }

        private ProductionActionPads CreateProductionActionPads(SpacePads lotSpacePADS)
        {
            var productionAction = new ProductionActionPads()
            {
                Id = lotSpacePADS.ProductionAction.Id,
                Type = "SpaceAggregation"
            };

            return productionAction;
        }

        private ItemPads CreateItemPads(SpacePads lotSpacePADS, string waferIdString)
        {
            var item = new ItemPads()
            {
                Id = "MotherlotWafer:" + lotSpacePADS.DataFlatMetaData.GetValueOrThrow(RequiredConvertedAttributes.MeasLot).ToString().Substring(0, 8) + ":" + waferIdString,
                IdSystemName = "MotherlotWafer",
                Type = "Wafer"
            };

            return item;
        }

        private SystemLogPads CreateSystemLog(SpaceE4A renamedAttributesE4A, IQueueMessage message)
        {
            return new SystemLogPads(renamedAttributesE4A.SystemLog, message)
            {
                DocCreatedTimestampUtc = DateTime.UtcNow,
            };
        }

        public SearchPatternsPads CreateSearchPatterns(string waferAggregationId, string siteKey)
        {
            return new SearchPatternsPads()
            {
                SpaceKey = waferAggregationId,
                TimeGroup = "0",
                SiteKey = siteKey
            };
        }

        public string GetOperWaferAggregationId(SpaceE4A e4aEntry, string motherLotWafer)
        {
            var parameterValueDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "WaferLotId", motherLotWafer }
            };

            string waferAggregationId = GetId("GetOperWaferAggregationId", _spaceIdCreationConfig, parameterValueDictionary, e4aEntry.SpaceAttributes);
            return waferAggregationId;
        }

        private void EnhanceFlatMetaData(SpacePads lotSpacePADS, SpacePads waferSpacePads,
            SpaceE4A renamedAttributesE4A, string waferIdString, List<Data1ListRawValuesPads4Wafer> waferValueList)
        {
            var rawvaluesList = new List<Data1ListRawValuesPads>();
            //foreach (var datalist in lotSpacePADS.Data1ListParameters)
            //{
            //    var waferValueObjects = waferValueList.Where(w => string.Equals(w.ParameterName, datalist.ParameterName, StringComparison.InvariantCultureIgnoreCase));
            //    if (waferValueObjects.Any())
            //        rawvaluesList = datalist.Data1ListRawValues; // TODO: Clarify value is overwritten without using it, random factor (96% possible bug)
            //}

            // Would translate to:
            rawvaluesList = lotSpacePADS.Data1ListParameters
                    .Where(d => waferValueList.Any(w => string.Equals(w.ParameterName, d.ParameterName, StringComparison.InvariantCultureIgnoreCase)))
                    .Select(d => d.Data1ListRawValues)
                    .LastOrDefault();

            var listSampleTimestamps = new List<DateTime>();
            var listSampleTimestampsUtc = new List<DateTime>();
            var rawValueObjects = rawvaluesList.Where(r => string.Equals(r.MotherlotWafer.Split(':').Last(), waferIdString, StringComparison.OrdinalIgnoreCase));
            foreach (var rawValueObject in rawValueObjects)
            {
                listSampleTimestamps.Add(rawValueObject.SampleTimestamp);
                listSampleTimestampsUtc.Add(rawValueObject.SampleTimestampUtc);
            }

            var dataFlatMetaData = waferSpacePads.DataFlatMetaData;
            dataFlatMetaData.Add(WaferAttributes.Motherlot, renamedAttributesE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.MeasLot).ToString().Substring(0, 8));
            dataFlatMetaData.Add(WaferAttributes.Wafer, waferIdString);
            dataFlatMetaData.Add(WaferAttributes.BeginTimestampUtc, listSampleTimestampsUtc.Any() ? listSampleTimestampsUtc.Min() : DateTime.MinValue);
            dataFlatMetaData.Add(WaferAttributes.BeginTimestamp, listSampleTimestamps.Any() ? listSampleTimestamps.Min() : DateTime.MinValue);
            dataFlatMetaData.Add(WaferAttributes.EndTimestampUtc, listSampleTimestampsUtc.Any() ? listSampleTimestampsUtc.Max() : DateTime.MinValue);
            dataFlatMetaData.Add(WaferAttributes.EndTimestamp, listSampleTimestamps.Any() ? listSampleTimestamps.Max() : DateTime.MinValue);
        }

        private List<Data1ListPads> CreateData1List(SpacePads lotSpacePADS, string waferIdString, List<Data1ListRawValuesPads4Wafer> waferValueList)
        {
            var data1ListWaferObjects = new List<Data1ListPads>();
            var parameterNames = waferValueList.Where(w => string.Equals(w.MotherlotWafer.Split(":").Last(), waferIdString, StringComparison.OrdinalIgnoreCase))
                                               .Select(w => w.ParameterName).ToList();

            var data1ListLotObjects = lotSpacePADS.Data1ListParameters.Where(p => parameterNames.Contains(p.ParameterName, StringComparer.InvariantCultureIgnoreCase));
            foreach (var data1ListLotObject in data1ListLotObjects)
            {
                var data1ListWaferObject = new Data1ListPads();

                // Transfer all properties from data1ListObject to data1List
                var data1ListObjectDictionary = data1ListLotObject.ToDictionary<object>();
                SetPropertiesFromDictionary(data1ListWaferObject, data1ListObjectDictionary);

                data1ListWaferObject.DataFlatLimits = CreateDataFlatLimits(lotSpacePADS, waferValueList);
                data1ListWaferObject.MeasurementAggregates = GetMeasurementAggregates(data1ListLotObject, waferIdString, waferValueList);
                data1ListWaferObject.Data1ListRawValues = null;

                data1ListWaferObjects.Add(data1ListWaferObject);
            }

            return data1ListWaferObjects;
        }

        private DataFlatLimitsPads CreateDataFlatLimits(SpacePads lotSpacePADS, List<Data1ListRawValuesPads4Wafer> waferValueList)
        {
            var lastDataListLotObject = lotSpacePADS.Data1ListParameters.LastOrDefault(
                 d => waferValueList.Select(w => w.ParameterName).Contains(d.ParameterName, StringComparer.InvariantCultureIgnoreCase
            ));

            return new DataFlatLimitsPads()
            {
                CKCId = lastDataListLotObject.DataFlatLimits.CKCId,
                MeasurementSpecLimits = lastDataListLotObject.DataFlatLimits.MeasurementSpecLimits,
                ControlLimits = lastDataListLotObject.DataFlatLimits.ControlLimits
            };
        }

        public MeasurementAggregatesPads GetMeasurementAggregates(Data1ListPads data1ListLotObject, string waferIdString, List<Data1ListRawValuesPads4Wafer> waferValueList)
        {
            if (string.Equals(data1ListLotObject.RvStoreFlag, "Y", StringComparison.InvariantCultureIgnoreCase))
            {
                var rawValues = waferValueList.Where(it => string.Equals(it.ParameterName, data1ListLotObject.ParameterName, StringComparison.OrdinalIgnoreCase)).ToList();
                var measurementRawValues = rawValues.Where(it => it.IsFlagged == "N" && it.MotherlotWafer.Split(':')[1].ToString() == waferIdString).ToList();
                var measurementAggregates = CreateMeasurementAggregates(rawValues.Count, measurementRawValues, measurementRawValues);
                AddGofAggregates(measurementAggregates, measurementRawValues);
                return measurementAggregates;
            }
            else if (string.Equals(data1ListLotObject.RvStoreFlag, "N", StringComparison.InvariantCultureIgnoreCase))
            {
                return LoadMeasurementAggregates(data1ListLotObject.ParameterName, data1ListLotObject.Data1ListRawValues, waferValueList);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Load the space aggregates when there are no raw values available.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="rawValues"></param>
        /// <param name="waferValueList"></param>
        internal static MeasurementAggregatesPads LoadMeasurementAggregates(string parameterName, List<Data1ListRawValuesPads> rawValues, List<Data1ListRawValuesPads4Wafer> waferValueList)
        {
            int baseCount = 0;
            int flaggedCount = 0;
            var weightedmeanvals = new List<double>();
            var minvals = new List<double>();
            var maxvals = new List<double>();
            var samples = new List<long>();
            foreach (var spcaggs in rawValues)
            {
                var waferValueObjects = waferValueList.Where(v => spcaggs.MotherlotWafer == v.MotherlotWafer && parameterName == v.ParameterName);
                foreach (var waferValueObject in waferValueObjects)
                {
                    // TODO: waferValueObject is not used, very likely a bug
                    if (spcaggs.IsFlagged == "N")
                    {
                        baseCount += spcaggs.SampleSize;
                        weightedmeanvals.Add(spcaggs.SampleMean * spcaggs.SampleSize);
                        minvals.Add(spcaggs.SampleMin);
                        maxvals.Add(spcaggs.SampleMax);
                        samples.Add(spcaggs.SampleId);
                    }
                    else
                    {
                        flaggedCount += spcaggs.SampleSize;
                    }
                }
            }

            return new MeasurementAggregatesPads()
            {
                BaseCount = baseCount,
                FlaggedCount = flaggedCount,
                ExecCount = baseCount + flaggedCount,
                Mean = weightedmeanvals.Count > 0 ? weightedmeanvals.Sum() / baseCount : 0.0,
                Min = minvals.Count > 0 ? minvals.Min() : 0.0,
                Max = maxvals.Count > 0 ? maxvals.Max() : 0.0,
                Samples = string.Join(", ", samples.Distinct())
            };
        }
    }
}
