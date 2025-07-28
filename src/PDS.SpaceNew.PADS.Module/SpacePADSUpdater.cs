using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using PDS.Common.E4AModel;
using PDS.Queue.Api.Message;
using PDS.Space.Common.Data.PADSModel;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.Common.Data.Config;
using PDS.SpaceNew.PADS.Module.Aggregations;
using PDS.SpaceNew.PADS.Module.Data.PADSModel;
using PDS.SpaceNew.PADS.Module.Helper;

namespace PDS.SpaceNew.PADS.Module
{
    internal class SpacePADSUpdater
    {
        private readonly SpaceLotAggregation _spaceE4AMapper;
        private readonly SpaceWaferAggregation _spaceWaferMapper;
        private readonly MethodInvokeConfig _methodInvokeConfig;
        private readonly PropertyPlacementConfig _attributePADSStructureMappingConfig;
        private readonly string _siteKey;

        internal SpacePADSUpdater(SpaceLotAggregation spaceE4AMapper, SpaceWaferAggregation spaceWaferAggregation,
            MethodInvokeConfig methodInvokeConfig, string siteKey, PropertyPlacementConfig attributePADSStructureMappingConfig)
        {
            _spaceE4AMapper = spaceE4AMapper;
            _spaceWaferMapper = spaceWaferAggregation;
            _methodInvokeConfig = methodInvokeConfig;
            _attributePADSStructureMappingConfig = attributePADSStructureMappingConfig;
            _siteKey = siteKey;
        }

        internal SpacePads UpdateDocument(SpacePads existingSpacePADS, SpacePads newSpacePADS, SpaceE4A newSpaceE4aDocument,
            IQueueMessage queueMessage)
        {
            var updatedSpacePads = new SpacePads
            {
                Document = newSpacePADS.Document,
                ProductionAction = newSpacePADS.ProductionAction,
                Item = newSpacePADS.Item,
                Id = existingSpacePADS.Id,
                SearchPatterns = newSpacePADS.SearchPatterns,

                DataFlatMetaData = new Dictionary<string, object>(),
                StrangeDataFlatMetaData = new Dictionary<string, object>()
            };

            // FlatMetaData
            AssignProperties(updatedSpacePads.DataFlatMetaData, newSpacePADS.DataFlatMetaData);
            AssignInvokeMethodPropertiesToDictionary(updatedSpacePads.DataFlatMetaData, new List<IDictionary<string, object>>() {
                existingSpacePADS.DataFlatMetaData, newSpacePADS.DataFlatMetaData
            });

            // StrangeMetaData
            AssignProperties(updatedSpacePads.StrangeDataFlatMetaData, newSpacePADS.StrangeDataFlatMetaData);
            AssignInvokeMethodPropertiesToDictionary(updatedSpacePads.StrangeDataFlatMetaData, new List<IDictionary<string, object>>() {
                existingSpacePADS.StrangeDataFlatMetaData, newSpacePADS.StrangeDataFlatMetaData
            });

            updatedSpacePads.Data1ListParameters = UpdateData1List(existingSpacePADS, newSpaceE4aDocument, newSpacePADS);
            updatedSpacePads.SystemLog = UpdateSystemLog(newSpaceE4aDocument, existingSpacePADS.SystemLog, queueMessage);

            return updatedSpacePads;
        }

        protected SystemLogPads UpdateSystemLog(SpaceE4A newSpaceE4aDocument, SystemLogPads padsSystemLog, IQueueMessage queueMessage)
        {
            var log = new SystemLogPads(newSpaceE4aDocument.SystemLog, queueMessage)
            {
                DocUpdatedTimestampUtc = DateTime.UtcNow
            };

            log.DocCreatedTimestampUtc = padsSystemLog != null ? padsSystemLog.DocCreatedTimestampUtc : log.DocUpdatedTimestampUtc.Value;

            return log;
        }

        private List<Data1ListPads> UpdateData1List(SpacePads existingSpacePADS, SpaceE4A newSpaceE4aDocument, SpacePads newSpacePADS)
        {
            var updatedData1ListParameters = existingSpacePADS.Data1ListParameters;
            var data1ListToUpdate = updatedData1ListParameters.FirstOrDefault(d => string.Equals(d.ParameterName, newSpaceE4aDocument.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.ParameterName).ToString(), StringComparison.OrdinalIgnoreCase) &&
                                                                                              string.Equals(d.ChannelId, newSpaceE4aDocument.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.ChannelId).ToString(), StringComparison.OrdinalIgnoreCase));
            if (data1ListToUpdate != null)
            {
                var newData1ListPads = UpdateData1List(newSpaceE4aDocument, data1ListToUpdate, newSpacePADS, _attributePADSStructureMappingConfig.Data1List);
                int data1ListToUpdateIndex = existingSpacePADS.Data1ListParameters.IndexOf(data1ListToUpdate);
                updatedData1ListParameters[data1ListToUpdateIndex] = newData1ListPads;
            }
            else
            {
                var data1ListPads = _spaceE4AMapper.CreateData1ListPADS(newSpaceE4aDocument, _attributePADSStructureMappingConfig.Data1List);
                updatedData1ListParameters.Add(data1ListPads);
            }

            return updatedData1ListParameters;
        }

        private Data1ListPads UpdateData1List(SpaceE4A newSpaceE4aDocument, Data1ListPads data1ListToUpdate, SpacePads newSpacePADS,
            HashSet<string> data1ListAttributes)
        {
            var data1ListPads = new Data1ListPads();
            var data1ListToUpdateDictionary = data1ListToUpdate.ToDictionary();

            var data1ListAttributeMappings = newSpaceE4aDocument.SpaceAttributes
                .Where(x => data1ListAttributes.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            var data1ListToUpdateData1ListMappings = data1ListToUpdateDictionary
                .Where(x => data1ListAttributes.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            var data1ListPadsDictionary = new Dictionary<string, object>();
            AssignProperties(data1ListPadsDictionary, data1ListAttributeMappings);
            AssignInvokeMethodPropertiesToDictionary(data1ListPadsDictionary, new List<IDictionary<string, object>>() {
                data1ListToUpdateData1ListMappings, data1ListAttributeMappings
            });

            // Add siteKey to dictionary for Normalize() method
            string siteKey = (string) newSpaceE4aDocument.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SiteKey);
            data1ListPadsDictionary[RequiredConvertedAttributes.SiteKey] = siteKey;

            data1ListPadsDictionary[RequiredConvertedAttributes.ChannelDescription] = data1ListToUpdateDictionary.GetValueOrDefault(RequiredConvertedAttributes.ChannelDescription);
            data1ListPadsDictionary[RequiredConvertedAttributes.SourceDataLevel] = data1ListToUpdateDictionary.GetValueOrDefault(RequiredConvertedAttributes.SourceDataLevel);

            SpaceBaseAggregation.SetPropertiesFromDictionary(data1ListPads, data1ListPadsDictionary);

            data1ListPads.Data1ListRawValues = UpdateData1ListRawValuesPADS(newSpaceE4aDocument, data1ListToUpdate, newSpacePADS);
            data1ListPads.DataFlatLimits = UpdateDataFlatLimits(newSpacePADS, data1ListToUpdate);
            data1ListPads.MeasurementAggregates = _spaceE4AMapper.GetMeasurementAggregates(data1ListPads.Data1ListRawValues, data1ListPads.RvStoreFlag);
            return data1ListPads;
        }

        /// <summary>
        /// updates dataflatlimits subdocument
        /// </summary>
        /// <param name="newSpacePADS">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <param name="data1ListToUpdate">existing mongodb document</param>
        /// <returns>returns the updated dataflatlimits subdocument to the parent method</returns>
        private DataFlatLimitsPads UpdateDataFlatLimits(SpacePads newSpacePADS, Data1ListPads data1ListToUpdate)
        {
            var data1List = newSpacePADS.Data1ListParameters[0];
            string newCkcId = data1List.DataFlatLimits.CKCId;
            return new DataFlatLimitsPads
            {
                CKCId = SpaceAggregationUtilsLocally.JoinStrings(data1ListToUpdate.DataFlatLimits.CKCId, newCkcId),
                MeasurementSpecLimits = UpdateMeasurementSpecLimits(data1List, data1ListToUpdate),
                ControlLimits = UpdateControlLimits(data1List, data1ListToUpdate)
            };
        }

        /// <summary>
        /// updates specification limits.
        /// </summary>
        /// <param name="data1List">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <param name="data1ListToUpdate">existing mongodb document</param>
        /// <returns>returns speclimits subdocument to the dataflatlimits subdocument</returns>
        private MeasurementSpecLimitsPads UpdateMeasurementSpecLimits(Data1ListPads data1List, Data1ListPads data1ListToUpdate)
        {
            var data1ListToUpdateLimits = data1ListToUpdate.DataFlatLimits.MeasurementSpecLimits;
            var data1ListLimits = data1List.DataFlatLimits.MeasurementSpecLimits;
            var measurementSpecLimits = new MeasurementSpecLimitsPads();

            var data1ListLimitsDictionary = data1ListLimits.ToDictionary();
            var data1ListToUpdateLimitsDictionary = data1ListToUpdateLimits.ToDictionary();

            var measurementSpecLimitsDictionary = new Dictionary<string, object>();
            AssignProperties(measurementSpecLimitsDictionary, data1ListLimitsDictionary);
            AssignInvokeMethodPropertiesToDictionary(measurementSpecLimitsDictionary, new List<IDictionary<string, object>>() {
                data1ListToUpdateLimitsDictionary, data1ListLimitsDictionary
            });

            SpaceBaseAggregation.SetPropertiesFromDictionary(measurementSpecLimits, measurementSpecLimitsDictionary);

            bool isLimitAmbiguous = GetIsMeasurementSpecLimitAmbiguous(data1ListLimits, data1ListToUpdateLimits);
            measurementSpecLimits.RemovalDue2Ambiguity = isLimitAmbiguous ? "Y" : null;
            return measurementSpecLimits;
        }

        private bool GetIsMeasurementSpecLimitAmbiguous(MeasurementSpecLimitsPads data1ListLimits, MeasurementSpecLimitsPads data1ListToUpdateLimits) =>
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.SpecHigh, data1ListToUpdateLimits.SpecHigh) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.SpecLow, data1ListToUpdateLimits.SpecLow) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.SpecTarget, data1ListToUpdateLimits.SpecTarget);
        
        /// <summary>
        /// updates the control limits for ckc_id = 0.
        /// </summary>
        /// <param name="data1List">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <param name="data1ListToUpdate">exisiting mongodb document</param>
        /// <returns> returns controllimits subdocument to the dataflatlimits subdocument</returns>
        private ControlLimitsPads UpdateControlLimits(Data1ListPads data1List, Data1ListPads data1ListToUpdate)
        {
            var controlLimits = new ControlLimitsPads();
            var data1ListToUpdateControlLimits = data1ListToUpdate.DataFlatLimits.ControlLimits;
            var data1ListControlLimits = data1List.DataFlatLimits.ControlLimits;

            var data1ListControlLimitsDictionary = data1ListControlLimits.ToDictionary();
            var data1ListToUpdateControlLimitsDictionary = data1ListToUpdateControlLimits.ToDictionary();

            var controlLimitsDictionary = new Dictionary<string, object>();
            AssignProperties(controlLimitsDictionary, data1ListControlLimitsDictionary);
            AssignInvokeMethodPropertiesToDictionary(controlLimitsDictionary, new List<IDictionary<string, object>>() {
                data1ListToUpdateControlLimitsDictionary, data1ListControlLimitsDictionary
            });

            SpaceBaseAggregation.SetPropertiesFromDictionary(controlLimits, controlLimitsDictionary);

            bool isLimitAmbiguous = GetIsControlLimitAmbiguous(data1ListToUpdateControlLimits, data1ListControlLimits);
            controlLimits.RemovalDue2Ambiguity = isLimitAmbiguous ? "Y" : null;
            return controlLimits;
        }

        private bool GetIsControlLimitAmbiguous(ControlLimitsPads data1ListLimits, ControlLimitsPads data1ListToUpdateLimits) =>
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.CntrlHigh, data1ListToUpdateLimits.CntrlHigh) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.CntrlLow, data1ListToUpdateLimits.CntrlLow) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.CntrlTarget, data1ListToUpdateLimits.CntrlTarget) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.RangeCntrlHigh, data1ListToUpdateLimits.RangeCntrlHigh) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.RangeCntrlLow, data1ListToUpdateLimits.RangeCntrlLow) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.RangeCntrlTarget, data1ListToUpdateLimits.RangeCntrlTarget) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.MeanCntrlHigh, data1ListToUpdateLimits.MeanCntrlHigh) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.SigmaCntrlLow, data1ListToUpdateLimits.SigmaCntrlLow) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.MeanCntrlLow, data1ListToUpdateLimits.MeanCntrlLow) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.SigmaCntrlTarget, data1ListToUpdateLimits.SigmaCntrlTarget) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.MeanCntrlTarget, data1ListToUpdateLimits.MeanCntrlTarget) ||
            SpaceAggregationUtilsLocally.IsLimitAmbigous(data1ListLimits.SigmaCntrlHigh, data1ListToUpdateLimits.SigmaCntrlHigh);

        /// <summary>
        /// updates the rawvalues in the existing document by verifying and
        /// adding non exisiting from the e4a document.
        /// </summary>
        /// <param name="newSpaceE4aDocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <param name="data1ListToUpdate">existing document from mongodb</param>
        /// <returns>returns updated list of rawvalues to the parent method</returns>
        private List<Data1ListRawValuesPads> UpdateData1ListRawValuesPADS(SpaceE4A newSpaceE4aDocument, Data1ListPads data1ListToUpdate, SpacePads newSpacePADS)
        {
            long newRawSampleId = newSpacePADS.Data1ListParameters[0].Data1ListRawValues.FirstOrDefault().SampleId;
            var newSpaceAggregates = newSpacePADS.Data1ListParameters[0].MeasurementAggregates;
            var newSpaceAttributes = newSpaceE4aDocument.SpaceAttributes;
            var padsRawValuesList = CreateNewRawValues(newSpaceE4aDocument, data1ListToUpdate, newSpaceAttributes, newSpaceAggregates);

            foreach (var existingRawValues in data1ListToUpdate.Data1ListRawValues)
            {
                AddExistingRawValues(padsRawValuesList, existingRawValues, newSpaceAttributes, newRawSampleId);
            }

            return padsRawValuesList;
        }

        private void AddExistingRawValues(List<Data1ListRawValuesPads> padsRawValuesList, Data1ListRawValuesPads existingRawValuesObject,
            IDictionary<string, object> newSpaceAttributes, long newRawSampleId)
        {
            var rawValues = new Data1ListRawValuesPads();
            var existingRawValuesDictionary = existingRawValuesObject.ToDictionary();

            var rawValuesDictionary = new Dictionary<string, object>();

            var existingRawValuesMappings = existingRawValuesDictionary
                    .Where(x => _attributePADSStructureMappingConfig.Data1ListRawValues.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Value);

            var spaceAttributeRawValuesMappings = newSpaceAttributes
                    .Where(x => _attributePADSStructureMappingConfig.Data1ListRawValues.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Value);
            existingRawValuesMappings.Remove("ProcessEquipment");
            AssignProperties(rawValuesDictionary, existingRawValuesMappings);
            AssignInvokeMethodPropertiesToDictionary(rawValuesDictionary, new List<IDictionary<string, object>>() {
                existingRawValuesMappings, spaceAttributeRawValuesMappings
            });

            SpaceBaseAggregation.SetPropertiesFromDictionary(rawValues, rawValuesDictionary);

            var sampleTimeStamp = (DateTime) newSpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SampleTimestamp);
            var sampleTimestampUtc = (DateTime) newSpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SampleTimestampUtc);
            var createdTimestamp = (DateTime) newSpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.CreatedTimestamp);
            var createdTimestampUtc = (DateTime) newSpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.CreatedTimestampUtc);
            var updatedTimestamp = (DateTime) newSpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.UpdatedTimestamp);
            var updatedTimestampUtc = (DateTime) newSpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.UpdatedTimestampUtc);
            string sampleMaxValue = (string) newSpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Max);
            string sampleMinValue = (string) newSpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Min);
            string sampleMeanValue = (string) newSpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Mean);
            string sampleSigmaValue = (string) newSpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Sigma);
            string sampleMedianValue = (string) newSpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Median);
            string sampleQ1Value = (string) newSpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Q1);
            string sampleQ3Value = (string) newSpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Q3);

            rawValues.SampleMax = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleMaxValue);
            rawValues.SampleMin = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleMinValue);
            rawValues.SampleMean = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleMeanValue);
            rawValues.SampleStdev = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleSigmaValue);
            rawValues.SampleMedian = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleMedianValue);
            rawValues.SampleQ1 = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleQ1Value);
            rawValues.SampleQ3 = SpaceAggregationUtilsLocally.ParseDoubleOrDefault(sampleQ3Value);
            rawValues.IsFlagged = ((string) existingRawValuesObject.IsFlagged);
            rawValues.SampleTimestamp = (existingRawValuesObject.SampleTimestamp != sampleTimeStamp && existingRawValuesObject.SampleId != newRawSampleId) ? existingRawValuesObject.SampleTimestamp : sampleTimeStamp;
            rawValues.SampleTimestampUtc = (existingRawValuesObject.SampleTimestampUtc != sampleTimestampUtc && existingRawValuesObject.SampleId != newRawSampleId) ? existingRawValuesObject.SampleTimestampUtc : sampleTimestampUtc;
            rawValues.CreatedTimestamp = (existingRawValuesObject.CreatedTimestamp != createdTimestamp && existingRawValuesObject.SampleId != newRawSampleId) ? existingRawValuesObject.CreatedTimestamp : createdTimestamp;
            rawValues.CreatedTimestampUtc = (existingRawValuesObject.CreatedTimestampUtc != createdTimestampUtc && existingRawValuesObject.SampleId != newRawSampleId) ? existingRawValuesObject.CreatedTimestampUtc : createdTimestampUtc;
            rawValues.UpdatedTimestamp = (existingRawValuesObject.UpdatedTimestamp != updatedTimestamp && existingRawValuesObject.SampleId != newRawSampleId) ? existingRawValuesObject.UpdatedTimestamp : updatedTimestamp;
            rawValues.UpdatedTimestampUtc = (existingRawValuesObject.UpdatedTimestampUtc != updatedTimestampUtc && existingRawValuesObject.SampleId != newRawSampleId) ? existingRawValuesObject.UpdatedTimestampUtc : updatedTimestampUtc;

            padsRawValuesList.Add(rawValues);
        }

        private List<Data1ListRawValuesPads> CreateNewRawValues(SpaceE4A newSpaceE4aDocument, Data1ListPads data1ListToUpdate,
            IDictionary<string, object> newSpaceAttributes, MeasurementAggregatesPads newSpaceAggregates)
        {
            var padsRawValuesList = new List<Data1ListRawValuesPads>();
            foreach (var e4aRawValues in newSpaceE4aDocument.SpaceRawValueAttributeCollection)
            {
                bool rawValueObjectExists = CheckIfData1ListRawValueObjectExists(e4aRawValues, data1ListToUpdate);
                if (!rawValueObjectExists)
                {
                    var rawValues = new Data1ListRawValuesPads();
                    var spaceAggregatesDictionary = newSpaceAggregates.ToDictionary();

                    var rawValuesDictionary = new Dictionary<string, object>();

                    // Add siteKey to dictionary for Normalize() method
                    string siteKey = (string) newSpaceE4aDocument.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SiteKey);
                    e4aRawValues[RequiredConvertedAttributes.SiteKey] = siteKey;

                    var spaceAttributeRawValuesMappings = newSpaceAttributes
                            .Where(x => _attributePADSStructureMappingConfig.Data1ListRawValues.Contains(x.Key))
                            .ToDictionary(x => x.Key, x => x.Value);

                    var spaceRawValuesMappings = e4aRawValues
                            .Where(x => _attributePADSStructureMappingConfig.Data1ListRawValues.Contains(x.Key))
                            .ToDictionary(x => x.Key, x => x.Value);

                    var spaceAggregatesRawValuesMappings = spaceAggregatesDictionary
                            .Where(x => _attributePADSStructureMappingConfig.Data1ListRawValues.Contains(x.Key))
                            .ToDictionary(x => x.Key, x => x.Value);

                    AssignProperties(rawValuesDictionary, spaceAttributeRawValuesMappings);
                    AssignProperties(rawValuesDictionary, spaceRawValuesMappings);
                    AssignInvokeMethodPropertiesToDictionary(rawValuesDictionary, new List<IDictionary<string, object>>() {
                        spaceAggregatesRawValuesMappings
                    });
                    AssignInvokeMethodPropertiesToDictionary(rawValuesDictionary, new List<IDictionary<string, object>>() {
                        spaceRawValuesMappings, spaceRawValuesMappings
                    });

                    SpaceBaseAggregation.SetPropertiesFromDictionary(rawValues, rawValuesDictionary);
                    string PE = rawValues.ProcessEquipment;
                    string internalFlagged = (string) e4aRawValues.GetValueOrDefault(RequiredConvertedAttributes.InternalFlagged);
                    string externalFlagged = (string) e4aRawValues.GetValueOrDefault(RequiredConvertedAttributes.ExternalFlagged);
                    rawValues.IsFlagged = (internalFlagged.Equals("Y", StringComparison.OrdinalIgnoreCase) ||
                                           externalFlagged.Equals("Y", StringComparison.OrdinalIgnoreCase)) ? "Y" : "N";

                    double value = (double) (e4aRawValues.GetValueOrDefault(RequiredConvertedAttributes.Value) ?? 0.0);
                    string rvStoreFlag = (string) (e4aRawValues.GetValueOrDefault(RequiredConvertedAttributes.RvStoreFlag) ?? null);
                    rawValues.Value = string.Equals(rvStoreFlag, "N", StringComparison.OrdinalIgnoreCase) ? double.NaN : value;

                    if (newSpaceE4aDocument.SpaceAttributes.TryGetValue(RequiredConvertedAttributes.SampleSize, out object sampleSizeObject))
                    {
                        if (sampleSizeObject is long sampleSizeLong)
                        {
                            rawValues.SampleSize = (int) sampleSizeLong;
                        }
                        else if (sampleSizeObject is string sampleSizeString)
                        {
                            if (int.TryParse(sampleSizeString, out int sampleSizeParsed))
                            {
                                rawValues.SampleSize = sampleSizeParsed;
                            }
                            else
                            {
                                Console.WriteLine("Invalid sample size value.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Unexpected type for sample size value.");
                        }
                    }
                    string motherLotWafer = rawValues.MotherlotWafer;
                    if (motherLotWafer != null)
                    {
                        string proberId = "";
                        if (rawValues.X != null && rawValues.Y != null)
                        {
                            proberId = $"{motherLotWafer.Split(":")[0]}:{motherLotWafer.Split(":").Last()}:{rawValues.X}:{rawValues.Y}";
                        }
                        rawValues.ItemIDFEProberChipID = proberId;
                    }
                    padsRawValuesList.Add(rawValues);
                }
            }

            return padsRawValuesList;
        }

        private bool CheckIfData1ListRawValueObjectExists(IDictionary<string, object> e4aRawValues, Data1ListPads data1ListToUpdate)
        {
            foreach (var checkRawValues in data1ListToUpdate.Data1ListRawValues)
            {
                bool rawValueObjectExists = CheckExisitingRawValues(e4aRawValues, checkRawValues);
                if (rawValueObjectExists)
                    return true;
            }

            return false;
        }

        internal bool CheckExisitingRawValues(IDictionary<string, object> e4aRawValues, Data1ListRawValuesPads checkRawValues)
        {
            string motherLotWafer = (string) e4aRawValues.GetValueOrDefault(RequiredConvertedAttributes.MotherlotWafer);
            long sampleId = Convert.ToInt64(e4aRawValues.GetValueOrThrow(RequiredConvertedAttributes.SampleId));
            double value = (double) (e4aRawValues.GetValueOrDefault(RequiredConvertedAttributes.Value) ?? 0.0);
            long sequenceNumber = (long) (e4aRawValues.GetValueOrDefault(RequiredConvertedAttributes.SeqenceNumber) ?? 0L);

            if (sampleId == checkRawValues.SampleId &&
                value == checkRawValues.Value &&
                sequenceNumber == checkRawValues.Seqnr &&
                string.Equals(motherLotWafer, checkRawValues.MotherlotWafer, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private void AssignProperties(IDictionary<string, object> targetMapping, IDictionary<string, object> parameterMapping)
        {
            foreach (var parameterMappingKeyValuePair in parameterMapping)
            {
                targetMapping[parameterMappingKeyValuePair.Key] = parameterMappingKeyValuePair.Value;
            }
        }

        private void AssignInvokeMethodPropertiesToDictionary(IDictionary<string, object> targetMapping, List<IDictionary<string, object>> inputParameterMappings)
        {
            // Add siteKey to inputParameters (this is necessary for SpaceAggregationUtilsLocally.Normalize() method)
            foreach (var inputParameterMapping in inputParameterMappings)
            {
                if (!inputParameterMapping.ContainsKey(RequiredConvertedAttributes.SiteKey))
                {
                    inputParameterMapping.Add(RequiredConvertedAttributes.SiteKey, _siteKey);
                }
            }

            int inputParameterCount = inputParameterMappings.Count;
            string inputParameterCountString = inputParameterCount.ToString();

            if (!_methodInvokeConfig.InputParameterCountMethodInvokeMapping.ContainsKey(inputParameterCountString))
            {
                throw new ArgumentOutOfRangeException($"{inputParameterCount} is not included in MethodInvokeConfig.json!");
            }

            var methodInvokeMappings = _methodInvokeConfig.InputParameterCountMethodInvokeMapping[inputParameterCountString];

            var allInputParameterNames = GetUniquePropertyNames(inputParameterMappings);
            var methodLeftParametersMappings = allInputParameterNames
                    .Select(p => GetMethodLeftParametersMapping(methodInvokeMappings, p))
                    .Where(p => p.Any()).ToList();

            foreach (var methodLeftParametersMapping in methodLeftParametersMappings)
            {
                foreach (var methodLeftParametersMappingKeyValuePair in methodLeftParametersMapping)
                {
                    string methodName = methodLeftParametersMappingKeyValuePair.Key;
                    var assignmentParameterNames = methodLeftParametersMappingKeyValuePair.Value;

                    foreach (string assignmentParameterName in assignmentParameterNames)
                    {
                        var methodInputParameterNames = methodInvokeMappings[methodName][assignmentParameterName];
                        var inputParameterValues = GetInputParameterValues(inputParameterMappings, methodInputParameterNames);
                        var inputParameters = inputParameterValues.Cast<object>().ToArray();
                        var returnValue = MethodInvoker.InvokeMethod(typeof(SpaceAggregationUtilsLocally), methodName, inputParameters);
                        if (returnValue != null) // We don't add properties with null value to dictionary
                        {
                            string returnValueAsString = returnValue.ToString();

                            if (targetMapping.ContainsKey(assignmentParameterName))
                            {
                                // overwrite it
                                targetMapping[assignmentParameterName] = returnValueAsString;
                            }
                            else
                            {
                                targetMapping.Add(assignmentParameterName, returnValueAsString);
                            }
                        }
                    }
                }
            }
        }

        private List<object> GetInputParameterValues(List<IDictionary<string, object>> inputParameterMappings, List<string> methodInputParameterNames)
        {
            if (inputParameterMappings.Count != methodInputParameterNames.Count)
                throw new ArgumentException($"Invalid configuration/implementation: Number of items in lists differ!");

            var inputParameterValues = new List<object>();
            for (int i = 0; i < inputParameterMappings.Count; i++)
            {
                var inputParameterMapping = inputParameterMappings[i];
                string methodInputParameterName = methodInputParameterNames[i];

                if (inputParameterMapping.ContainsKey(methodInputParameterName))
                {
                    inputParameterValues.Add(inputParameterMapping[methodInputParameterName]);
                }
                else
                {
                    inputParameterValues.Add(null);
                }
            }

            return inputParameterValues;
        }

        private IEnumerable<string> GetUniquePropertyNames(List<IDictionary<string, object>> propertyMappings)
        {
            return propertyMappings
                .SelectMany(dict => dict.Keys)
                .Distinct();
        }

        private Dictionary<string, List<string>> GetMethodLeftParametersMapping(Dictionary<string, Dictionary<string, List<string>>> methodInvokeMappings, string propertyName)
        {
            var methodLeftParametersMapping = new Dictionary<string, List<string>>();
                foreach (var methodInvokeMappingKeyValuePair in methodInvokeMappings)
                {
                    string methodName = methodInvokeMappingKeyValuePair.Key;
                    var propertyMethodDefinitions = methodInvokeMappingKeyValuePair.Value;

                foreach(var propertyMethodDefinitionKeyValuePair in propertyMethodDefinitions)
                    {
                        string assignmentParameterName = propertyMethodDefinitionKeyValuePair.Key;
                        var methodInputParameters = propertyMethodDefinitionKeyValuePair.Value;

                        if (methodInputParameters.Contains(propertyName))
                        {
                            if (!methodLeftParametersMapping.ContainsKey(methodName))
                            {
                                methodLeftParametersMapping.Add(methodName, new List<string>());
                            }

                            methodLeftParametersMapping[methodName].Add(assignmentParameterName);
                        }
                    }

                }

            return methodLeftParametersMapping;
        }

        internal SpacePads UpdateWaferSpacePADS(string waferAggregationId, string waferIdString,
            SpacePads newLotSpacePADS, SpacePads existingWaferDocument, SpaceE4A newSpaceE4a,
            List<Data1ListRawValuesPads4Wafer> waferValueList, IQueueMessage message)
        {
            string siteKey = (string) newSpaceE4a.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SiteKey);
            var updatedSpacePads = new SpacePads
            {
                Document = newLotSpacePADS.Document,
                ProductionAction = newLotSpacePADS.ProductionAction,
                Item = newLotSpacePADS.Item,
                Id = existingWaferDocument.Id,
                SearchPatterns = _spaceWaferMapper.CreateSearchPatterns(waferAggregationId, siteKey),

                DataFlatMetaData = new Dictionary<string, object>(),
                StrangeDataFlatMetaData = new Dictionary<string, object>()
            };

            updatedSpacePads.SystemLog = UpdateSystemLog(newSpaceE4a, existingWaferDocument.SystemLog, message);
            updatedSpacePads.Data1ListParameters = UpdateWaferData1List(newLotSpacePADS, existingWaferDocument,
                waferValueList, waferIdString, newSpaceE4a);

            updatedSpacePads.DataFlatMetaData = UpdateDataFlatMetaData(newLotSpacePADS, existingWaferDocument, waferValueList);
            updatedSpacePads.StrangeDataFlatMetaData = UpdateStrangeMetaData(newLotSpacePADS, existingWaferDocument);

            return updatedSpacePads;
        }

        private IDictionary<string, object> UpdateStrangeMetaData(SpacePads newLotSpacePADS, SpacePads existingWaferDocument)
        {
            var strangeDataFlatMetaData = new Dictionary<string, object>();
            var inputParameterMappings = new List<IDictionary<string, object>>() { existingWaferDocument.StrangeDataFlatMetaData, newLotSpacePADS.StrangeDataFlatMetaData };
            AssignInvokeMethodPropertiesToDictionary(strangeDataFlatMetaData, inputParameterMappings);
            return strangeDataFlatMetaData;
        }

        private IDictionary<string, object> UpdateDataFlatMetaData(SpacePads newLotSpacePADS, SpacePads existingWaferDocument, List<Data1ListRawValuesPads4Wafer> waferValueList)
        {
            var lotRawvaluesList = new List<Data1ListRawValuesPads>();
            // TODO: Clarify if this algorithm should work like this:
            //foreach (var data1List in newLotSpacePADS.Data1ListParameters)
            //{
            //    var waferValueObject = waferValueList.FirstOrDefault(l => string.Equals(l.ParameterName, data1List.ParameterName, StringComparison.OrdinalIgnoreCase));
            //    if (waferValueObject != null)
            //        lotRawvaluesList = data1List.Data1ListRawValues;
            //}

            // Would translate to:
            lotRawvaluesList = newLotSpacePADS.Data1ListParameters
                    .Where(d => waferValueList.Any(w => string.Equals(w.ParameterName, d.ParameterName, StringComparison.InvariantCultureIgnoreCase)))
                    .Select(d => d.Data1ListRawValues)
                    .LastOrDefault();

            var listSampleTimestamps = new List<DateTime>();
            var listSamplesTimestampsUtc = new List<DateTime>();
            string existingWaferLotIdString = (string) existingWaferDocument.DataFlatMetaData.GetValueOrDefault(RequiredConvertedAttributes.Wafer);
            int existingWaferLotId = int.Parse(existingWaferLotIdString);
            foreach (var rawValuesObject in lotRawvaluesList)
            {
                int waferLotId = int.Parse(rawValuesObject.MotherlotWafer.Split(':').Last());
                if (waferLotId == existingWaferLotId)
                {
                    listSampleTimestamps.Add(rawValuesObject.SampleTimestamp);
                    listSamplesTimestampsUtc.Add(rawValuesObject.SampleTimestampUtc);
                }
            }

            var dataFlatMetaData = new Dictionary<string, object>();
            AssignProperties(dataFlatMetaData, newLotSpacePADS.DataFlatMetaData);
            AssignInvokeMethodPropertiesToDictionary(dataFlatMetaData, new List<IDictionary<string, object>>() { existingWaferDocument.DataFlatMetaData, newLotSpacePADS.DataFlatMetaData} );

            string existingMotherLot = (string) existingWaferDocument.DataFlatMetaData.GetValueOrDefault(RequiredConvertedAttributes.MotherLot);
            string existingWafer = (string) existingWaferDocument.DataFlatMetaData.GetValueOrDefault(RequiredConvertedAttributes.Wafer);
            dataFlatMetaData[RequiredConvertedAttributes.MotherLot] = existingMotherLot;
            dataFlatMetaData[RequiredConvertedAttributes.Wafer] = existingWafer;

            var existingBeginTimeStampUtc = (DateTime) existingWaferDocument.DataFlatMetaData.GetValueOrThrow(RequiredConvertedAttributes.BeginTimestampUtc);
            var existingBeginTimestamp = (DateTime) existingWaferDocument.DataFlatMetaData.GetValueOrThrow(RequiredConvertedAttributes.BeginTimestamp);
            var existingEndTimestampUtc = (DateTime) existingWaferDocument.DataFlatMetaData.GetValueOrThrow(RequiredConvertedAttributes.EndTimestampUtc);
            var existingEndTimestamp = (DateTime) existingWaferDocument.DataFlatMetaData.GetValueOrThrow(RequiredConvertedAttributes.EndTimestamp);

            dataFlatMetaData[RequiredConvertedAttributes.BeginTimestampUtc] = listSamplesTimestampsUtc.Any() ? SpaceAggregationUtilsLocally.MinDate(existingBeginTimeStampUtc, listSamplesTimestampsUtc.Min()) : DateTime.MinValue;
            dataFlatMetaData[RequiredConvertedAttributes.BeginTimestamp] = listSampleTimestamps.Any() ? SpaceAggregationUtilsLocally.MinDate(existingBeginTimestamp, listSampleTimestamps.Min()) : DateTime.MinValue;
            dataFlatMetaData[RequiredConvertedAttributes.EndTimestampUtc] = listSamplesTimestampsUtc.Any() ? SpaceAggregationUtilsLocally.MaxDate(existingEndTimestampUtc, listSamplesTimestampsUtc.Max()) : DateTime.MinValue;
            dataFlatMetaData[RequiredConvertedAttributes.EndTimestamp] = listSampleTimestamps.Any() ? SpaceAggregationUtilsLocally.MaxDate(existingEndTimestamp, listSampleTimestamps.Max()) : DateTime.MinValue;

            return dataFlatMetaData;
        }

        private List<Data1ListPads> UpdateWaferData1List(SpacePads newLotSpacePADS, SpacePads existingWaferDocument,
            List<Data1ListRawValuesPads4Wafer> waferValueList, string waferIdString, SpaceE4A newSpaceE4a)
        {
            var waferData1ListCollection = new List<Data1ListPads>();
            string existingWaferLotIdString = (string) existingWaferDocument.DataFlatMetaData.GetValueOrDefault(RequiredConvertedAttributes.Wafer);
            if (string.Equals(existingWaferLotIdString, waferIdString, StringComparison.OrdinalIgnoreCase))
            {
                return waferData1ListCollection;
            }

            var existingChannelIds = existingWaferDocument.Data1ListParameters.Select(w => w.ChannelId);
            var existingParameterNames = existingWaferDocument.Data1ListParameters.Select(w => w.ChannelId);

            var wafersData1ListToCreate = newLotSpacePADS.Data1ListParameters.Where(p =>
                    !existingParameterNames.Contains(p.ParameterName) ||
                    !existingChannelIds.Contains(p.ChannelId));

            foreach (var waferData1ListToCreate in wafersData1ListToCreate)
            {
                var waferData1List = CreateWaferData1List(existingWaferDocument,
                                        waferValueList, waferIdString, waferData1ListToCreate, newSpaceE4a);
                if (waferData1List != null)
                    waferData1ListCollection.Add(waferData1List);
            }

            foreach (var existingData1List in existingWaferDocument.Data1ListParameters)
            {
                UpdateExistingWaferData1List(waferData1ListCollection, existingData1List);
            }

            return waferData1ListCollection;
        }

        private void UpdateExistingWaferData1List(List<Data1ListPads> waferData1ListCollection, Data1ListPads existingData1List)
        {
            var existingWaferData1List = waferData1ListCollection.FirstOrDefault(d =>
                    string.Equals(existingData1List.ParameterName, d.ParameterName, StringComparison.OrdinalIgnoreCase));
            if (existingWaferData1List != null)
            {
                return;
            }

            var newWaferData1List = new Data1ListPads();
            var waferData1ListToCreateAsDictionary = existingData1List.ToDictionary();
            SpaceBaseAggregation.SetPropertiesFromDictionary(newWaferData1List, waferData1ListToCreateAsDictionary);

            if (newWaferData1List.ParameterName == null)
                return;

            newWaferData1List.DataFlatLimits = existingData1List.DataFlatLimits;
            newWaferData1List.MeasurementAggregates = existingData1List.MeasurementAggregates;
            waferData1ListCollection.Add(newWaferData1List);
        }

        private Data1ListPads CreateWaferData1List(SpacePads existingWaferDocument, List<Data1ListRawValuesPads4Wafer> waferValueList,
            string waferIdString, Data1ListPads waferData1ListToCreate, SpaceE4A newSpaceE4a)
        {
            var existingWaferData1List = existingWaferDocument.Data1ListParameters.FirstOrDefault(d =>
                string.Equals(waferData1ListToCreate.ParameterName, d.ParameterName, StringComparison.OrdinalIgnoreCase));
            if (existingWaferData1List != null)
            {
                return null;
            }

            var newWaferData1List = new Data1ListPads();
            var waferData1ListToCreateAsDictionary = waferData1ListToCreate.ToDictionary();
            SpaceBaseAggregation.SetPropertiesFromDictionary(newWaferData1List, waferData1ListToCreateAsDictionary);

            if (newWaferData1List.ParameterName == null)
                return null;

            newWaferData1List.DataFlatLimits = _spaceE4AMapper.CreateDataFlatLimits(newSpaceE4a);
            newWaferData1List.MeasurementAggregates = _spaceWaferMapper.GetMeasurementAggregates(waferData1ListToCreate, waferIdString, waferValueList);
            return newWaferData1List;
        }
    }
}
