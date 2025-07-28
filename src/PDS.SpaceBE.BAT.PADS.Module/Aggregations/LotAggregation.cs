using System.Linq;
using System.Collections.Generic;
using MongoDB.Driver;
using PDS.SpaceBE.BAT.Common.Data.E4AModel;
using PDS.SpaceBE.BAT.PADS.Module.Data.PADSModel;
using PDS.Space.Common.Aggregations;
using PDS.Space.Common.Data.PADSModel;
using PDS.Queue.Api.Message;
using System.Diagnostics.CodeAnalysis;
using PDS.Space.Common;
using PDS.Space.Common.Data.E4AModel;
using SharpCompress.Common;

namespace PDS.SpaceBE.BAT.PADS.Module.Aggregations
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
        /// <returns name="spacePADS">spacePADS is the pads document that contains all the lot aggregations and is returned to SpaceQueueReader.cs to be inserted into MongoDB</returns>
        public SpacePads CreateNew()
        {
            var spacePADS = CreateCommon(E4aDocument);
            spacePADS.DataFlatMetaData = CreateDataFlatMetaData(E4aDocument);
            spacePADS.StrangeDataFlatMetaData = CreateStrangeMetaData(E4aDocument);
            spacePADS.Data1List = CreateData1list(E4aDocument);
            spacePADS.SystemLog = CreateSystemLog();
            return spacePADS;
        }
        /// <summary>
        /// Update the existing pads document and replaces it in mongodb
        /// </summary>
        /// <param name="checkdocument">Existing mogodb document</param>
        /// <returns>spacePADS is the pads document that contains all the lot aggregations and is returned to SpaceQueueReader.cs to be replaced in MongoDB</returns>
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
            spacePADS.SystemLog = UpdateSystemLog(checkdocument.SystemLog);
            spacePADS.Data1List = data1List;
            return spacePADS;
        }
        /// <summary>
        /// create all the common document fields and sub doucments for both creating and updating document.
        /// </summary>
        /// <param name="e4adocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns spacepads document with the filled fields back to the parent method.</returns>
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
            var searchPattern = new SearchPatternsPads()
            {
                SpaceKey = AggregationId,
                TimeGroup = "0",
                SiteKey = e4adocument.DataFlatMetaData.SiteKey
            };
            return new SpacePads()
            {
                Document = document,
                ProductionAction = productionAction,
                Item = item,
                SearchPatterns = searchPattern
            };
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
            dataFlatMetaData.Shift = e4aMetaData.Shift;
            dataFlatMetaData.Process = e4aMetaData.Process;
            return dataFlatMetaData;
        }
        /// <summary>
        /// Create Strange metadata for a new lot document
        /// </summary>
        /// <param name="e4adocument"></param>
        private static StrangeDataFlatMetaDataPads CreateStrangeMetaData(SpaceE4A e4adocument)
        {
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var dataFlatStrangeMetaData = new StrangeDataFlatMetaDataPads();
            BaseLotAggregation.InitCreateStrangeData(dataFlatStrangeMetaData, e4aMetaData);
            dataFlatStrangeMetaData.DieCategory = e4aMetaData.DieCategory;
            dataFlatStrangeMetaData.GaugeId = e4aMetaData.GaugeId;
            dataFlatStrangeMetaData.Operator = e4aMetaData.Operator;
            dataFlatStrangeMetaData.PackageClass = e4aMetaData.PackageClass;
            dataFlatStrangeMetaData.Product = e4aMetaData.Product;
            dataFlatStrangeMetaData.SawStreet = e4aMetaData.SawStreet;
            dataFlatStrangeMetaData.Track = e4aMetaData.Track;
            dataFlatStrangeMetaData.PackageType = e4aMetaData.PackageType;
            dataFlatStrangeMetaData.Device = e4aMetaData.Device;
            dataFlatStrangeMetaData.DieSet = e4aMetaData.DieSet;
            dataFlatStrangeMetaData.PackageFamily = e4aMetaData.PackageFamily;
            dataFlatStrangeMetaData.Package = e4aMetaData.Package;
            dataFlatStrangeMetaData.PadMetal = e4aMetaData.PadMetal;
            dataFlatStrangeMetaData.WaferThickness = e4aMetaData.WaferThickness;
            dataFlatStrangeMetaData.WireSize = e4aMetaData.WireSize;
            dataFlatStrangeMetaData.Classification = e4aMetaData.Classification;
            dataFlatStrangeMetaData.Submission = e4aMetaData.Submission;
            dataFlatStrangeMetaData.Reserve2 = e4aMetaData.Reserve2;
            dataFlatStrangeMetaData.ChipType = e4aMetaData.ChipType;
            dataFlatStrangeMetaData.Area = e4aMetaData.Area;
            dataFlatStrangeMetaData.Capilary = e4aMetaData.Capilary;
            dataFlatStrangeMetaData.McPlatform = e4aMetaData.McPlatform;
            dataFlatStrangeMetaData.OperatorId = e4aMetaData.OperatorId;
            dataFlatStrangeMetaData.MeasurementEquipment = e4aMetaData.MeasurementEquipment;
            dataFlatStrangeMetaData.BladeCategory = e4aMetaData.BladeCategory;
            dataFlatStrangeMetaData.SubmissionType = e4aMetaData.SubmissionType;
            dataFlatStrangeMetaData.SolderGlue = e4aMetaData.SolderGlue;
            dataFlatStrangeMetaData.DieSize = e4aMetaData.DieSize;
            dataFlatStrangeMetaData.Grade = e4aMetaData.Grade;
            dataFlatStrangeMetaData.CartridgeID = e4aMetaData.CartridgeID;
            dataFlatStrangeMetaData.Module = e4aMetaData.Module;
            dataFlatStrangeMetaData.ParameterClass = e4aMetaData.ParameterClass;
            dataFlatStrangeMetaData.TargetCpk = e4aMetaData.TargetCpk;
            dataFlatStrangeMetaData.ProcessOwner = e4aMetaData.ProcessOwner;
            dataFlatStrangeMetaData.Segment = e4aMetaData.Segment;
            dataFlatStrangeMetaData.F56Parameter = e4aMetaData.F56Parameter;
            dataFlatStrangeMetaData.Published = e4aMetaData.Published;
            dataFlatStrangeMetaData.GroupId = e4aMetaData.GroupId;

            return dataFlatStrangeMetaData;
        }
        /// <summary>
        /// Creates list of all the rawvalues from the child method.
        /// </summary>
        /// <param name="e4aDocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns the list to the parent method.</returns>
        internal static List<Data1ListPads> CreateData1list(SpaceE4A e4aDocument)
        {
            var e4aMetaData = e4aDocument.DataFlatMetaData;
            var dataList = new List<Data1ListPads>();
            var data1List = new Data1ListPads();
            BaseLotAggregation.InitCreateDataList(data1List, e4aMetaData);
            data1List.ProcessEquipment = e4aMetaData.ProcessEquipment;
            data1List.Machine = e4aMetaData.Machine;
            data1List.ProcessTool = e4aMetaData.ProcessTool;
            data1List.CFComment = e4aMetaData.CFComment;
            data1List.FourDReport = e4aMetaData.FourDReport;
            data1List.SpecialCharacteristics = e4aMetaData.SpecialCharacteristics;
            data1List.DataFlatLimits = CreateDataFlatLimits(e4aDocument);
            data1List.Data1ListRawValues = CreateData1listRawValuesList(e4aDocument);
            if (data1List.RvStoreFlag == "Y")
            {
                data1List.MeasurementAggregates = CreateMeasurementAggregates(data1List.Data1ListRawValues);
            }
            else if (data1List.RvStoreFlag == "N")
            {
                data1List.MeasurementAggregates = LoadMeasurementAggregates(data1List.Data1ListRawValues);
            }
            dataList.Add(data1List);
            return dataList;
        }
        /// <summary>
        /// Create RawValues List in a lot document
        /// </summary>
        /// <param name="e4aDocument"></param>
        private static List<Data1ListRawValuesPads> CreateData1listRawValuesList(SpaceE4A e4aDocument)
        {
            var list = new List<Data1ListRawValuesPads>();
            foreach (var rawValues in e4aDocument.Data1List.Data1ListRawValues)
            {
                if (list.Count == 0)
                {
                    list.Add(CreatData1ListRawValues(rawValues, e4aDocument));
                }
                else
                {
                    bool isEqual = true;
                    foreach (var i in list)
                    {
                        if (rawValues.SampleId != i.SampleId ||
                            !rawValues.Value.Equals(i.Value) ||
                            rawValues.Seqnr != i.Seqnr)
                        {
                            isEqual = false;
                        }
                    }
                    if (!isEqual)
                    {
                        list.Add(CreatData1ListRawValues(rawValues, e4aDocument));
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// Create Rawvalues to add up to the list
        /// </summary>
        /// <param name="e4aRawValues"></param>
        /// <param name="e4aDocument"></param>
        internal static Data1ListRawValuesPads CreatData1ListRawValues(Data1ListRawValuesE4A e4aRawValues, SpaceE4A e4aDocument)
        {
            var e4aMetadata = e4aDocument.DataFlatMetaData;
            var spaceAggregates = e4aDocument.Data1List.DataFlatLimits.SpaceAggregates;
            var rawValues = new Data1ListRawValuesPads();
            BaseLotAggregation.InitCreateRawValues(rawValues, e4aRawValues, e4aMetadata, spaceAggregates);
            rawValues.SampleSize = e4aDocument.Data1List.SampleSize;
            rawValues.ProcessTool = e4aRawValues.ProcessTool;
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
                ControlLimits = CreateControlLimits(e4adoc)
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
        /// calculates measurement aggregates
        /// </summary>
        /// <param name="rawValues">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <returns>returns the aggregates to the measurementaggregates subdocument</returns>
        internal static MeasurementAggregatesPads CreateMeasurementAggregates(List<Data1ListRawValuesPads> rawValues)
        {
            var measurementRawValues = rawValues.Where(it => it.IsFlagged == "N").ToList();
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
        /// Update DataList when there comes a new parameter in a lot document
        /// </summary>
        /// <param name="e4aDocument"></param>
        internal static Data1ListPads UpdateNewData1list(SpaceE4A e4aDocument)
        {
            var e4aMetaData = e4aDocument.DataFlatMetaData;
            var data1List = new Data1ListPads();
            BaseLotAggregation.InitCreateDataList(data1List, e4aMetaData);
            data1List.ProcessEquipment = e4aMetaData.ProcessEquipment;
            data1List.ProcessTool = e4aMetaData.ProcessTool;
            data1List.Machine = e4aMetaData.Machine;
            data1List.CFComment = e4aMetaData.CFComment;
            data1List.FourDReport = e4aMetaData.FourDReport;
            data1List.SpecialCharacteristics = e4aMetaData.SpecialCharacteristics;
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
        /// Update strange data in the already existing lot document
        /// </summary>
        /// <param name="e4adocument"></param>
        /// <param name="checkdocument"></param>
        private static StrangeDataFlatMetaDataPads UpdateDataFlatStrangeMetaData(SpaceE4A e4adocument, SpacePads checkdocument)
        {
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var checkMetaData = checkdocument.StrangeDataFlatMetaData;

            var dataFlatStrangeMetaData = new StrangeDataFlatMetaDataPads();
            BaseLotAggregation.InitUpdateStrangeMetaData(dataFlatStrangeMetaData, e4aMetaData, checkMetaData);
            dataFlatStrangeMetaData.PackageType = SpaceAggregationUtils.JoinStrings(e4aMetaData.PackageType, checkMetaData.PackageType);
            dataFlatStrangeMetaData.DieCategory = SpaceAggregationUtils.JoinStrings(e4aMetaData.DieCategory, checkMetaData.DieCategory);
            dataFlatStrangeMetaData.GaugeId = SpaceAggregationUtils.JoinStrings(e4aMetaData.GaugeId, checkMetaData.GaugeId);
            dataFlatStrangeMetaData.Operator = SpaceAggregationUtils.JoinStrings(e4aMetaData.Operator, checkMetaData.Operator);
            dataFlatStrangeMetaData.PackageClass = SpaceAggregationUtils.JoinStrings(e4aMetaData.PackageClass, checkMetaData.PackageClass);
            dataFlatStrangeMetaData.Product = SpaceAggregationUtils.JoinStrings(e4aMetaData.Product, checkMetaData.Product);
            dataFlatStrangeMetaData.SawStreet = SpaceAggregationUtils.JoinStrings(e4aMetaData.SawStreet, checkMetaData.SawStreet);
            dataFlatStrangeMetaData.Track = SpaceAggregationUtils.JoinStrings(e4aMetaData.Track, checkMetaData.Track);
            dataFlatStrangeMetaData.Device = SpaceAggregationUtils.JoinStrings(e4aMetaData.Device, checkMetaData.Device);
            dataFlatStrangeMetaData.DieSet = SpaceAggregationUtils.JoinStrings(e4aMetaData.DieSet, checkMetaData.DieSet);
            dataFlatStrangeMetaData.PackageFamily = SpaceAggregationUtils.JoinStrings(e4aMetaData.PackageFamily, checkMetaData.PackageFamily);
            dataFlatStrangeMetaData.Package = SpaceAggregationUtils.JoinStrings(e4aMetaData.Package, checkMetaData.Package);
            dataFlatStrangeMetaData.PadMetal = SpaceAggregationUtils.JoinStrings(e4aMetaData.PadMetal, checkMetaData.PadMetal);
            dataFlatStrangeMetaData.WaferThickness = SpaceAggregationUtils.JoinStrings(e4aMetaData.WaferThickness, checkMetaData.WaferThickness);
            dataFlatStrangeMetaData.WireSize = SpaceAggregationUtils.JoinStrings(e4aMetaData.WireSize, checkMetaData.WireSize);
            dataFlatStrangeMetaData.Classification = SpaceAggregationUtils.JoinStrings(e4aMetaData.Classification, checkMetaData.Classification);
            dataFlatStrangeMetaData.Submission = SpaceAggregationUtils.JoinStrings(e4aMetaData.Submission, checkMetaData.Submission);
            dataFlatStrangeMetaData.Reserve2 = SpaceAggregationUtils.JoinStrings(e4aMetaData.Reserve2, checkMetaData.Reserve2);
            dataFlatStrangeMetaData.ChipType = SpaceAggregationUtils.JoinStrings(e4aMetaData.ChipType, checkMetaData.ChipType);
            dataFlatStrangeMetaData.Area = SpaceAggregationUtils.JoinStrings(e4aMetaData.Area, checkMetaData.Area);
            dataFlatStrangeMetaData.Capilary = SpaceAggregationUtils.JoinStrings(e4aMetaData.Capilary, checkMetaData.Capilary);
            dataFlatStrangeMetaData.McPlatform = SpaceAggregationUtils.JoinStrings(e4aMetaData.McPlatform, checkMetaData.McPlatform);
            dataFlatStrangeMetaData.OperatorId = SpaceAggregationUtils.JoinStrings(e4aMetaData.OperatorId, checkMetaData.OperatorId);
            dataFlatStrangeMetaData.MeasurementEquipment = SpaceAggregationUtils.JoinStrings(e4aMetaData.MeasurementEquipment, checkMetaData.MeasurementEquipment);
            dataFlatStrangeMetaData.BladeCategory = SpaceAggregationUtils.JoinStrings(e4aMetaData.BladeCategory, checkMetaData.BladeCategory);
            dataFlatStrangeMetaData.SolderGlue = SpaceAggregationUtils.JoinStrings(e4aMetaData.SolderGlue, checkMetaData.SolderGlue);
            dataFlatStrangeMetaData.SubmissionType = SpaceAggregationUtils.JoinStrings(e4aMetaData.SubmissionType, checkMetaData.SubmissionType);
            dataFlatStrangeMetaData.DieSize = SpaceAggregationUtils.JoinStrings(e4aMetaData.DieSize, checkMetaData.DieSize);
            dataFlatStrangeMetaData.Grade = SpaceAggregationUtils.JoinStrings(e4aMetaData.Grade, checkMetaData.Grade);
            dataFlatStrangeMetaData.CartridgeID = SpaceAggregationUtils.JoinStrings(e4aMetaData.CartridgeID, checkMetaData.CartridgeID);
            dataFlatStrangeMetaData.Module = SpaceAggregationUtils.JoinStrings(e4aMetaData.Module, checkMetaData.Module);
            dataFlatStrangeMetaData.ParameterClass = SpaceAggregationUtils.JoinStrings(e4aMetaData.ParameterClass, checkMetaData.ParameterClass);
            dataFlatStrangeMetaData.TargetCpk = SpaceAggregationUtils.JoinStrings(e4aMetaData.TargetCpk, checkMetaData.TargetCpk);
            dataFlatStrangeMetaData.ProcessOwner = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProcessOwner, checkMetaData.ProcessOwner);
            dataFlatStrangeMetaData.Segment = SpaceAggregationUtils.JoinStrings(e4aMetaData.Segment, checkMetaData.Segment);
            dataFlatStrangeMetaData.F56Parameter = SpaceAggregationUtils.JoinStrings(e4aMetaData.F56Parameter, checkMetaData.F56Parameter);
            dataFlatStrangeMetaData.Published = SpaceAggregationUtils.JoinStrings(e4aMetaData.Published, checkMetaData.Published);
            dataFlatStrangeMetaData.GroupId = SpaceAggregationUtils.JoinStrings(e4aMetaData.GroupId, checkMetaData.GroupId);
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
            dataFlatMetaData.Shift = SpaceAggregationUtils.JoinStrings(e4aMetaData.Shift, checkMetaData.Shift);
            dataFlatMetaData.Process = SpaceAggregationUtils.JoinStrings(e4aMetaData.Process, checkMetaData.Process);
            return dataFlatMetaData;
        }
        /// <summary>
        /// update the datalist with new data of already existing parameter in an already existing lot document
        /// </summary>
        /// <param name="e4adocument"></param>
        /// <param name="checkDataList"></param>
        internal static Data1ListPads UpdateOldData1list(SpaceE4A e4adocument, Data1ListPads checkDataList)
        {
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var data1List = new Data1ListPads();
            BaseLotAggregation.InitUpdateDataList(data1List, e4aMetaData, checkDataList);
            data1List.ProcessEquipment = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProcessEquipment, checkDataList.ProcessEquipment);
            data1List.ProcessTool = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProcessTool, checkDataList.ProcessTool);
            data1List.Machine = SpaceAggregationUtils.JoinStrings(e4aMetaData.Machine, checkDataList.Machine);
            data1List.CFComment = SpaceAggregationUtils.JoinStrings(e4aMetaData.CFComment, checkDataList.CFComment);
            data1List.FourDReport = SpaceAggregationUtils.JoinStrings(e4aMetaData.FourDReport, checkDataList.FourDReport);
            data1List.SpecialCharacteristics = SpaceAggregationUtils.JoinStrings(e4aMetaData.SpecialCharacteristics, checkDataList.SpecialCharacteristics);
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
        /// To load the aggregates from space as it is when there are no rawvalues to calculate from.
        /// </summary>
        /// <param name="rawvalues"></param>
        internal static MeasurementAggregatesPads LoadMeasurementAggregates(List<Data1ListRawValuesPads> rawvalues)
        {
            int baseCount = 0;
            int flaggedCount = 0;
            var meanvals = new List<double>();
            var minvals = new List<double>();
            var maxvals = new List<double>();
            var primaryViolations = new List<string>();
            var primaryViolComm = new List<string>();
            var violationList = new List<string>();
            var violationComments = new List<string>();
            var numViolations = new List<int>();

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
                    meanvals.Add(rawvalue.SampleMean);
                    minvals.Add(rawvalue.SampleMin);
                    maxvals.Add(rawvalue.SampleMax);
                }
                else
                {
                    flaggedCount += rawvalue.SampleSize;
                }
            }

            var measurementAggregates = new MeasurementAggregatesPads();
            if (baseCount > 0)
            {
                measurementAggregates = new MeasurementAggregatesPads()
                {
                    BaseCount = baseCount,
                    FlaggedCount = flaggedCount,
                    ExecCount = baseCount + flaggedCount,
                    Mean = meanvals.Sum() / baseCount,
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
                measurementAggregates.NumViolations = (measurementAggregates.ViolationList?.Split(",").Length) ?? 0;
            }
            return measurementAggregates;
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
            rawValues.SampleSize = checkRawValues.SampleSize;
            rawValues.ProcessTool = checkRawValues.ProcessTool;
            padsRawValuesList.Add(rawValues);
        }

        private static void CreateNewRawValues(SpaceE4A e4adocument, Data1ListPads checkdocument, List<Data1ListRawValuesPads> padsRawValuesList, DataFlatMetaDataE4A e4aMetaData, BaseSpaceAggregatesE4A spaceAggregates, Data1ListRawValuesE4A e4aRawValues)
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
                rawValues.SampleSize = e4adocument.Data1List.SampleSize;
                rawValues.ProcessTool = e4aRawValues.ProcessTool;
                padsRawValuesList.Add(rawValues);
            }
        }

        internal static bool CheckExisitingRawValues(Data1ListRawValuesE4A e4aRawValues, bool isEqual, Data1ListRawValuesPads checkRawValues)
        {
            if (e4aRawValues.SampleId == checkRawValues.SampleId
                    && e4aRawValues.Value.Equals(checkRawValues.Value)
                    && e4aRawValues.Seqnr.Equals(checkRawValues.Seqnr))
            {
                isEqual = true;
            }
            return isEqual;
        }

        /// <summary>
        /// updates dataflatlimits subdocument
        /// </summary>
        /// <param name="e4adocument">e4adocument is the incoming e4adocument from the filesystem</param>
        /// <param name="checkdocument">existing mongodb document</param>
        /// <returns>returns the updated dataflatlimits subdocument to the parent method</returns>
        private static DataFlatLimitsPads UpdateDataFlatLimits(Data1ListE4A e4adocument, Data1ListPads checkdocument)
        {
            return new DataFlatLimitsPads
            {
                CKCId = SpaceAggregationUtils.JoinStrings(checkdocument.DataFlatLimits.CKCId, e4adocument.DataFlatLimits.CkcId),
                MeasurementSpecLimits = UpdateMeasurementSpecLimits(e4adocument, checkdocument),
                ControlLimits = UpdateControlLimits(e4adocument, checkdocument)
            };
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
            string oper = e4aEntry.DataFlatMetaData.ParameterOper;
            string subOper = "";
            return $"{id}:SPACEAGGED2:BAT:BE:{facility}:{oper}:{subOper}:ProcessControl:1.0";
        }

        private static string GetProductionActionId(SpaceE4A e4aEntry)
        {
            string facility = e4aEntry.DataFlatMetaData.ParameterFacility;
            string oper = e4aEntry.DataFlatMetaData.ParameterOper;
            string subOper = "";
            return $"SPACEAGGED2:BAT:BE:{facility}:{oper}:{subOper}";
        }
    }
}
