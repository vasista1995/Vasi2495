using System.Linq;
using System.Collections.Generic;
using MongoDB.Driver;
using PDS.SpaceBE.RBG.Common.Data.E4AModel;
using PDS.SpaceBE.RBG.PADS.Module.Data.PADSModel;
using PDS.Space.Common.Aggregations;
using PDS.Space.Common.Data.PADSModel;
using PDS.Queue.Api.Message;
using System.Diagnostics.CodeAnalysis;
using PDS.Space.Common;
using System;

namespace PDS.SpaceBE.RBG.PADS.Module.Aggregations
{
    /// <summary>
    /// This Lot Class generates lot aggregates for each e4a document that is pulled from the file system.
    /// Each incoming e4a document will result in either creating a new pads document or updating already existing pads document.
    /// </summary>
    public class LotAggregation : BaseAggregation
    {
        public LotAggregation([NotNull] SpaceE4A e4aDocument, [NotNull] IQueueMessage message) : base(e4aDocument, message)
        {
            E4aDocument = e4aDocument;

            AggregationId = GetLotAggregationId(e4aDocument);
            ProductionActionId = GetProductionActionId(e4aDocument);
        }
        public new SpaceE4A E4aDocument { get; }
        public string AggregationId { get; }
        public string ProductionActionId { get; }
        /// <summary>
        /// create new pads document from the e4a doucment
        /// </summary>
        /// <param name="idsList">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns name="spacePADS">spacePADS is the pads document that contains all the lot aggregations and is returned to SpaceQueueReader.cs to be inserted into MongoDB</returns>
        public SpacePads CreateNew()
        {
            var spacePADS = CreateCommon(E4aDocument);
            spacePADS.DataFlatMetaData = CreateDataFlatMetaData(E4aDocument);
            spacePADS.StrangeDataFlatMetaData = CreateStrangeMetaData(E4aDocument);
            spacePADS.Data1List = CreateData1list(E4aDocument);
            spacePADS.SearchPatterns = CreateSearchPattern(spacePADS.DataFlatMetaData.SiteKey);
            spacePADS.SystemLog = CreateSystemLog();
            return spacePADS;
        }
        private SearchPatternsPads CreateSearchPattern(string siteKey)
        {
            return new SearchPatternsPads()
            {
                SpaceKey = AggregationId,
                TimeGroup = "0",
                SiteKey = siteKey
            };
        }
        /// <summary>
        /// To update the existing lot document
        /// </summary>
        /// <param name="checkdocument"></param>
        public SpacePads UpdateExisting(SpacePads checkdocument)
        {
            var spacePADS = CreateCommon(E4aDocument);
            spacePADS.Id = checkdocument.Id;
            spacePADS.DataFlatMetaData = UpdateDataFlatMetaData(E4aDocument, checkdocument);
            spacePADS.StrangeDataFlatMetaData = UpdateDataFlatStrangeMetaData(E4aDocument, checkdocument);
            var e4aMetaData = E4aDocument.DataFlatMetaData;
            var checkDataList = new Data1ListPads();
            var data1List = new List<Data1ListPads>();
            foreach (var paramvalues in checkdocument.Data1List)
            {
                if (paramvalues.ParameterName == e4aMetaData.ParameterName && paramvalues.ChannelId == e4aMetaData.ChannelId)
                {
                    checkDataList = paramvalues;
                }
                else
                {
                    data1List.Add(paramvalues);
                }
            }

            if (checkDataList.ParameterName is null)
            {
                data1List.Add(UpdateNewData1list(E4aDocument));
            }
            else
            {
                data1List.Add(UpdateOldData1list(E4aDocument, checkDataList));
            }
            spacePADS.Data1List = data1List;
            spacePADS.SystemLog = UpdateSystemLog(checkdocument.SystemLog);
            spacePADS.SearchPatterns = CreateSearchPattern(spacePADS.DataFlatMetaData.SiteKey);
            return spacePADS;
        }
        /// <summary>
        /// To create all the common fields for both creating and updating the lot document
        /// </summary>
        /// <param name="e4adocument"></param>
        /// <returns></returns>
        private SpacePads CreateCommon(SpaceE4A e4adocument)
        {
            var repetition = new RepetitionPads()
            {
                Id = "0",
                IdBaseValue = e4adocument.DataFlatMetaData.UpdatedTimestampUtc
            };
            var document = new DocumentPads()
            {
                Type = "ProcessControl",
                Version = "1.0",
                Repetition = repetition
            };
            var productionAction = new ProductionActionPads()
            {
                Id = ProductionActionId,
                Type = "SpaceAggregation",
            };
            var item = new ItemPads()
            {
                Id = e4adocument.Item.Id,
                IdSystemName = e4adocument.Item.IdSystemName,
                Type = e4adocument.Item.Type,
            };
            return new SpacePads()
            {
                Document = document,
                ProductionAction = productionAction,
                Item = item
            };
        }
        /// <summary>
        /// To create strangemetadata ina new lot document
        /// </summary>
        /// <param name="e4adocument"></param>
        /// <returns></returns>
        private static StrangeDataFlatMetaDataPads CreateStrangeMetaData(SpaceE4A e4adocument)
        {
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var dataFlatStrangeMetaData = new StrangeDataFlatMetaDataPads();
            BaseLotAggregation.InitCreateStrangeData(dataFlatStrangeMetaData, e4aMetaData);

            dataFlatStrangeMetaData.Package = e4aMetaData.Package;
            dataFlatStrangeMetaData.PackageClass = e4aMetaData.PackageClass;
            dataFlatStrangeMetaData.PackageFamily = e4aMetaData.PackageFamily;
            dataFlatStrangeMetaData.PackageGroup = e4aMetaData.PackageGroup;
            dataFlatStrangeMetaData.Pin = e4aMetaData.Pin;
            dataFlatStrangeMetaData.ProductName = e4aMetaData.ProductName;
            dataFlatStrangeMetaData.BeSegmentName = e4aMetaData.BeSegmentName;
            dataFlatStrangeMetaData.SalesName = e4aMetaData.SalesName;
            dataFlatStrangeMetaData.SampleType = e4aMetaData.SampleType;
            dataFlatStrangeMetaData.Segment = e4aMetaData.Segment;
            dataFlatStrangeMetaData.BeSort = e4aMetaData.BeSort;
            dataFlatStrangeMetaData.Data1 = e4aMetaData.Data1;
            dataFlatStrangeMetaData.Data2 = e4aMetaData.Data2;
            dataFlatStrangeMetaData.Data3 = e4aMetaData.Data3;
            dataFlatStrangeMetaData.Device = e4aMetaData.Device;
            dataFlatStrangeMetaData.Module = e4aMetaData.Module;
            dataFlatStrangeMetaData.DeviceFamily = e4aMetaData.DeviceFamily;
            dataFlatStrangeMetaData.ErrorCode = e4aMetaData.ErrorCode;
            dataFlatStrangeMetaData.Group1 = e4aMetaData.Group1;
            dataFlatStrangeMetaData.Group2 = e4aMetaData.Group2;
            dataFlatStrangeMetaData.Group3 = e4aMetaData.Group3;
            dataFlatStrangeMetaData.GroupId = e4aMetaData.GroupId;
            dataFlatStrangeMetaData.EquipmentType = e4aMetaData.EquipmentType;
            dataFlatStrangeMetaData.OriginSampleSize = e4aMetaData.OriginSampleSize;
            dataFlatStrangeMetaData.ManufacturingWipLevel = e4aMetaData.ManufacturingWipLevel;
            dataFlatStrangeMetaData.Owner = e4aMetaData.Owner;
            dataFlatStrangeMetaData.UserClass1 = e4aMetaData.UserClass1;
            dataFlatStrangeMetaData.UserClass2 = e4aMetaData.UserClass2;
            dataFlatStrangeMetaData.UserClass3 = e4aMetaData.UserClass3;
            dataFlatStrangeMetaData.Wire = e4aMetaData.Wire;
            return dataFlatStrangeMetaData;
        }
        /// <summary>
        /// Create DataFlatMetaData subdocument.
        /// </summary>
        /// <param name="e4aDocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns DataFlatMetadata subdocument to the parent method.</returns>
        private static DataFlatMetaDataPads CreateDataFlatMetaData(SpaceE4A e4aDocument)
        {
            var e4aMetaData = e4aDocument.DataFlatMetaData;
            var dataFlatMetaData = new DataFlatMetaDataPads();
            BaseLotAggregation.InitCreateDataFlatMetaData(dataFlatMetaData, e4aMetaData);
            dataFlatMetaData.SpecName = e4aMetaData.SpecName;
            dataFlatMetaData.Material = e4aMetaData.Material;
            dataFlatMetaData.OperatorId = e4aMetaData.OperatorId;
            return dataFlatMetaData;
        }
        /// <summary>
        /// Creates list of all the rawvalues from the child method.
        /// </summary>
        /// <param name="e4aDocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns the list to the parent method.</returns>
        private static List<Data1ListPads> CreateData1list(SpaceE4A e4aDocument)
        {
            var e4aMetaData = e4aDocument.DataFlatMetaData;
            var data1List = new List<Data1ListPads>();
            var dataList = new Data1ListPads();
            BaseLotAggregation.InitCreateDataList(dataList, e4aMetaData);
            dataList.DataFlatLimits = CreateDataFlatLimits(e4aDocument);
            dataList.Data1ListRawValues = CreateData1listRawValuesList(e4aDocument);
            if (dataList.RvStoreFlag != "N")
            {
                dataList.MeasurementAggregates = CreateMeasurementAggregates(dataList.Data1ListRawValues);
            }
            else
            {
                dataList.MeasurementAggregates = LoadMeasurementAggregates(dataList.Data1ListRawValues);
            }
            data1List.Add(dataList);
            return data1List;
        }
        /// <summary>
        /// To create rawvalues list
        /// </summary>
        /// <param name="e4aDocument"></param>
        /// <returns></returns>
        private static List<Data1ListRawValuesPads> CreateData1listRawValuesList(SpaceE4A e4aDocument)
        {
            var list = new List<Data1ListRawValuesPads>();
            foreach (var rawValues in e4aDocument.Data1List.Data1ListRawValues)
            {
                list.Add(CreatData1ListRawValues(rawValues, e4aDocument));
            }
            return list;
        }
        /// <summary>
        /// To create rawvalues
        /// </summary>
        /// <param name="e4aRawValues"></param>
        /// <param name="e4aDocument"></param>
        /// <returns></returns>
        internal static Data1ListRawValuesPads CreatData1ListRawValues(Data1ListRawValuesE4A e4aRawValues, SpaceE4A e4aDocument)
        {
            var spaceAggregates = e4aDocument.Data1List.DataFlatLimits.SpaceAggregates;
            var dataFlatMetaData = e4aDocument.DataFlatMetaData;
            var rawValues = new Data1ListRawValuesPads();
            BaseLotAggregation.InitCreateRawValues(rawValues, e4aRawValues, dataFlatMetaData, spaceAggregates);
            rawValues.WaferLot = e4aRawValues.WaferLot;
            rawValues.SampleSize = e4aDocument.Data1List.SampleSize;
            return rawValues;
        }
        /// <summary>
        /// Create limits from e4a document data and child methods
        /// </summary>
        /// <param name="e4adoc">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns subdocument to the pads document in the parent method</returns>
        private static DataFlatLimitsPads CreateDataFlatLimits(SpaceE4A e4adoc)
        {
            return new DataFlatLimitsPads()
            {
                CKCId = e4adoc.Data1List.DataFlatLimits.CkcId,
                MeasurementSpecLimits = CreateMeasurementSpecLimits(e4adoc),
                ControlLimits = CreateControlLimits(e4adoc),
            };
        }
        /// <summary>
        /// creates control limits for ckc_id 0
        /// </summary>
        /// <param name="e4aDocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns controllimits0 to the dataflatlimits subdocument</returns>
        private static ControlLimitsPads CreateControlLimits(SpaceE4A e4aDocument)
        {
            var e4aLimits = e4aDocument.Data1List.DataFlatLimits.ControlLimits;
            var controlLimits = new ControlLimitsPads();
            BaseLotAggregation.InitCreateControlLimits(controlLimits, e4aLimits);
            return controlLimits;
        }
        /// <summary>
        /// To load the measurement aggregates from space when
        /// rawvalus are not available for aggregate calculations
        /// </summary>
        /// <param name="rawvalues"></param>
        /// <returns></returns>
        internal static MeasurementAggregatesPads LoadMeasurementAggregates(List<Data1ListRawValuesPads> rawvalues)
        {
            var measurementAggregates = new MeasurementAggregatesPads();
            int baseCount = 0;
            int flaggedCount = 0;
            var weightedmeanvals = new List<double>();
            var minvals = new List<double>();
            var maxvals = new List<double>();
            var primaryViolations = new List<string>();
            var primaryViolComm = new List<string>();
            var violationList = new List<string>();
            var violationComments = new List<string>();
            var numViolations = new List<int>();
            var rawvaluesValid = rawvalues.Where(i => i.IsFlagged == "N").ToList();
            var samples = rawvaluesValid.Select(i => i.SampleId).Distinct().ToList();
            foreach (var rawvalue in rawvalues)
            {
                primaryViolations.Add(rawvalue.PrimaryViolation);
                primaryViolComm.Add(rawvalue.PrimaryViolationComments);
                violationComments.Add(rawvalue.ViolationComments);
                if (rawvalue.ViolationList != null)
                {
                    if (rawvalue.ViolationList.Contains(","))
                    {
                        violationList.AddRange(rawvalue.ViolationList.Split(","));
                    }
                    else
                    {
                        violationList.Add(rawvalue.ViolationList);
                    }
                }
                numViolations.Add(rawvalue.NumViolations);
                if (rawvalue.IsFlagged == "N")
                {
                    baseCount += rawvalue.SampleSize;
                    weightedmeanvals.Add(rawvalue.SampleMean * rawvalue.SampleSize);
                    minvals.Add(rawvalue.SampleMin);
                    maxvals.Add(rawvalue.SampleMax);
                }
                else
                {
                    flaggedCount += rawvalue.SampleSize;
                }
            }

            if (baseCount > 0)
            {
                measurementAggregates = new MeasurementAggregatesPads()
                {
                    BaseCount = baseCount,
                    FlaggedCount = flaggedCount,
                    ExecCount = baseCount + flaggedCount,
                    Mean = weightedmeanvals.Sum() / baseCount,
                    Max = maxvals.Max(),
                    Min = minvals.Min()
                };
                if (primaryViolations.Distinct().Any())
                    measurementAggregates.PrimaryViolation = string.Join(", ", primaryViolations.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                if (primaryViolComm.Distinct().Any())
                    measurementAggregates.PrimaryViolationComments = string.Join(", ", primaryViolComm.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                if (violationComments.Distinct().Any())
                    measurementAggregates.ViolationComments = string.Join(", ", violationComments.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                if (violationList.Distinct().Any())
                    measurementAggregates.ViolationList = string.Join(", ", violationList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                measurementAggregates.NumViolations = measurementAggregates.ViolationList is null ? 0 : measurementAggregates.ViolationList.Split(',').Count();
            }
            measurementAggregates.Samples = string.Join(", ", samples.Distinct());
            return measurementAggregates;
        }

        /// <summary>
        /// To calculate aggregates when the raw values are available
        /// </summary>
        /// <param name="rawValues"></param>
        /// <returns></returns>
        internal static MeasurementAggregatesPads CreateMeasurementAggregates(List<Data1ListRawValuesPads> rawValues)
        {
            var measurementRawValues = rawValues.Where(it => it.IsFlagged == "N" || it.IsFlagged == null).ToList();
            return CreateMeasurementAggregates(rawValues.Count, measurementRawValues, rawValues);
        }

        /// <summary>
        /// creates specification limits.
        /// </summary>
        /// <param name="e4aDocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns the specification limits to the MeasurementSpecLimits subdocument</returns>
        private static MeasurementSpecLimitsPads CreateMeasurementSpecLimits(SpaceE4A e4aDocument)
        {
            var e4aLimits = e4aDocument.Data1List.DataFlatLimits.MeasurementSpecLimits;
            var measurementSpecLimits = new MeasurementSpecLimitsPads();
            BaseLotAggregation.InitCreateSpecLimits(measurementSpecLimits, e4aLimits);
            return measurementSpecLimits;
        }
        /// <summary>
        /// To update the datalistparameters subdocument of an existing lot document
        /// </summary>
        /// <param name="e4aDocument"></param>
        /// <returns></returns>
        private static Data1ListPads UpdateNewData1list(SpaceE4A e4aDocument)
        {
            var e4aMetaData = e4aDocument.DataFlatMetaData;
            var data1List = new Data1ListPads();
            BaseLotAggregation.InitCreateDataList(data1List, e4aMetaData);
            data1List.Data1ListRawValues = CreateData1listRawValuesList(e4aDocument);
            data1List.DataFlatLimits = CreateDataFlatLimits(e4aDocument);
            if (data1List.RvStoreFlag == "Y")
            {
                data1List.MeasurementAggregates = CreateMeasurementAggregates(data1List.Data1ListRawValues);
            }
            else if (data1List.RvStoreFlag == "N")
            {
                data1List.MeasurementAggregates = LoadMeasurementAggregates(data1List.Data1ListRawValues);
            }
            return data1List;
        }
        /// <summary>
        /// To update the datalist parameters in existing lot document
        /// </summary>
        /// <param name="e4adocument"></param>
        /// <param name="checkDataList"></param>
        /// <returns></returns>
        private static Data1ListPads UpdateOldData1list(SpaceE4A e4adocument, Data1ListPads checkDataList)
        {
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var data1List = new Data1ListPads();
            BaseLotAggregation.InitUpdateDataList(data1List, e4aMetaData, checkDataList);
            data1List.Data1ListRawValues = UpdateData1ListRawValuesPADS(e4adocument, checkDataList);
            data1List.DataFlatLimits = UpdateDataFlatLimits(e4adocument.Data1List, checkDataList);
            if (data1List.RvStoreFlag == "Y")
            {
                data1List.MeasurementAggregates = CreateMeasurementAggregates(data1List.Data1ListRawValues);
            }
            else if (data1List.RvStoreFlag == "N")
            {
                data1List.MeasurementAggregates = LoadMeasurementAggregates(data1List.Data1ListRawValues);
            }
            return data1List;
        }

        /// <summary>
        /// Update strange data for an already existing lot document
        /// </summary>
        /// <param name="e4adocument"></param>
        /// <param name="checkdocument"></param>
        /// <returns></returns>
        private static StrangeDataFlatMetaDataPads UpdateDataFlatStrangeMetaData(SpaceE4A e4adocument, SpacePads checkdocument)
        {
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var checkMetaData = checkdocument.StrangeDataFlatMetaData;
            var dataFlatStrangeMetaData = new StrangeDataFlatMetaDataPads();
            BaseLotAggregation.InitUpdateStrangeMetaData(dataFlatStrangeMetaData, e4aMetaData, checkMetaData);
            dataFlatStrangeMetaData.Pin = SpaceAggregationUtils.JoinStrings(checkMetaData.Pin, e4aMetaData.Pin);
            dataFlatStrangeMetaData.BeSegmentName = SpaceAggregationUtils.JoinStrings(checkMetaData.BeSegmentName, e4aMetaData.BeSegmentName);
            dataFlatStrangeMetaData.BeSort = SpaceAggregationUtils.JoinStrings(checkMetaData.BeSort, e4aMetaData.BeSort);
            dataFlatStrangeMetaData.Data1 = SpaceAggregationUtils.JoinStrings(checkMetaData.Data1, e4aMetaData.Data1);
            dataFlatStrangeMetaData.Data2 = SpaceAggregationUtils.JoinStrings(checkMetaData.Data2, e4aMetaData.Data2);
            dataFlatStrangeMetaData.Data3 = SpaceAggregationUtils.JoinStrings(checkMetaData.Data3, e4aMetaData.Data3);
            dataFlatStrangeMetaData.Device = SpaceAggregationUtils.JoinStrings(checkMetaData.Device, e4aMetaData.Device);
            dataFlatStrangeMetaData.Module = SpaceAggregationUtils.JoinStrings(checkMetaData.Module, e4aMetaData.Module);
            dataFlatStrangeMetaData.DeviceFamily = SpaceAggregationUtils.JoinStrings(checkMetaData.DeviceFamily, e4aMetaData.DeviceFamily);
            dataFlatStrangeMetaData.ErrorCode = SpaceAggregationUtils.JoinStrings(checkMetaData.ErrorCode, e4aMetaData.ErrorCode);
            dataFlatStrangeMetaData.Group1 = SpaceAggregationUtils.JoinStrings(checkMetaData.Group1, e4aMetaData.Group1);
            dataFlatStrangeMetaData.Group2 = SpaceAggregationUtils.JoinStrings(checkMetaData.Group2, e4aMetaData.Group2);
            dataFlatStrangeMetaData.Group3 = SpaceAggregationUtils.JoinStrings(checkMetaData.Group3, e4aMetaData.Group3);
            dataFlatStrangeMetaData.GroupId = SpaceAggregationUtils.JoinStrings(checkMetaData.GroupId, e4aMetaData.GroupId);
            dataFlatStrangeMetaData.EquipmentType = SpaceAggregationUtils.JoinStrings(checkMetaData.EquipmentType, e4aMetaData.EquipmentType);
            dataFlatStrangeMetaData.ManufacturingWipLevel = SpaceAggregationUtils.JoinStrings(checkMetaData.ManufacturingWipLevel, e4aMetaData.ManufacturingWipLevel);
            dataFlatStrangeMetaData.OriginSampleSize = SpaceAggregationUtils.JoinStrings(checkMetaData.OriginSampleSize, e4aMetaData.OriginSampleSize);
            dataFlatStrangeMetaData.Owner = SpaceAggregationUtils.JoinStrings(checkMetaData.Owner, e4aMetaData.Owner);
            dataFlatStrangeMetaData.Package = SpaceAggregationUtils.JoinStrings(checkMetaData.Package, e4aMetaData.Package);
            dataFlatStrangeMetaData.PackageClass = SpaceAggregationUtils.JoinStrings(checkMetaData.PackageClass, e4aMetaData.PackageClass);
            dataFlatStrangeMetaData.PackageFamily = SpaceAggregationUtils.JoinStrings(checkMetaData.PackageFamily, e4aMetaData.PackageFamily);
            dataFlatStrangeMetaData.PackageGroup = SpaceAggregationUtils.JoinStrings(checkMetaData.PackageGroup, e4aMetaData.PackageGroup);
            dataFlatStrangeMetaData.ProductName = SpaceAggregationUtils.JoinStrings(checkMetaData.ProductName, e4aMetaData.ProductName);
            dataFlatStrangeMetaData.SalesName = SpaceAggregationUtils.JoinStrings(checkMetaData.SalesName, e4aMetaData.SalesName);
            dataFlatStrangeMetaData.SampleType = SpaceAggregationUtils.JoinStrings(checkMetaData.SampleType, e4aMetaData.SampleType);
            dataFlatStrangeMetaData.Segment = SpaceAggregationUtils.JoinStrings(checkMetaData.Segment, e4aMetaData.Segment);
            dataFlatStrangeMetaData.UserClass1 = SpaceAggregationUtils.JoinStrings(checkMetaData.UserClass1, e4aMetaData.UserClass1);
            dataFlatStrangeMetaData.UserClass2 = SpaceAggregationUtils.JoinStrings(checkMetaData.UserClass2, e4aMetaData.UserClass2);
            dataFlatStrangeMetaData.UserClass3 = SpaceAggregationUtils.JoinStrings(checkMetaData.UserClass3, e4aMetaData.UserClass3);
            dataFlatStrangeMetaData.Wire = SpaceAggregationUtils.JoinStrings(checkMetaData.Wire, e4aMetaData.Wire);
            return dataFlatStrangeMetaData;
        }
        /// <summary>
        /// updates the existing document's dataflatmetadata subdocument.
        /// </summary>
        /// <param name="e4adocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <param name="checkdocument"> existing pads document</param>
        /// <returns>returns dataflatmetadata subdocument to the parent method</returns>
        private static DataFlatMetaDataPads UpdateDataFlatMetaData(SpaceE4A e4adocument, SpacePads checkdocument)
        {
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var checkMetaData = checkdocument.DataFlatMetaData;
            var dataFlatMetaData = new DataFlatMetaDataPads();
            BaseLotAggregation.InitUpdateDataFlatMetaData(dataFlatMetaData, e4aMetaData, checkMetaData);
            dataFlatMetaData.SpecName = SpaceAggregationUtils.JoinStrings(checkMetaData.SpecName, e4aMetaData.SpecName);
            dataFlatMetaData.Material = SpaceAggregationUtils.JoinStrings(checkMetaData.Material, e4aMetaData.Material);
            dataFlatMetaData.OperatorId = SpaceAggregationUtils.JoinStrings(checkMetaData.OperatorId, e4aMetaData.OperatorId);
            return dataFlatMetaData;
        }
        /// <summary>
        /// updates dataflatlimits subdocument
        /// </summary>
        /// <param name="e4adocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <param name="checkdocument">existing mongodb document</param>
        /// <returns>returns the updated dataflatlimits subdocument to the parent method</returns>
        private static DataFlatLimitsPads UpdateDataFlatLimits(Data1ListE4A e4adocument, Data1ListPads checkdocument)
        {
            var dataFlatLimts = new DataFlatLimitsPads()
            {
                CKCId = SpaceAggregationUtils.JoinStrings(checkdocument.DataFlatLimits.CKCId, e4adocument.DataFlatLimits.CkcId),
                MeasurementSpecLimits = UpdateMeasurementSpecLimits(e4adocument, checkdocument),
                ControlLimits = UpdateControlLimits(e4adocument, checkdocument)
            };
            return dataFlatLimts;
        }
        /// <summary>
        /// updates the rawvalues in the existing document by verifying and
        /// adding non exisiting from the e4a document.
        /// </summary>
        /// <param name="e4adocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <param name="checkdocument">existing document from mongodb</param>
        /// <returns>returns updated list of rawvalues to the parent method</returns>
        private static List<Data1ListRawValuesPads> UpdateData1ListRawValuesPADS(SpaceE4A e4adocument, Data1ListPads checkdocument)
        {
            var padsRawValuesList = new List<Data1ListRawValuesPads>();
            var e4aData1List = e4adocument.Data1List;
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var e4aRawSampleId = e4adocument.Data1List.Data1ListRawValues.Select(x => x.SampleId).FirstOrDefault();
            var spaceAggregates = e4aData1List.DataFlatLimits.SpaceAggregates;
            foreach (var e4aRawValues in e4aData1List.Data1ListRawValues)
            {
                CreateNewRawValues(e4adocument, checkdocument, padsRawValuesList, e4aMetaData, spaceAggregates, e4aRawValues);
            }

            foreach (var checkRawValues in checkdocument.Data1ListRawValues)
            {
                AddExistingRawValues(padsRawValuesList, checkRawValues, e4aMetaData, e4aRawSampleId);
            }
            return padsRawValuesList;
        }

        private static void AddExistingRawValues(List<Data1ListRawValuesPads> padsRawValuesList, Data1ListRawValuesPads checkRawValues, DataFlatMetaDataE4A metaData, long e4aRawSampleId)
        {
            var rawValues = new Data1ListRawValuesPads();
            BaseLotAggregation.InitUpdateRawValues(rawValues, checkRawValues, metaData, e4aRawSampleId);
            rawValues.WaferLot = checkRawValues.WaferLot;
            rawValues.SampleSize = checkRawValues.SampleSize;
            padsRawValuesList.Add(rawValues);
        }

        private static void CreateNewRawValues(SpaceE4A e4adocument, Data1ListPads checkdocument, List<Data1ListRawValuesPads> padsRawValuesList, DataFlatMetaDataE4A e4aMetaData, Space.Common.Data.E4AModel.BaseSpaceAggregatesE4A spaceAggregates, Data1ListRawValuesE4A e4aRawValues)
        {
            bool isEqual = false;
            foreach (var checkRawValues in checkdocument.Data1ListRawValues)
            {
                isEqual = CheckExisitingRawValues(e4aRawValues, isEqual, checkRawValues);
            }
            if (!isEqual)
            {
                var rawValues = new Data1ListRawValuesPads();
                BaseLotAggregation.InitCreateRawValues(rawValues, e4aRawValues, e4aMetaData, spaceAggregates);
                rawValues.WaferLot = e4aRawValues.WaferLot;
                rawValues.SampleSize = e4adocument.Data1List.SampleSize;
                padsRawValuesList.Add(rawValues);
            }
        }

        internal static bool CheckExisitingRawValues(Data1ListRawValuesE4A e4aRawValues, bool isEqual, Data1ListRawValuesPads checkRawValues)
        {
            if (e4aRawValues.WaferLot == null)
            {
                if (e4aRawValues.SampleId == checkRawValues.SampleId
                    && e4aRawValues.Value.Equals(checkRawValues.Value)
                    && checkRawValues.WaferLot == null
                    && e4aRawValues.Seqnr.Equals(checkRawValues.Seqnr))
                {
                    isEqual = true;
                }
            }
            else
            {
                if (e4aRawValues.SampleId == checkRawValues.SampleId
                    && e4aRawValues.Value.Equals(checkRawValues.Value)
                    && e4aRawValues.WaferLot.Equals(checkRawValues.WaferLot, StringComparison.Ordinal)
                    && e4aRawValues.Seqnr.Equals(checkRawValues.Seqnr))
                {
                    isEqual = true;
                }
            }

            return isEqual;
        }
        /// <summary>
        /// updates the control limits for ckc_id = 0.
        /// </summary>
        /// <param name="e4adocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <param name="checkdocument">exisiting mongodb document</param>
        /// <returns> returns controllimits subdocument to the dataflatlimits subdocument</returns>
        private static ControlLimitsPads UpdateControlLimits(Data1ListE4A e4adocument, Data1ListPads checkdocument)
        {
            var controlLimits = new ControlLimitsPads();
            var checkLimits = checkdocument.DataFlatLimits.ControlLimits;
            var e4aLimits = e4adocument.DataFlatLimits.ControlLimits;
            BaseLotAggregation.InitUpdateControlLimits(controlLimits, e4aLimits, checkLimits);
            return controlLimits;
        }
        /// <summary>
        /// updates specification limits.
        /// </summary>
        /// <param name="e4aDocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <param name="checkDocument">existing mongodb document</param>
        /// <returns>returns speclimits subdocument to the dataflatlimits subdocument</returns>
        private static MeasurementSpecLimitsPads UpdateMeasurementSpecLimits(Data1ListE4A e4aDocument, Data1ListPads checkDocument)
        {
            var checkLimits = checkDocument.DataFlatLimits.MeasurementSpecLimits;
            var e4aLimits = e4aDocument.DataFlatLimits.MeasurementSpecLimits;
            var measurementSpecLimits = new MeasurementSpecLimitsPads();
            BaseLotAggregation.InitUpdateSpecLimits(measurementSpecLimits, e4aLimits, checkLimits);
            return measurementSpecLimits;
        }

        private static string GetLotAggregationId(SpaceE4A e4aEntry)
        {
            string id = e4aEntry.Item.Id;
            string facility = e4aEntry.DataFlatMetaData.ParameterFacility;
            string specName = e4aEntry.DataFlatMetaData.SpecName;
            string oper = e4aEntry.DataFlatMetaData.ParameterOper;
            return $"{id}:SPACEAGGED2:RBG:BE:{facility}:{oper}:{specName}:ProcessControl:1.0";
        }

        private static string GetProductionActionId(SpaceE4A e4aEntry)
        {
            string facility = e4aEntry.DataFlatMetaData.ParameterFacility;
            string specName = e4aEntry.DataFlatMetaData.SpecName;
            string oper = e4aEntry.DataFlatMetaData.ParameterOper;
            return $"SPACEAGGED2:RBG:BE:{facility}:{oper}:{specName}";
        }
    }
}
