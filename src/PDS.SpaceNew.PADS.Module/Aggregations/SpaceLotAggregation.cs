using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using PDS.Queue.Api.Message;
using PDS.Space.Common.Data.PADSModel;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.Common.Data.Config;
using PDS.SpaceNew.PADS.Module.Data.PADSModel;
using BaseDataFlatMetaDataPads = PDS.SpaceNew.PADS.Module.Data.PADSModel.BaseDataFlatMetaDataPads;
using BaseStrangeDataFlatMetaDataPads = PDS.SpaceNew.PADS.Module.Data.PADSModel.BaseStrangeDataFlatMetaDataPads;

namespace PDS.SpaceNew.PADS.Module.Aggregations
{
    internal class SpaceLotAggregation : SpaceBaseAggregation
    {
        private readonly SpaceAttributeMapper _attributeMapper = new SpaceAttributeMapper();
        private readonly IdCreationConfig _spaceIdCreationConfig;
        private readonly PropertyPlacementConfig _attributePADSStructureMappingConfig;

        public SpaceLotAggregation(IdCreationConfig spaceIdCreationConfig, PropertyPlacementConfig attributePADSStructureMappingConfig, string siteKey) : base(siteKey)
        {
            _spaceIdCreationConfig = spaceIdCreationConfig;
            _attributePADSStructureMappingConfig = attributePADSStructureMappingConfig;
        }

        /// <summary>
        /// create new pads document from the e4a doucment
        /// </summary>
        /// <returns name="SpacePads">spacePADS is the pads document that contains all the lot aggregations and is returned to SpaceQueueReader.cs to be inserted into MongoDB</returns>
        public SpacePads CreatePADSDocument(SpaceE4A renamedAttributesE4A, IDictionary<string, string> flexibleAttributeMappingConfig,
            IDictionary<string, string> customerFieldAttributesMappingConfig, IQueueMessage queueMessage)
        {
            var spacePads = new SpacePads
            {
                Document = CreatePadsDocument(renamedAttributesE4A),
                ProductionAction = CreateProductionAction(renamedAttributesE4A),
                Item = CreateItem(renamedAttributesE4A),
                SystemLog = CreateSystemLog(renamedAttributesE4A, queueMessage)
            };

            string lotAggregationId = GetLotAggregationId(renamedAttributesE4A, spacePads.Item);
            spacePads.SearchPatterns = CreateSearchPatterns(renamedAttributesE4A, lotAggregationId);

            // Create FlatMetaData & StrangeMetaData sections and add assign further originally base properties dynamically
            var newSpacePADS = _attributeMapper.MapCertainAttributesToPADSStructure(spacePads, _attributePADSStructureMappingConfig,
                renamedAttributesE4A, flexibleAttributeMappingConfig, customerFieldAttributesMappingConfig);
            AddBaseProperties<BaseDataFlatMetaDataPads>(newSpacePADS.DataFlatMetaData, renamedAttributesE4A);
            if (newSpacePADS.DataFlatMetaData.ContainsKey("SPSNumber"))
            {
                newSpacePADS.DataFlatMetaData.Add("SubOperation", newSpacePADS.DataFlatMetaData["SPSNumber"]);
            }

            List<string> keysToRemove = new List<string> { "Slot", "WaferSequence" };
            foreach (string key in keysToRemove)
            {
                if (newSpacePADS.DataFlatMetaData.ContainsKey(key))
                {
                    newSpacePADS.DataFlatMetaData.Remove(key);
                }
            }
            AddBaseProperties<BaseStrangeDataFlatMetaDataPads>(newSpacePADS.StrangeDataFlatMetaData, renamedAttributesE4A);
            newSpacePADS.DataFlatMetaData.Add("BeginTimestamp", renamedAttributesE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SampleTimestamp));
            newSpacePADS.DataFlatMetaData.Add("BeginTimestampUtc", renamedAttributesE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SampleTimestampUtc));
            newSpacePADS.DataFlatMetaData.Add("EndTimestamp", renamedAttributesE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SampleTimestamp));
            newSpacePADS.DataFlatMetaData.Add("EndTimestampUtc", renamedAttributesE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SampleTimestampUtc));

            newSpacePADS.Data1ListParameters = new List<Data1ListPads>();
            var data1ListPADS = CreateData1ListPADS(renamedAttributesE4A, _attributePADSStructureMappingConfig.Data1List);
            newSpacePADS.Data1ListParameters.Add(data1ListPADS);

            return newSpacePADS;
        }

        private DocumentPads CreatePadsDocument(SpaceE4A spaceE4A)
        {
            var updatedTimeStampUtc = (DateTime) spaceE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.UpdatedTimestampUtc);
            var repetition = new RepetitionPads()
            {
                Id = "0",
                IdBaseValue = updatedTimeStampUtc
            };

            var document = new DocumentPads()
            {
                Type = "ProcessControl",
                Version = "1.0",
                Repetition = repetition
            };

            return document;
        }

        private ProductionActionPads CreateProductionAction(SpaceE4A renamedAttributesE4A)
        {
            var productionAction = new ProductionActionPads()
            {
                Id = GetProductionActionId(renamedAttributesE4A),
                Type = "SpaceAggregation",
            };

            return productionAction;
        }

        private static ItemPads CreateItem(SpaceE4A entry)
        {
            var itemPads = new ItemPads();
            // Check if 'IdSystemName' exists in SpaceAttributes
            if (entry.SpaceAttributes.ContainsKey("IdSystemName"))
            {
                itemPads.IdSystemName = (string)entry.SpaceAttributes["IdSystemName"];
                itemPads.Id = itemPads.IdSystemName + ":" + entry.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.MeasLot).ToString();
                itemPads.Type = "Fauf";
            }
            else
            {
                // If it doesn't exist, set default values for IdSystemName, Id and Type
                itemPads.Id = "Measlot:" + entry.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.MeasLot).ToString();
                itemPads.IdSystemName = "Measlot";
                itemPads.Type = "Lot";
            }

            return itemPads;
        }

        protected SystemLogPads CreateSystemLog(SpaceE4A entry, IQueueMessage queueMessage)
        {
            return new SystemLogPads(entry.SystemLog, queueMessage)
            {
                DocCreatedTimestampUtc = DateTime.UtcNow,
            };
        }

        private SearchPatternsPads CreateSearchPatterns(SpaceE4A renamedAttributesE4A, string lotAggregationId)
        {
            var searchPattern = new SearchPatternsPads()
            {
                SpaceKey = lotAggregationId,
                TimeGroup = "0",
                SiteKey = (string) renamedAttributesE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SiteKey)
            };

            return searchPattern;
        }

        internal string GetLotAggregationId(SpaceE4A e4aEntry, ItemPads item)
        {
            string id = item.Id;
            var parameterValueDictionary = new Dictionary<string, object>()
            {
                { "ItemId", id }
            };

            string lotAggregationId = GetId("GetLotAggregationId", _spaceIdCreationConfig, parameterValueDictionary, e4aEntry.SpaceAttributes);
            return lotAggregationId;
        }

        private string GetProductionActionId(SpaceE4A e4aEntry)
        {
            var parameterValueDictionary = new Dictionary<string, object>();

            string productionActionId = GetId("GetProductionActionId", _spaceIdCreationConfig, parameterValueDictionary, e4aEntry.SpaceAttributes);
            return productionActionId;
        }

        internal Data1ListPads CreateData1ListPADS(SpaceE4A renamedAttributesE4A, HashSet<string> data1ListAttributes)
        {
            var data1ListPads = new Data1ListPads();
            var data1ListAttributeMappings = renamedAttributesE4A.SpaceAttributes
                    .Where(x => data1ListAttributes.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Value);

            SetPropertiesFromDictionary(data1ListPads, data1ListAttributeMappings);
            data1ListPads.DataFlatLimits = CreateDataFlatLimits(renamedAttributesE4A);
            data1ListPads.Data1ListRawValues = CreateData1listRawValuesList(renamedAttributesE4A);
            data1ListPads.MeasurementAggregates = GetMeasurementAggregates(data1ListPads.Data1ListRawValues, data1ListPads.RvStoreFlag);
            return data1ListPads;
        }

        internal MeasurementAggregatesPads GetMeasurementAggregates(List<Data1ListRawValuesPads> data1ListRawValues, string rvStoreFlag)
        {
            MeasurementAggregatesPads measurementAggregates;
            string rawValuesExists = rvStoreFlag;
            if (string.Equals(rawValuesExists, "Y", StringComparison.OrdinalIgnoreCase))
            {
                var measurementRawValues = data1ListRawValues.Where(it => string.Equals(it.IsFlagged, "N", StringComparison.OrdinalIgnoreCase)).ToList();
                measurementAggregates = CreateMeasurementAggregates(data1ListRawValues.Count, measurementRawValues, data1ListRawValues);
                AddGofAggregates(measurementAggregates, measurementRawValues);
            }
            else if (string.Equals(rawValuesExists, "N", StringComparison.OrdinalIgnoreCase))
            {
                measurementAggregates = LoadMeasurementAggregates(data1ListRawValues);
            }
            else
            {
                return null;
            }

            return measurementAggregates;
        }

        /// <summary>
        /// To load the aggregates from space as it is when there are no rawvalues to calculate from.
        /// </summary>
        /// <param name="rawvalues"></param>
        private MeasurementAggregatesPads LoadMeasurementAggregates(List<Data1ListRawValuesPads> rawvalues)
        {
            var notFlaggedRawValues = rawvalues.Where(r => string.Equals(r.IsFlagged, "N", StringComparison.OrdinalIgnoreCase));
            var meanValues = notFlaggedRawValues.Select(r => r.SampleMean * r.SampleSize).ToList();
            var minValues = notFlaggedRawValues.Select(r => r.SampleMin).ToList();
            var maxValues = notFlaggedRawValues.Select(r => r.SampleMax).ToList();
            int flaggedCount = rawvalues.Where(r => !string.Equals(r.IsFlagged, "N", StringComparison.OrdinalIgnoreCase))
                                         .Select(r => r.SampleSize).Sum();

            var measurementAggregates = new MeasurementAggregatesPads();

            var rawvaluesValid = rawvalues.Where(it => it.IsFlagged == "N").ToList();
            var samples = rawvaluesValid.Select(i => i.SampleId).Distinct().ToList();
            measurementAggregates.Samples = string.Join(", ", samples.Distinct());

            int baseCount = CalculateBaseCount(rawvalues);
            if (baseCount > 0)
            {
                measurementAggregates.BaseCount = baseCount;
                measurementAggregates.FlaggedCount = flaggedCount;
                measurementAggregates.ExecCount = baseCount + flaggedCount;
                measurementAggregates.Mean = meanValues.Sum() / baseCount;
                measurementAggregates.Max = maxValues.Max();
                measurementAggregates.Min = minValues.Min();

                if (rawvalues.Select(r => r.PrimaryViolation).Distinct().Any())
                    measurementAggregates.PrimaryViolation = string.Join(", ", rawvalues.Select(r => r.PrimaryViolation).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                if (rawvalues.Select(r => r.PrimaryViolationComments).Distinct().Any())
                    measurementAggregates.PrimaryViolationComments = string.Join(", ", rawvalues.Select(r => r.PrimaryViolationComments).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                if (rawvalues.Select(r => r.ViolationComments).Distinct().Any())
                    measurementAggregates.ViolationComments = string.Join(", ", rawvalues.Select(r => r.ViolationComments).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                if (rawvalues.Select(r => r.ViolationList).Distinct().Any())
                    measurementAggregates.ViolationList = string.Join(", ", GetViolationList(rawvalues));

                measurementAggregates.NumViolations = (measurementAggregates.ViolationList?.Split(",", StringSplitOptions.RemoveEmptyEntries).Length) ?? 0;
            }

            return measurementAggregates;
        }

        private int CalculateBaseCount(List<Data1ListRawValuesPads> rawvalues)
        {
            var notFlaggedRawValues = rawvalues.Where(r => string.Equals(r.IsFlagged, "N", StringComparison.OrdinalIgnoreCase));
            var baseValues = notFlaggedRawValues.Select(r => r.SampleSize).ToList();
            int baseCount = baseValues.Sum();
            return baseCount;
        }

        /// <summary>
        /// Create RawValues List in a lot document
        /// </summary>
        /// <param name="e4aDocument"></param>
        public List<Data1ListRawValuesPads> CreateData1listRawValuesList(SpaceE4A e4aDocument)
        {
            var rawValuesPadsList = new List<Data1ListRawValuesPads>();
            foreach (var rawValuesEntry in e4aDocument.SpaceRawValueAttributeCollection)
            {
                long sampleId = Convert.ToInt64(rawValuesEntry.GetValueOrThrow(RequiredConvertedAttributes.SampleId));
                double value = (double) (rawValuesEntry.GetValueOrDefault(RequiredConvertedAttributes.Value) ?? 0.0);
                long sequenceNumber = (long) (rawValuesEntry.GetValueOrDefault(RequiredConvertedAttributes.SeqenceNumber) ?? 0L);
                bool entryExists = rawValuesPadsList.FirstOrDefault(i => sampleId == i.SampleId &&
                                                                         value == i.Value &&
                                                                         sequenceNumber == i.Seqnr
                                                                    ) != null;

                if (!entryExists)
                    rawValuesPadsList.Add(CreatData1ListRawValues(rawValuesEntry, e4aDocument));
            }

            return rawValuesPadsList;
        }

        /// <summary>
        /// Create Rawvalues to add up to the list
        /// </summary>
        /// <param name="rawValuesEntry"></param>
        /// <param name="e4aDocument"></param>
        internal Data1ListRawValuesPads CreatData1ListRawValues(IDictionary<string, object> rawValuesEntry, SpaceE4A e4aDocument)
        {
            var rawValuesPadsEntry = new Data1ListRawValuesPads();
            
            var spaceAttributeRawValuesMappings = e4aDocument.SpaceAttributes
                    .Where(x => _attributePADSStructureMappingConfig.Data1ListRawValues.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Value);

            var spaceRawValuesMappings = rawValuesEntry
                    .Where(x => _attributePADSStructureMappingConfig.Data1ListRawValues.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Value);

            SetPropertiesFromDictionary(rawValuesPadsEntry, spaceAttributeRawValuesMappings);
            SetPropertiesFromDictionary(rawValuesPadsEntry, spaceRawValuesMappings);

            rawValuesPadsEntry.IsFlagged = ((string) rawValuesEntry.GetValueOrDefault(RequiredConvertedAttributes.InternalFlagged)).Equals("Y", StringComparison.OrdinalIgnoreCase) ||
                                           ((string) rawValuesEntry.GetValueOrDefault(RequiredConvertedAttributes.ExternalFlagged)).Equals("Y", StringComparison.OrdinalIgnoreCase)
                                            ? "Y" : "N";

            double value = rawValuesEntry.GetValueOrDefault(RequiredConvertedAttributes.Value) as double? ?? 0.0;
            string rawValuesStoreFlagValue = (string) (e4aDocument.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.RvStoreFlag) ?? null);
            rawValuesPadsEntry.Value = !string.Equals(rawValuesStoreFlagValue, "N", StringComparison.OrdinalIgnoreCase) ? value : double.NaN;

            string sampleMaxValue = (string) e4aDocument.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Max);
            string sampleMinValue = (string) e4aDocument.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Min);
            string sampleMeanValue = (string) e4aDocument.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Mean);
            string sampleSigmaValue = (string) e4aDocument.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Sigma);
            string sampleMedianValue = (string) e4aDocument.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Median);
            string sampleQ1Value = (string) e4aDocument.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Q1);
            string sampleQ3Value = (string) e4aDocument.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Q3);

            rawValuesPadsEntry.SampleMax = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleMaxValue);
            rawValuesPadsEntry.SampleMin = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleMinValue);
            rawValuesPadsEntry.SampleMean = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleMeanValue);
            rawValuesPadsEntry.SampleStdev = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleSigmaValue);
            rawValuesPadsEntry.SampleMedian = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleMedianValue);
            rawValuesPadsEntry.SampleQ1 = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleQ1Value);
            rawValuesPadsEntry.SampleQ3 = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleQ3Value);


            string motherLotWafer = rawValuesPadsEntry.MotherlotWafer;
            if (motherLotWafer != null)
            {
                string proberId = "";
                if (rawValuesPadsEntry.X != null && rawValuesPadsEntry.Y != null)
                {
                    proberId = $"{motherLotWafer.Split(":")[0]}:{motherLotWafer.Split(":").Last()}:{rawValuesPadsEntry.X}:{rawValuesPadsEntry.Y}";
                }
                rawValuesPadsEntry.ItemIDFEProberChipID = proberId;
            }
            return rawValuesPadsEntry;
        }

        /// <summary>
        /// Create limits from e4a document data and child methods
        /// </summary>
        /// <param name="e4adoc">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns subdocument to the pads document in the parent method</returns>
        public DataFlatLimitsPads CreateDataFlatLimits(SpaceE4A e4adoc)
        {
            return new DataFlatLimitsPads()
            {
                CKCId = e4adoc.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.CKCId).ToString(),
                MeasurementSpecLimits = CreateMeasurementSpecLimits(e4adoc),
                ControlLimits = CreateControlLimits(e4adoc)
            };
        }

        /// <summary>
        /// creates control limits for ckc_id 0
        /// </summary>
        /// <param name="e4aDocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns controllimits0 to the dataflatlimits subdocument</returns>
        private ControlLimitsPads CreateControlLimits(SpaceE4A e4aDocument)
        {
            var controlLimits = new ControlLimitsPads();
            SetPropertiesFromDictionary(controlLimits, e4aDocument.SpaceAttributes);
            return controlLimits;
        }

        /// <summary>
        /// creates specification limits.
        /// </summary>
        /// <param name="e4aDocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns the specification limits to the MeasurementSpecLimits subdocument</returns>
        private MeasurementSpecLimitsPads CreateMeasurementSpecLimits(SpaceE4A e4aDocument)
        {
            var measurementSpecLimits = new MeasurementSpecLimitsPads();
            SetPropertiesFromDictionary(measurementSpecLimits, e4aDocument.SpaceAttributes);
            return measurementSpecLimits;
        }
    }
}
