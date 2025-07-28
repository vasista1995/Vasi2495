using System;
using System.Linq;
using System.Collections.Generic;
using PDS.SpaceBE.RBG.PADS.Module.Data.PADSModel;
using PDS.Space.Common.Aggregations;
using PDS.Space.Common.Data.PADSModel;
using PDS.SpaceBE.RBG.Common.Data.E4AModel;
using PDS.Queue.Api.Message;
using System.Diagnostics.CodeAnalysis;
using PDS.Space.Common;

namespace PDS.SpaceBE.RBG.PADS.Module.Aggregations
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
        /// Creating Wafer Aggregation to load to MongoDB
        /// </summary>
        /// <param name="wafAggId"></param>
        /// <param name="wafer"></param>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkValueList"></param>
        public SpacePads CreateNew(string wafAggId, string wafer, SpacePads lotaggdocument, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var spacePADS = CreateCommon(lotaggdocument, wafer);
            spacePADS.DataFlatMetaData = CreateDataFlatMetaData(lotaggdocument, wafer, checkValueList);
            spacePADS.StrangeDataFlatMetaData = CreateStrangeMetaData(lotaggdocument);
            spacePADS.Data1List = CreateData1List(lotaggdocument, wafer, checkValueList);
            spacePADS.SystemLog = CreateSystemLog();
            spacePADS.SearchPatterns = CreateSearchPattern(wafAggId, spacePADS.DataFlatMetaData.SiteKey);
            return spacePADS;
        }

        private static SearchPatternsPads CreateSearchPattern(string id, string siteKey)
        {
            return new SearchPatternsPads()
            {
                SpaceKey = id,
                TimeGroup = "0",
                SiteKey = siteKey
            };
        }
        /// <summary>
        /// Creating data1list subdocument
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="wafer"></param>
        /// <param name="checkValueList"></param>
        private static List<Data1ListPads> CreateData1List(SpacePads lotaggdocument, string wafer, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var dataList = new List<Data1ListPads>();
            var parameters = new List<string>();
            foreach (var values in checkValueList)
            {
                if (values.WaferLot.Split(":").Last() == wafer)
                {
                    parameters.Add(values.ParameterName);
                }
            }
            foreach (var lotDataList in lotaggdocument.Data1List)
            {
                var data1List = new Data1ListPads();
                if (parameters.Contains(lotDataList.ParameterName))
                {
                    BaseWaferAggregation.InitCreateData1List(data1List, lotDataList);
                    data1List.CreatedTimestamp = lotDataList.CreatedTimestamp;
                    data1List.CreatedTimestampUtc = lotDataList.CreatedTimestampUtc;
                    data1List.UpdatedTimestamp = lotDataList.UpdatedTimestamp;
                    data1List.UpdatedTimestampUtc = lotDataList.UpdatedTimestampUtc;
                    data1List.DataFlatLimits = CreateDataFlatLimits(lotaggdocument, checkValueList);
                    if (lotDataList.RvStoreFlag == "Y")
                    {
                        data1List.MeasurementAggregates = CreateMeasurementAggregates(lotDataList.ParameterName, wafer, checkValueList);
                    }
                    else if (lotDataList.RvStoreFlag == "N")
                    {
                        data1List.MeasurementAggregates = LoadMeasurementAggregates(lotDataList.ParameterName, lotDataList.Data1ListRawValues, checkValueList);
                    }
                    dataList.Add(data1List);
                }
            }
            return dataList;
        }
        /// <summary>
        /// updating the exisiting wafer document
        /// </summary>
        /// <param name="wafAggId"></param>
        /// <param name="wafer"></param>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkdocument"></param>
        /// <param name="checkValueList"></param>
        public SpacePads UpdateExisting(string wafAggId, string wafer, SpacePads lotaggdocument, SpacePads checkdocument, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var spacePADS = CreateCommon(lotaggdocument, wafer);
            spacePADS.Id = checkdocument.Id;
            spacePADS.DataFlatMetaData = UpdateDataFlatMetaData(lotaggdocument, checkdocument, checkValueList);
            spacePADS.StrangeDataFlatMetaData = UpdateStrangeMetaData(lotaggdocument, checkdocument);
            spacePADS.Data1List = UpdateData1List(lotaggdocument, checkdocument, checkValueList, wafer);
            spacePADS.SystemLog = UpdateSystemLog(checkdocument.SystemLog);
            spacePADS.SearchPatterns = CreateSearchPattern(wafAggId, spacePADS.DataFlatMetaData.SiteKey);
            return spacePADS;
        }
        /// <summary>
        /// updating the data1list in an exisitng wafer document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkdocument"></param>
        /// <param name="checkValueList"></param>
        /// <param name="wafer"></param>
        private static List<Data1ListPads> UpdateData1List(SpacePads lotaggdocument, SpacePads checkdocument, List<Data1ListRawValuesPads4Wafer> checkValueList, string wafer)
        {
            var dataList = new List<Data1ListPads>();
            var checkedParams = checkdocument.Data1List.ConvertAll(it => it.ParameterName);
            var checkedchids = checkdocument.Data1List.ConvertAll(it => it.ChannelId);
            foreach (var lotparamvalues in lotaggdocument.Data1List)
            {
                if (checkdocument.DataFlatMetaData.Wafer == wafer && (!checkedParams.Contains(lotparamvalues.ParameterName) ||
                    !checkedchids.Contains(lotparamvalues.ChannelId)))
                {
                    UpdateData1ListNew(lotaggdocument, checkdocument, checkValueList, wafer, dataList, lotparamvalues);
                }
            }
            foreach (var checkparams in checkdocument.Data1List)
            {
                if (wafer == checkdocument.DataFlatMetaData.Wafer)
                {
                    UpdateData1ListExisting(dataList, checkparams);
                }
            }
            return dataList;
        }

        internal static void UpdateData1ListExisting(List<Data1ListPads> dataList, Data1ListPads checkparams)
        {
            var otherparams = new Data1ListPads();
            bool isEqual = false;
            foreach (var dataparam in dataList)
            {
                if (checkparams.ParameterName == dataparam.ParameterName)
                    isEqual = true;
            }
            if (!isEqual)
            {
                BaseWaferAggregation.InitCreateData1List(otherparams, checkparams);
                otherparams.CreatedTimestamp = checkparams.CreatedTimestamp;
                otherparams.CreatedTimestampUtc = checkparams.CreatedTimestampUtc;
                otherparams.UpdatedTimestamp = checkparams.UpdatedTimestamp;
                otherparams.UpdatedTimestampUtc = checkparams.UpdatedTimestampUtc;
                otherparams.DataFlatLimits = checkparams.DataFlatLimits;
                otherparams.MeasurementAggregates = checkparams.MeasurementAggregates;
            }
            if (otherparams.ParameterName != null)
            {
                dataList.Add(otherparams);
            }
        }

        private static void UpdateData1ListNew(SpacePads lotaggdocument, SpacePads checkdocument, List<Data1ListRawValuesPads4Wafer> checkValueList, string wafer, List<Data1ListPads> dataList, Data1ListPads lotparamvalues)
        {
            var data1List = new Data1ListPads();
            bool isEqual = false;
            foreach (var checkparams in checkdocument.Data1List)
            {
                if (lotparamvalues.ParameterName == checkparams.ParameterName)
                {
                    isEqual = true;
                }
            }
            if (!isEqual)
            {
                BaseWaferAggregation.InitCreateData1List(data1List, lotparamvalues);
                data1List.CreatedTimestamp = lotparamvalues.CreatedTimestamp;
                data1List.CreatedTimestampUtc = lotparamvalues.CreatedTimestampUtc;
                data1List.UpdatedTimestamp = lotparamvalues.UpdatedTimestamp;
                data1List.UpdatedTimestampUtc = lotparamvalues.UpdatedTimestampUtc;
                data1List.DataFlatLimits = CreateDataFlatLimits(lotaggdocument, checkValueList);
                if (lotparamvalues.RvStoreFlag == "Y")
                {
                    data1List.MeasurementAggregates = CreateMeasurementAggregates(lotparamvalues.ParameterName, wafer, checkValueList);
                }
                else if (lotparamvalues.RvStoreFlag == "N")
                {
                    data1List.MeasurementAggregates = LoadMeasurementAggregates(lotparamvalues.ParameterName, lotparamvalues.Data1ListRawValues, checkValueList);
                }
            }
            if (data1List.ParameterName != null)
            {
                dataList.Add(data1List);
            }
        }

        /// <summary>
        /// Load the space aggregates when there are no raw values available.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="rawvalues"></param>
        /// <param name="checkValueList"></param>
        internal static MeasurementAggregatesPads LoadMeasurementAggregates(string parameterName, List<Data1ListRawValuesPads> rawvalues, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            int baseCount = 0;
            int flaggedCount = 0;
            var weightedmeanvals = new List<double>();
            var minvals = new List<double>();
            var maxvals = new List<double>();
            var samples = new List<long>();
            foreach (var spcaggs in rawvalues)
            {
                foreach (var checkValues in checkValueList)
                {
                    if (spcaggs.WaferLot == checkValues.WaferLot && parameterName == checkValues.ParameterName)
                    {
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
        /// <summary>
        /// Creating all the common fields for both creating and updating the wafer documents
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="wafer"></param>
        private static SpacePads CreateCommon(SpacePads lotaggdocument, string wafer)
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
            return new SpacePads()
            {
                Document = document,
                ProductionAction = productionAction,
                Item = item
            };
        }
        /// <summary>
        /// creating the strangedataflatmetadata fields
        /// </summary>
        /// <param name="lotaggdocument"></param>
        private static StrangeDataFlatMetaDataPads CreateStrangeMetaData(SpacePads lotaggdocument)
        {
            var lotMetaData = lotaggdocument.StrangeDataFlatMetaData;
            var dataFlatStrangeMetaData = new StrangeDataFlatMetaDataPads();
            BaseWaferAggregation.InitCreateStrangeData(dataFlatStrangeMetaData, lotMetaData);
            dataFlatStrangeMetaData.PackageGroup = lotMetaData.PackageGroup;
            dataFlatStrangeMetaData.EquipmentType = lotMetaData.EquipmentType;
            dataFlatStrangeMetaData.Wire = lotMetaData.Wire;
            dataFlatStrangeMetaData.ManufacturingWipLevel = lotMetaData.ManufacturingWipLevel;
            dataFlatStrangeMetaData.BeSegmentName = lotMetaData.BeSegmentName;
            dataFlatStrangeMetaData.BeSort = lotMetaData.BeSort;
            dataFlatStrangeMetaData.Data1 = lotMetaData.Data1;
            dataFlatStrangeMetaData.Data2 = lotMetaData.Data2;
            dataFlatStrangeMetaData.Data3 = lotMetaData.Data3;
            dataFlatStrangeMetaData.Device = lotMetaData.Device;
            dataFlatStrangeMetaData.DeviceFamily = lotMetaData.DeviceFamily;
            dataFlatStrangeMetaData.ErrorCode = lotMetaData.ErrorCode;
            dataFlatStrangeMetaData.Group1 = lotMetaData.Group1;
            dataFlatStrangeMetaData.Group2 = lotMetaData.Group2;
            dataFlatStrangeMetaData.Group3 = lotMetaData.Group3;
            dataFlatStrangeMetaData.GroupId = lotMetaData.GroupId;
            dataFlatStrangeMetaData.SalesName = lotMetaData.SalesName;
            dataFlatStrangeMetaData.SampleType = lotMetaData.SampleType;
            dataFlatStrangeMetaData.Segment = lotMetaData.Segment;
            dataFlatStrangeMetaData.Module = lotMetaData.Module;
            dataFlatStrangeMetaData.Owner = lotMetaData.Owner;
            dataFlatStrangeMetaData.Package = lotMetaData.Package;
            dataFlatStrangeMetaData.PackageClass = lotMetaData.PackageClass;
            dataFlatStrangeMetaData.OriginSampleSize = lotMetaData.OriginSampleSize;
            dataFlatStrangeMetaData.Pin = lotMetaData.Pin;
            dataFlatStrangeMetaData.ProductName = lotMetaData.ProductName;
            dataFlatStrangeMetaData.UserClass1 = lotMetaData.UserClass1;
            dataFlatStrangeMetaData.UserClass2 = lotMetaData.UserClass2;
            dataFlatStrangeMetaData.UserClass3 = lotMetaData.UserClass3;
            dataFlatStrangeMetaData.PackageFamily = lotMetaData.PackageFamily;
            return dataFlatStrangeMetaData;
        }
        /// <summary>
        /// create dataflatmetadata fields in a new wafer document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="wafer"></param>
        /// <param name="checkValueList"></param>
        private static DataFlatMetaDataPads CreateDataFlatMetaData(SpacePads lotaggdocument, string wafer, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var listSampleTimestamps = new List<DateTime>();
            var listSampleTimestampsUtc = new List<DateTime>();
            var rawvaluesList = new List<Data1ListRawValuesPads>();
            foreach (var datalist in lotaggdocument.Data1List)
            {
                foreach (var checkparams in checkValueList)
                {
                    if (datalist.ParameterName == checkparams.ParameterName)
                    {
                        rawvaluesList = datalist.Data1ListRawValues;
                    }
                }
            }
            foreach (var rawvalues in rawvaluesList)
            {
                if (int.Parse(rawvalues.WaferLot.Split('.').Last()).ToString() == wafer)
                {
                    listSampleTimestamps.Add(rawvalues.SampleTimestamp);
                    listSampleTimestampsUtc.Add(rawvalues.SampleTimestampUtc);
                }
            }
            var lotMetaData = lotaggdocument.DataFlatMetaData;
            var dataFlatMetaData = new DataFlatMetaDataPads();
            BaseWaferAggregation.InitCreateDataFlatMetaData(dataFlatMetaData, lotMetaData);
            dataFlatMetaData.Motherlot = lotMetaData.MeasLot.Substring(0, 8);
            dataFlatMetaData.Wafer = int.Parse(wafer).ToString();
            dataFlatMetaData.BeginTimestampUtc = listSampleTimestampsUtc.Min();
            dataFlatMetaData.BeginTimestamp = listSampleTimestamps.Min();
            dataFlatMetaData.EndTimestampUtc = listSampleTimestampsUtc.Max();
            dataFlatMetaData.EndTimestamp = listSampleTimestamps.Max();
            dataFlatMetaData.SpecName = lotMetaData.SpecName;
            dataFlatMetaData.Material = lotMetaData.Material;
            dataFlatMetaData.OperatorId = lotMetaData.OperatorId;
            return dataFlatMetaData;
        }
        /// <summary>
        /// To create dataflatlimits fields in the new document
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
                    if (datalist.ParameterName == checkparams.ParameterName)
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
        /// To create control limits fields in a new wafer document
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
                    if (datalist.ParameterName == checkparams.ParameterName)
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
        /// To create measurementaggregates in a new wafer document
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="wafer"></param>
        /// <param name="checkValueList"></param>
        internal static MeasurementAggregatesPads CreateMeasurementAggregates(string parameter, string wafer, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var rawValues = checkValueList.Where(it => it.ParameterName == parameter).ToList();
            var measurementRawValues = rawValues.Where(it => it.IsFlagged == "N" && it.WaferLot == wafer);
            return CreateMeasurementAggregates(rawValues.Count, measurementRawValues, measurementRawValues);
        }
        /// <summary>
        /// To create speclimits in a new wafer document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkValueList"></param>
        private static MeasurementSpecLimitsPads CreateMeasurementSpecLimits(SpacePads lotaggdocument, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var dataList = new Data1ListPads();
            foreach (var datalist in lotaggdocument.Data1List)
            {
                foreach (var checkparams in checkValueList)
                {
                    if (datalist.ParameterName == checkparams.ParameterName)
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
        /// To update exisiting strangedataflatmetadata fields in an exisiting wafer document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkdocument"></param>
        private static StrangeDataFlatMetaDataPads UpdateStrangeMetaData(SpacePads lotaggdocument, SpacePads checkdocument)
        {
            var lotMetaData = lotaggdocument.StrangeDataFlatMetaData;
            var checkMetaData = checkdocument.StrangeDataFlatMetaData;
            var dataFlatStrangeMetaData = new StrangeDataFlatMetaDataPads();
            BaseWaferAggregation.InitUpdateStrangeMetaData(dataFlatStrangeMetaData, lotMetaData, checkMetaData);
            dataFlatStrangeMetaData.SalesName = SpaceAggregationUtils.JoinStrings(checkMetaData.SalesName, lotMetaData.SalesName);
            dataFlatStrangeMetaData.SampleType = SpaceAggregationUtils.JoinStrings(checkMetaData.SampleType, lotMetaData.SampleType);
            dataFlatStrangeMetaData.BeSegmentName = SpaceAggregationUtils.JoinStrings(checkMetaData.BeSegmentName, lotMetaData.BeSegmentName);
            dataFlatStrangeMetaData.BeSort = SpaceAggregationUtils.JoinStrings(checkMetaData.BeSort, lotMetaData.BeSort);
            dataFlatStrangeMetaData.Data1 = SpaceAggregationUtils.JoinStrings(checkMetaData.Data1, lotMetaData.Data1);
            dataFlatStrangeMetaData.Data2 = SpaceAggregationUtils.JoinStrings(checkMetaData.Data2, lotMetaData.Data2);
            dataFlatStrangeMetaData.Data3 = SpaceAggregationUtils.JoinStrings(checkMetaData.Data3, lotMetaData.Data3);
            dataFlatStrangeMetaData.Group1 = SpaceAggregationUtils.JoinStrings(checkMetaData.Group1, lotMetaData.Group1);
            dataFlatStrangeMetaData.Group2 = SpaceAggregationUtils.JoinStrings(checkMetaData.Group2, lotMetaData.Group2);
            dataFlatStrangeMetaData.Group3 = SpaceAggregationUtils.JoinStrings(checkMetaData.Group3, lotMetaData.Group3);
            dataFlatStrangeMetaData.Device = SpaceAggregationUtils.JoinStrings(checkMetaData.Device, lotMetaData.Device);
            dataFlatStrangeMetaData.UserClass1 = SpaceAggregationUtils.JoinStrings(checkMetaData.UserClass1, lotMetaData.UserClass1);
            dataFlatStrangeMetaData.UserClass2 = SpaceAggregationUtils.JoinStrings(checkMetaData.UserClass2, lotMetaData.UserClass2);
            dataFlatStrangeMetaData.UserClass3 = SpaceAggregationUtils.JoinStrings(checkMetaData.UserClass3, lotMetaData.UserClass3);
            dataFlatStrangeMetaData.EquipmentType = SpaceAggregationUtils.JoinStrings(checkMetaData.EquipmentType, lotMetaData.EquipmentType);
            dataFlatStrangeMetaData.ErrorCode = SpaceAggregationUtils.JoinStrings(checkMetaData.ErrorCode, lotMetaData.ErrorCode);
            dataFlatStrangeMetaData.DeviceFamily = SpaceAggregationUtils.JoinStrings(checkMetaData.DeviceFamily, lotMetaData.DeviceFamily);
            dataFlatStrangeMetaData.GroupId = SpaceAggregationUtils.JoinStrings(checkMetaData.GroupId, lotMetaData.GroupId);
            dataFlatStrangeMetaData.Segment = SpaceAggregationUtils.JoinStrings(checkMetaData.Segment, lotMetaData.Segment);
            dataFlatStrangeMetaData.ManufacturingWipLevel = SpaceAggregationUtils.JoinStrings(checkMetaData.ManufacturingWipLevel, lotMetaData.ManufacturingWipLevel);
            dataFlatStrangeMetaData.OriginSampleSize = SpaceAggregationUtils.JoinStrings(checkMetaData.OriginSampleSize, lotMetaData.OriginSampleSize);
            dataFlatStrangeMetaData.Module = SpaceAggregationUtils.JoinStrings(checkMetaData.Module, lotMetaData.Module);
            dataFlatStrangeMetaData.ProductName = SpaceAggregationUtils.JoinStrings(checkMetaData.ProductName, lotMetaData.ProductName);
            dataFlatStrangeMetaData.Package = SpaceAggregationUtils.JoinStrings(checkMetaData.Package, lotMetaData.Package);
            dataFlatStrangeMetaData.PackageClass = SpaceAggregationUtils.JoinStrings(checkMetaData.PackageClass, lotMetaData.PackageClass);
            dataFlatStrangeMetaData.PackageFamily = SpaceAggregationUtils.JoinStrings(checkMetaData.PackageFamily, lotMetaData.PackageFamily);
            dataFlatStrangeMetaData.PackageGroup = SpaceAggregationUtils.JoinStrings(checkMetaData.PackageGroup, lotMetaData.PackageGroup);
            dataFlatStrangeMetaData.Owner = SpaceAggregationUtils.JoinStrings(checkMetaData.Owner, lotMetaData.Owner);
            dataFlatStrangeMetaData.Pin = SpaceAggregationUtils.JoinStrings(checkMetaData.Pin, lotMetaData.Pin);
            dataFlatStrangeMetaData.Wire = SpaceAggregationUtils.JoinStrings(checkMetaData.Wire, lotMetaData.Wire);
            return dataFlatStrangeMetaData;
        }
        /// <summary>
        /// To update the dataflatmetadata fields in an existing wafer document
        /// </summary>
        /// <param name="lotaggdocument"></param>
        /// <param name="checkdocument"></param>
        /// <param name="checkValueList"></param>
        private static DataFlatMetaDataPads UpdateDataFlatMetaData(SpacePads lotaggdocument, SpacePads checkdocument, List<Data1ListRawValuesPads4Wafer> checkValueList)
        {
            var listSampleTimestamps = new List<DateTime>();
            var listSamplesTimestampsUtc = new List<DateTime>();
            var lotrawvaluesList = new List<Data1ListRawValuesPads>();
            foreach (var datalist in lotaggdocument.Data1List)
            {
                foreach (var checkparams in checkValueList)
                {
                    if (datalist.ParameterName == checkparams.ParameterName)
                    {
                        lotrawvaluesList = datalist.Data1ListRawValues;
                    }
                }
            }
            foreach (var rawvalues in lotrawvaluesList)
            {
                if (int.Parse(rawvalues.WaferLot.Split('.').Last()).ToString() == checkdocument.DataFlatMetaData.Wafer)
                {
                    listSampleTimestamps.Add(rawvalues.SampleTimestamp);
                    listSamplesTimestampsUtc.Add(rawvalues.SampleTimestampUtc);
                }
            }
            var lotMetaData = lotaggdocument.DataFlatMetaData;
            var checkMetaData = checkdocument.DataFlatMetaData;
            var dataFlatMetaData = new DataFlatMetaDataPads();
            BaseWaferAggregation.InitUpdateDataFlatMetaData(dataFlatMetaData, lotMetaData, checkMetaData);
            dataFlatMetaData.Motherlot = checkMetaData.Motherlot;
            dataFlatMetaData.Wafer = checkMetaData.Wafer;
            dataFlatMetaData.SpecName = SpaceAggregationUtils.JoinStrings(checkMetaData.SpecName, lotMetaData.SpecName);
            dataFlatMetaData.BeginTimestampUtc = SpaceAggregationUtils.MinDate(checkMetaData.BeginTimestampUtc, listSamplesTimestampsUtc.Min());
            dataFlatMetaData.BeginTimestamp = SpaceAggregationUtils.MinDate(checkMetaData.BeginTimestamp, listSampleTimestamps.Min());
            dataFlatMetaData.EndTimestampUtc = SpaceAggregationUtils.MaxDate(checkMetaData.EndTimestampUtc, listSamplesTimestampsUtc.Max());
            dataFlatMetaData.EndTimestamp = SpaceAggregationUtils.MaxDate(checkMetaData.EndTimestamp, listSampleTimestamps.Max());
            dataFlatMetaData.Material = SpaceAggregationUtils.JoinStrings(checkMetaData.Material, lotMetaData.Material);
            dataFlatMetaData.OperatorId = SpaceAggregationUtils.JoinStrings(checkMetaData.OperatorId, lotMetaData.OperatorId);
            return dataFlatMetaData;
        }
    }
}
