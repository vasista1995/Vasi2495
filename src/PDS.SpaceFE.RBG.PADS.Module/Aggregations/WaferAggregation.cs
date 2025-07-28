using System;
using System.Linq;
using System.Collections.Generic;
using PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel;
using PDS.Space.Common.Aggregations;
using PDS.Space.Common.Data.PADSModel;
using PDS.SpaceFE.RBG.Common.Data.E4AModel;
using PDS.Queue.Api.Message;
using System.Diagnostics.CodeAnalysis;
using PDS.Space.Common;

namespace PDS.SpaceFE.RBG.PADS.Module.Aggregations
{
    /// <summary>
    /// This class Wafer Aggregation has all the required ethods to create a wafer document to be loaded to PADS collection
    /// </summary>
    public class WaferAggregation : BaseAggregation
    {
        public WaferAggregation([NotNull] SpaceE4A e4aDocument, [NotNull] IQueueMessage message) : base(e4aDocument, message)
        {
        }

        /// <summary>
        /// This method creates a new Wafer document
        /// </summary>
        /// <param name="wafAggId"></param>
        /// <param name="wafer"></param>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkValueList"></param>
        public SpacePads CreateNew(string wafAggId, string wafer, SpacePads lotaggdocument, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var spacePADS = CreateCommon(lotaggdocument, wafer, wafAggId);
            spacePADS.DataFlatMetaData = CreateDataFlatMetaData(lotaggdocument, wafer, checkValueList);
            spacePADS.StrangeDataFlatMetaData = CreateStrangeMetaData(lotaggdocument);
            spacePADS.Data1List = CreateData1List(lotaggdocument, wafer, checkValueList);
            spacePADS.SystemLog = CreateSystemLog();
            return spacePADS;
        }
        /// <summary>
        /// Create DataList for a lot aggregation document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="wafer"></param>
        /// <param name="checkValueList"></param>,
        internal static List<Data1ListPads> CreateData1List(SpacePads lotaggdocument, string wafer, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var dataList = new List<Data1ListPads>();
            string processEquipment = "";
            var parameters = new List<string>();
            foreach (var values in checkValueList)
            {
                if (values.ItemIdMotherlotWafer.Split(":").Last() == wafer)
                {
                    processEquipment = values.ProcessEquipment;
                    parameters.Add(values.ParameterName);
                }
            }
            foreach (var lotDataList in lotaggdocument.Data1List)
            {
                var data1List = new Data1ListPads();
                if (parameters.Contains(lotDataList.ParameterName))
                {
                    BaseWaferAggregation.InitCreateData1List(data1List, lotDataList);
                    data1List.CreatedTimestamp = lotDataList.Data1ListRawValues.Min(it => it.CreatedTimestamp);
                    data1List.CreatedTimestampUtc = lotDataList.Data1ListRawValues.Min(it => it.CreatedTimestampUtc);
                    data1List.UpdatedTimestamp = lotDataList.Data1ListRawValues.Max(it => it.UpdatedTimestamp);
                    data1List.UpdatedTimestampUtc = lotDataList.Data1ListRawValues.Max(it => it.UpdatedTimestampUtc);
                    data1List.ProcessEquipment = processEquipment;
                    data1List.ProcessBatch = lotDataList.ProcessBatch;
                    data1List.DataFlatLimits = CreateDataFlatLimits(lotaggdocument, checkValueList);
                    if (data1List.RvStoreFlag == "Y")
                    {
                        data1List.MeasurementAggregates = CreateMeasurementAggregates(data1List.ParameterName, data1List.ChannelId, checkValueList);
                    }
                    else if (data1List.RvStoreFlag == "N")
                    {
                        data1List.MeasurementAggregates = LoadMeasurementAggregates(data1List.ParameterName, data1List.ChannelId, checkValueList);
                    }
                    dataList.Add(data1List);
                }
            }
            return dataList;
        }
        /// <summary>
        /// Load the aggregates when there are no raw values to calculate
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="channelId"></param>
        /// <param name="checkValueList"></param>
        private static MeasurementAggregatesPads LoadMeasurementAggregates(string parameter, string channelId, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            int baseCount = 0;
            int flaggedCount = 0;
            double maxval = 0.0;
            double minval = 0.0;
            double meanval = 0.0;
            var rawvalue = checkValueList.Where(it => it.ParameterName == parameter && it.ChannelId == channelId).ToList();
            var rawvaluesValid = rawvalue.Where(i => i.IsFlagged == "N").ToList();
            var samples = rawvaluesValid.Select(i => i.SampleId).Distinct().ToList();
            var primaryViolations = new List<string>();
            var primaryViolComm = new List<string>();
            var violationComments = new List<string>();
            var violationList = new List<string>();

            foreach (var rawval in rawvalue)
            {
                if (rawval.ViolationList?.Contains(",") == true)
                {
                    violationList.AddRange(rawval.ViolationList.Split(","));
                }
                else if (rawval.ViolationList != null)
                {
                    violationList.Add(rawval.ViolationList);
                }
                if (rawval.ViolationComments?.Contains(",") == true)
                {
                    violationComments.AddRange(rawval.ViolationComments.Split(","));
                }
                else if (rawval.ViolationComments != null)
                {
                    violationComments.Add(rawval.ViolationComments);
                }
                if (rawval.PrimaryViolation != null)
                {
                    primaryViolations.Add(rawval.PrimaryViolation);
                }
                if (rawval.PrimaryViolationComments != null)
                {
                    primaryViolComm.Add(rawval.PrimaryViolationComments);
                }
            }
            var rawValNotFlagged = rawvalue.Where(it => it.IsFlagged == "N").ToList();
            if (rawValNotFlagged.Count > 0)
            {
                baseCount = rawValNotFlagged.Select(it => it.SampleSize).Sum();
                meanval = rawValNotFlagged.Select(it => it.SampleMean * it.SampleSize).Sum() / baseCount;
                minval = rawValNotFlagged.Min(it => it.SampleMin);
                maxval = rawValNotFlagged.Max(it => it.SampleMax);
            }
            var rawValFlagged = rawvalue.Where(it => it.IsFlagged == "Y").ToList();
            if (rawValFlagged.Count > 0)
            {
                flaggedCount = rawValFlagged.Select(it => it.SampleSize).Sum();
            }
            var measurementAggregates = new MeasurementAggregatesPads
            {
                BaseCount = baseCount,
                FlaggedCount = flaggedCount,
                ExecCount = baseCount + flaggedCount,
                Mean = meanval,
                Max = maxval,
                Min = minval
            };
            measurementAggregates.PrimaryViolation = primaryViolations.Count > 0 ? string.Join(", ", primaryViolations.Distinct()) : null;
            measurementAggregates.PrimaryViolationComments = primaryViolComm.Count > 0 ? string.Join(", ", primaryViolComm.Distinct()) : null;
            measurementAggregates.ViolationComments = violationComments.Count > 0 ? string.Join(", ", violationComments.Distinct()) : null;
            measurementAggregates.ViolationList = violationList.Count > 0 ? string.Join(", ", violationList.Distinct()) : null;
            measurementAggregates.NumViolations = violationList.Distinct().Count();
            measurementAggregates.Samples = samples.Count > 0 ? string.Join(", ", samples) : null;
            return measurementAggregates;
        }
        /// <summary>
        /// Update existing PADS document with new incoming data
        /// </summary>
        /// <param name="wafAggId"></param>
        /// <param name="wafer"></param>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkdocument"></param>
        /// <param name="checkValueList"></param>
        public SpacePads UpdateExisting(string wafAggId, string wafer, SpacePads lotaggdocument, SpacePads checkdocument, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var spacePADS = CreateCommon(lotaggdocument, wafer, wafAggId);
            spacePADS.Id = checkdocument.Id;
            spacePADS.DataFlatMetaData = UpdateDataFlatMetaData(lotaggdocument, checkdocument, checkValueList);
            spacePADS.StrangeDataFlatMetaData = UpdateStrangeMetaData(lotaggdocument, checkdocument);
            spacePADS.Data1List = UpdateData1List(lotaggdocument, checkdocument, checkValueList, wafer);
            spacePADS.SystemLog = UpdateSystemLog(checkdocument.SystemLog);
            return spacePADS;
        }
        /// <summary>
        /// Updating Data1List with a new parametername in an already existing document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkdocument"></param>
        /// <param name="checkValueList"></param>
        /// <param name="wafer"></param>
        internal static List<Data1ListPads> UpdateData1List(SpacePads lotaggdocument, SpacePads checkdocument, List<Data1ListRawValuesPads4Wafer> checkValueList, string wafer)
        {
            var dataList = new List<Data1ListPads>();
            var checkedParams = checkdocument.Data1List.ConvertAll(it => it.ParameterName);
            var checkedchids = checkdocument.Data1List.ConvertAll(it => it.ChannelId);
            foreach (var lotparamvalues in lotaggdocument.Data1List)
            {
                if ((checkdocument.DataFlatMetaData.Wafer == wafer && !checkedParams.Contains(lotparamvalues.ParameterName)) ||
                    !checkedchids.Contains(lotparamvalues.ChannelId))
                {
                    UpdateData1ListNew(lotaggdocument, checkdocument, checkValueList, dataList, wafer, lotparamvalues);
                }
            }
            foreach (var checkparams in checkdocument.Data1List)
            {
                if (wafer == checkdocument.DataFlatMetaData.Wafer)
                {
                    UpdateData1ListExisting(dataList, checkparams, checkValueList, wafer);
                }
            }
            return dataList;
        }
        internal static void UpdateData1ListExisting(List<Data1ListPads> dataList, Data1ListPads checkparams, List<Data1ListRawValuesPads4Wafer> checkValueList, string wafer)
        {
            var otherparams = new Data1ListPads();
            bool isEqual = false;
            var measurementAggregates = new MeasurementAggregatesPads();
            var values = checkValueList.Where(it => it.ItemIdMotherlotWafer.Split(":").Last() == wafer).ToList();
            var listprocessEquipment = values.Select(it => it.ProcessEquipment).Distinct().ToList();
            if (checkparams.RvStoreFlag == "Y")
            {
                measurementAggregates = CreateMeasurementAggregates(checkparams.ParameterName, checkparams.ChannelId, checkValueList);
            }
            else if (checkparams.RvStoreFlag == "N")
            {
                measurementAggregates = LoadMeasurementAggregates(checkparams.ParameterName, checkparams.ChannelId, checkValueList);
            }
            foreach (var dataparam in dataList)
            {
                if (checkparams.ParameterName == dataparam.ParameterName && checkparams.ChannelId == dataparam.ChannelId)
                    isEqual = true;
            }
            if (!isEqual)
            {
                BaseWaferAggregation.InitCreateData1List(otherparams, checkparams);
                otherparams.CreatedTimestamp = checkparams.CreatedTimestamp;
                otherparams.CreatedTimestampUtc = checkparams.CreatedTimestampUtc;
                otherparams.UpdatedTimestamp = checkparams.UpdatedTimestamp;
                otherparams.UpdatedTimestampUtc = checkparams.UpdatedTimestampUtc;
                otherparams.ProcessBatch = checkparams.ProcessBatch;
                otherparams.DataFlatLimits = checkparams.DataFlatLimits;
                otherparams.ProcessEquipment = listprocessEquipment.Count == 0 ? null : string.Join(", ", listprocessEquipment.AsEnumerable());
                otherparams.MeasurementAggregates = measurementAggregates;
            }
            if (otherparams.ParameterName != null)
            {
                dataList.Add(otherparams);
            }
        }
        internal static void UpdateData1ListNew(SpacePads lotaggdocument, SpacePads checkdocument, List<Data1ListRawValuesPads4Wafer> checkValueList, List<Data1ListPads> dataList, string wafer, Data1ListPads lotparamvalues)
        {
            var data1List = new Data1ListPads();
            var measurementAggregates = new MeasurementAggregatesPads();
            var values = checkValueList.Where(it => it.ItemIdMotherlotWafer.Split(":").Last() == wafer).ToList();
            var listprocessEquipment = values.Select(it => it.ProcessEquipment).Distinct().ToList();
            var checkedParams = checkValueList.Select(it => it.ParameterName).Distinct().ToList();
            var checkedchids = checkValueList.Select(it => it.ChannelId).Distinct().ToList();
            if (lotparamvalues.RvStoreFlag == "Y")
            {
                measurementAggregates = CreateMeasurementAggregates(lotparamvalues.ParameterName, lotparamvalues.ChannelId, checkValueList);
            }
            else if (lotparamvalues.RvStoreFlag == "N")
            {
                measurementAggregates = LoadMeasurementAggregates(lotparamvalues.ParameterName, lotparamvalues.ChannelId, checkValueList);
            }
            bool isEqual = false;
            foreach (var checkparams in checkdocument.Data1List)
            {
                if (!checkedParams.Contains(lotparamvalues.ParameterName) || !checkedchids.Contains(lotparamvalues.ChannelId))
                {
                    if (lotparamvalues.ParameterName == checkparams.ParameterName && lotparamvalues.ChannelId == checkparams.ChannelId)
                    {
                        isEqual = true;
                    }
                    isEqual = true;
                }
            }
            if (!isEqual)
            {
                BaseWaferAggregation.InitCreateData1List(data1List, lotparamvalues);
                data1List.ProcessEquipment = listprocessEquipment.Count == 0 ? null : string.Join(", ", listprocessEquipment.AsEnumerable());
                data1List.CreatedTimestamp = values.Min(it => it.CreatedTimestamp);
                data1List.CreatedTimestampUtc = values.Min(it => it.CreatedTimestampUtc);
                data1List.UpdatedTimestamp = values.Max(it => it.UpdatedTimestamp);
                data1List.UpdatedTimestampUtc = values.Max(it => it.UpdatedTimestampUtc);
                data1List.ProcessBatch = lotparamvalues.ProcessBatch;
                data1List.DataFlatLimits = CreateDataFlatLimits(lotaggdocument, checkValueList);
                data1List.MeasurementAggregates = measurementAggregates;
            }
            if (data1List.ParameterName != null)
            {
                dataList.Add(data1List);
            }
        }

        /// <summary>
        /// Create all the common fields in both creating and updating wafer documents
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="wafer"></param>
        /// <param name="wafAggId"></param>
        private static SpacePads CreateCommon(SpacePads lotaggdocument, string wafer, string wafAggId)
        {
            var repetition = new RepetitionPads()
            {
                Id = "0",
                IdBaseValue = lotaggdocument.DataFlatMetaData.EndTimestampUtc
            };
            var document = new DocumentPads()
            {
                Type = "ProcessControl",
                Version = "1.0",
                Repetition = repetition
            };

            var productionAction = new ProductionActionPads()
            {
                Id = lotaggdocument.ProductionAction.Id,
                Type = "SpaceAggregation"
            };
            var item = new ItemPads()
            {
                Id = "MotherlotWafer:" + lotaggdocument.DataFlatMetaData.MeasLot.Substring(0, 8) + ":" + wafer,
                IdSystemName = "MotherlotWafer",
                Type = "Wafer"
            };
            var searchPatterns = new SearchPatternsPads()
            {
                SpaceKey = wafAggId,
                TimeGroup = "0",
                SiteKey = lotaggdocument.DataFlatMetaData.SiteKey
            };
            return new SpacePads()
            {
                Document = document,
                ProductionAction = productionAction,
                Item = item,
                SearchPatterns = searchPatterns
            };
        }
        /// <summary>
        /// Create strange meta data fields in a new wafer document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        private static StrangeDataFlatMetaDataPads CreateStrangeMetaData(SpacePads lotaggdocument)
        {
            var lotMetaData = lotaggdocument.StrangeDataFlatMetaData;
            var dataFlatStrangeMetaData = new StrangeDataFlatMetaDataPads();
            BaseWaferAggregation.InitCreateStrangeData(dataFlatStrangeMetaData, lotMetaData);
            dataFlatStrangeMetaData.ProcessGroup = lotMetaData.ProcessGroup;
            dataFlatStrangeMetaData.ProcessLine = lotMetaData.ProcessLine;
            dataFlatStrangeMetaData.Design = lotMetaData.Design;
            dataFlatStrangeMetaData.Layer = lotMetaData.Layer;
            dataFlatStrangeMetaData.MeasurementBatch = lotMetaData.MeasurementBatch;
            return dataFlatStrangeMetaData;
        }
        /// <summary>
        /// Create Data flat metadata for a new wafer document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="wafer"></param>
        /// <param name="checkValueList"></param>
        private static DataFlatMetaDataPads CreateDataFlatMetaData(SpacePads lotaggdocument, string wafer, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            string slot = "";
            string waferSequence = "";
            string testPosition = "";
            var listSampleTimestamps = new List<DateTime>();
            var listSampleTimestampsUtc = new List<DateTime>();
            var rawvaluesList = new List<Data1ListRawValuesPads>();
            var lotMetaData = lotaggdocument.DataFlatMetaData;
            foreach (var datalist in lotaggdocument.Data1List)
            {
                foreach (var checkparams in checkValueList)
                {
                    if (datalist.ParameterName == checkparams.ParameterName && datalist.ChannelId == checkparams.ChannelId)
                    {
                        rawvaluesList = datalist.Data1ListRawValues;
                    }
                }
            }
            foreach (var rawvalues in rawvaluesList)
            {
                if (rawvalues.Wafer == wafer)
                {
                    slot = rawvalues.Slot;
                    waferSequence = rawvalues.WaferSequence;
                    testPosition = rawvalues.TestPosition;
                    listSampleTimestamps.Add(rawvalues.SampleTimestamp);
                    listSampleTimestampsUtc.Add(rawvalues.SampleTimestampUtc);
                }
            }
            var dataFlatMetaData = new DataFlatMetaDataPads();
            BaseWaferAggregation.InitCreateDataFlatMetaData(dataFlatMetaData, lotMetaData);
            dataFlatMetaData.Motherlot = lotMetaData.MeasLot.Substring(0, 8);
            dataFlatMetaData.Wafer = wafer;
            dataFlatMetaData.BeginTimestampUtc = listSampleTimestampsUtc.Min();
            dataFlatMetaData.BeginTimestamp = listSampleTimestamps.Min();
            dataFlatMetaData.EndTimestampUtc = listSampleTimestampsUtc.Max();
            dataFlatMetaData.EndTimestamp = listSampleTimestamps.Max();
            dataFlatMetaData.SPSNumber = lotMetaData.SPSNumber;
            dataFlatMetaData.SubOperation = lotMetaData.SPSNumber;
            dataFlatMetaData.Slot = slot;
            dataFlatMetaData.TestPosition = testPosition;
            dataFlatMetaData.WaferSequence = waferSequence;
            dataFlatMetaData.EquipmentType = lotMetaData.EquipmentType;
            dataFlatMetaData.Recipe = lotMetaData.Recipe;
            return dataFlatMetaData;
        }
        /// <summary>
        /// Create limits for new wafer documents
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkValueList"></param>
        private static DataFlatLimitsPads CreateDataFlatLimits(SpacePads lotaggdocument, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var dataList = new Data1ListPads();
            foreach (var datalist in lotaggdocument.Data1List)
            {
                foreach (var checkparams in checkValueList)
                {
                    if (datalist.ParameterName == checkparams.ParameterName && datalist.ChannelId == checkparams.ChannelId)
                    {
                        dataList = datalist;
                    }
                }
            }
            return new DataFlatLimitsPads()
            {
                CKCId = dataList.DataFlatLimits.CKCId,
                MeasurementSpecLimits = CreateMeasurementSpecLimits(lotaggdocument, checkValueList),
                ControlLimits = CreateControlLimits(lotaggdocument, checkValueList)
            };
        }
        /// <summary>
        /// Create control limits for a new wafer document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkValueList"></param>
        private static ControlLimitsPads CreateControlLimits(SpacePads lotaggdocument, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var dataList = new Data1ListPads();
            foreach (var datalist in lotaggdocument.Data1List)
            {
                foreach (var checkparams in checkValueList)
                {
                    if (datalist.ParameterName == checkparams.ParameterName && datalist.ChannelId == checkparams.ChannelId)
                    {
                        dataList = datalist;
                    }
                }
            }
            var lotCtrlLimits = dataList.DataFlatLimits.ControlLimits;
            var controlLimits = new ControlLimitsPads();
            BaseWaferAggregation.InitCreateControlLimits(controlLimits, lotCtrlLimits);
            return controlLimits;
        }
        /// <summary>
        /// Create Measurement aggregates by calculating from the raw values.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="channelId"></param>
        /// <param name="checkValueList"></param>
        /// <returns>MeasurementAggregatesPads</returns>
        internal static MeasurementAggregatesPads CreateMeasurementAggregates(string parameter, string channelId, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var rawValues = checkValueList.Where(it => it.ParameterName == parameter && it.ChannelId == channelId).ToList();
            var measurementRawValues = rawValues
                .Where(it => it.IsFlagged == "N").ToList();
            var measurementAggregates = CreateMeasurementAggregates(rawValues.Count, measurementRawValues, rawValues);
            LotAggregation.AddGofAggregates(measurementAggregates, measurementRawValues);
            return measurementAggregates;
        }

        /// <summary>
        /// Create spec limits for a new wafer aggregates document.
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkValueList"></param>
        /// <returns>MeasurementSpecLimitsPads</returns>
        private static MeasurementSpecLimitsPads CreateMeasurementSpecLimits(SpacePads lotaggdocument, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var dataList = new Data1ListPads();
            foreach (var datalist in lotaggdocument.Data1List)
            {
                foreach (var checkparams in checkValueList)
                {
                    if (datalist.ParameterName == checkparams.ParameterName && datalist.ChannelId == checkparams.ChannelId)
                    {
                        dataList = datalist;
                    }
                }
            }
            var lotSpecLimits = dataList.DataFlatLimits.MeasurementSpecLimits;
            var measurementSpecLimits = new MeasurementSpecLimitsPads();
            BaseWaferAggregation.InitCreateSpecLimits(measurementSpecLimits, lotSpecLimits);
            return measurementSpecLimits;
        }
        /// <summary>
        /// Update the strange data in an existing wafer aggregation document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkdocument"></param>
        /// <returns>StrangeDataFlatMetaDataPads object</returns>
        private static StrangeDataFlatMetaDataPads UpdateStrangeMetaData(SpacePads lotaggdocument, SpacePads checkdocument)
        {
            var lotMetaData = lotaggdocument.StrangeDataFlatMetaData;
            var checMetaData = checkdocument.StrangeDataFlatMetaData;
            var dataFlatStrangeMetaData = new StrangeDataFlatMetaDataPads();
            BaseWaferAggregation.InitUpdateStrangeMetaData(dataFlatStrangeMetaData, lotMetaData, checMetaData);
            dataFlatStrangeMetaData.ProcessGroup = SpaceAggregationUtils.JoinStrings(checMetaData.ProcessGroup, lotMetaData.ProcessGroup);
            dataFlatStrangeMetaData.Design = SpaceAggregationUtils.JoinStrings(checMetaData.Design, lotMetaData.Design);
            dataFlatStrangeMetaData.Layer = SpaceAggregationUtils.JoinStrings(checMetaData.Layer, lotMetaData.Layer);
            dataFlatStrangeMetaData.ProcessLine = SpaceAggregationUtils.JoinStrings(checMetaData.ProcessLine, lotMetaData.ProcessLine);
            dataFlatStrangeMetaData.MeasurementBatch = SpaceAggregationUtils.JoinStrings(checMetaData.MeasurementBatch, lotMetaData.MeasurementBatch);
            return dataFlatStrangeMetaData;
        }
        /// <summary>
        /// Update the data flat meta data for an already existing wafer document.
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkdocument"></param>
        /// <param name="checkValueList"></param>
        /// <returns>DataFlatMetaDataPads object</returns>
        private static DataFlatMetaDataPads UpdateDataFlatMetaData(SpacePads lotaggdocument, SpacePads checkdocument, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            string slot = "";
            string waferSequence = "";
            string testPosition = "";
            var listSampleTimestamps = new List<DateTime>();
            var listSamplesTimestampsUtc = new List<DateTime>();
            var lotrawvaluesList = new List<Data1ListRawValuesPads>();
            foreach (var datalist in lotaggdocument.Data1List)
            {
                foreach (var checkparams in checkValueList)
                {
                    if (datalist.ParameterName == checkparams.ParameterName && checkparams.ChannelId == datalist.ChannelId)
                    {
                        lotrawvaluesList = datalist.Data1ListRawValues;
                    }
                }
            }
            foreach (var rawvalues in lotrawvaluesList)
            {
                if (rawvalues.ItemIdMotherlotWafer != null && rawvalues.ItemIdMotherlotWafer.Split(":").Last() == checkdocument.DataFlatMetaData.Wafer)
                {
                    slot = rawvalues.Slot;
                    waferSequence = rawvalues.WaferSequence;
                    testPosition = rawvalues.TestPosition;
                    listSampleTimestamps.Add(rawvalues.SampleTimestamp);
                    listSamplesTimestampsUtc.Add(rawvalues.SampleTimestampUtc);
                }
            }
            var lotMetaData = lotaggdocument.DataFlatMetaData;
            var checMetaData = checkdocument.DataFlatMetaData;
            var dataFlatMetaData = new DataFlatMetaDataPads();
            BaseWaferAggregation.InitUpdateDataFlatMetaData(dataFlatMetaData, lotMetaData, checMetaData);
            dataFlatMetaData.Motherlot = lotMetaData.MeasLot.Substring(0, 8);
            dataFlatMetaData.Wafer = checMetaData.Wafer;
            dataFlatMetaData.BeginTimestampUtc = listSamplesTimestampsUtc.Count > 0 ? SpaceAggregationUtils.MinDate(checMetaData.BeginTimestampUtc, listSamplesTimestampsUtc.Min()) : DateTime.MinValue;
            dataFlatMetaData.BeginTimestamp = listSampleTimestamps.Count > 0 ? SpaceAggregationUtils.MinDate(checMetaData.BeginTimestamp, listSampleTimestamps.Min()) : DateTime.MinValue;
            dataFlatMetaData.EndTimestampUtc = listSamplesTimestampsUtc.Count > 0 ? SpaceAggregationUtils.MaxDate(checMetaData.EndTimestampUtc, listSamplesTimestampsUtc.Max()) : DateTime.MinValue;
            dataFlatMetaData.EndTimestamp = listSampleTimestamps.Count > 0 ? SpaceAggregationUtils.MaxDate(checMetaData.EndTimestamp, listSampleTimestamps.Max()) : DateTime.MinValue;
            dataFlatMetaData.EquipmentType = SpaceAggregationUtils.JoinStrings(checMetaData.EquipmentType, lotMetaData.EquipmentType);
            dataFlatMetaData.Slot = SpaceAggregationUtils.JoinStrings(checMetaData.Slot, slot);
            dataFlatMetaData.TestPosition = SpaceAggregationUtils.JoinStrings(checMetaData.TestPosition, testPosition);
            dataFlatMetaData.WaferSequence = SpaceAggregationUtils.JoinStrings(checMetaData.WaferSequence, waferSequence);
            dataFlatMetaData.SPSNumber = SpaceAggregationUtils.JoinStrings(checMetaData.SPSNumber, lotMetaData.SPSNumber);
            dataFlatMetaData.SubOperation = SpaceAggregationUtils.JoinStrings(checMetaData.SPSNumber, lotMetaData.SPSNumber);
            dataFlatMetaData.Recipe = SpaceAggregationUtils.JoinStrings(checMetaData.Recipe, lotMetaData.Recipe);
            return dataFlatMetaData;
        }
    }
}
