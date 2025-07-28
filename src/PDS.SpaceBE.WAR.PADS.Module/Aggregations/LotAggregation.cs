using System.Linq;
using System.Collections.Generic;
using MongoDB.Driver;
using PDS.SpaceBE.WAR.Common.Data.E4AModel;
using PDS.SpaceBE.WAR.PADS.Module.Data.PADSModel;
using PDS.Space.Common.Aggregations;
using PDS.Space.Common.Data.PADSModel;
using PDS.Space.Common.Data.E4AModel;
using PDS.Queue.Api.Message;
using PDS.Space.Common;
using System.Diagnostics.CodeAnalysis;

namespace PDS.SpaceBE.WAR.PADS.Module.Aggregations
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
            var searchPatterns = new SearchPatternsPads()
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
                SearchPatterns = searchPatterns
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
            dataFlatMetaData.MeasLot = e4aMetaData.ProductionOrder;
            dataFlatMetaData.ProductionOrder = e4aMetaData.ProductionOrder;
            dataFlatMetaData.Line = e4aMetaData.Line;
            dataFlatMetaData.Batch = e4aMetaData.Batch;
            dataFlatMetaData.CustomComment = e4aMetaData.ParameterOper;
            dataFlatMetaData.MaterialNumber = e4aMetaData.MaterialNumber;
            dataFlatMetaData.EquipmentName = e4aMetaData.EquipmentName;
            dataFlatMetaData.MeasurementRecipe = e4aMetaData.MeasurementRecipe;
            dataFlatMetaData.OrderType = e4aMetaData.OrderType;
            dataFlatMetaData.ProcessGroup = e4aMetaData.ProcessGroup;
            dataFlatMetaData.ProductGroup = e4aMetaData.ProductGroup;
            dataFlatMetaData.QrkGroup = e4aMetaData.QrkGroup;
            dataFlatMetaData.Segment = e4aMetaData.Segment;
            dataFlatMetaData.SpcClass = e4aMetaData.SpcClass;
            dataFlatMetaData.ProcessStep = e4aMetaData.ProcessStep;
            dataFlatMetaData.CleaningType = e4aMetaData.CleaningType;
            dataFlatMetaData.Robot = e4aMetaData.Robot;
            dataFlatMetaData.CuringProcess = e4aMetaData.CuringProcess;
            dataFlatMetaData.BasePlatePosition = e4aMetaData.BasePlatePosition;
            dataFlatMetaData.CleaningChamber = e4aMetaData.CleaningChamber;
            dataFlatMetaData.USPower = e4aMetaData.USPower;
            dataFlatMetaData.ChipSolderingLine = e4aMetaData.ChipSolderingLine;
            dataFlatMetaData.ShearSurface = e4aMetaData.ShearSurface;
            dataFlatMetaData.StencilThickness = e4aMetaData.StencilThickness;
            dataFlatMetaData.ProcessType = e4aMetaData.ProcessType;
            dataFlatMetaData.CarrierID = e4aMetaData.CarrierID;
            dataFlatMetaData.Surface = e4aMetaData.Surface;
            dataFlatMetaData.SonotrodeNumber = e4aMetaData.SonotrodeNumber;
            dataFlatMetaData.DataType = e4aMetaData.DataType;
            dataFlatMetaData.FootPosition = e4aMetaData.FootPosition;
            dataFlatMetaData.Zone = e4aMetaData.Zone;
            dataFlatMetaData.CarrierFamily = e4aMetaData.CarrierFamily;
            dataFlatMetaData.BondParameter = e4aMetaData.BondParameter;
            dataFlatMetaData.PadSize = e4aMetaData.PadSize;
            dataFlatMetaData.WireDiameter = e4aMetaData.WireDiameter;
            dataFlatMetaData.TerminalConnector = e4aMetaData.TerminalConnector;
            dataFlatMetaData.TerminalType = e4aMetaData.TerminalType;
            dataFlatMetaData.WireMaterial = e4aMetaData.WireMaterial;
            dataFlatMetaData.Material = e4aMetaData.Material;
            dataFlatMetaData.Identifier = e4aMetaData.Identifier;
            dataFlatMetaData.CarrierAnvilPosition = e4aMetaData.CarrierAnvilPosition;
            dataFlatMetaData.PastePrinter = e4aMetaData.PastePrinter;
            dataFlatMetaData.VaduFrameID = e4aMetaData.VaduFrameID;
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
            dataFlatStrangeMetaData.F56Parameter = e4aMetaData.F56Parameter;
            dataFlatStrangeMetaData.GroupId = e4aMetaData.GroupId;
            dataFlatStrangeMetaData.LengthX = e4aMetaData.LengthX;
            dataFlatStrangeMetaData.Package = e4aMetaData.Package;
            dataFlatStrangeMetaData.MaterialNumberText = e4aMetaData.MaterialNumberText;
            dataFlatStrangeMetaData.Module = e4aMetaData.Module;
            dataFlatStrangeMetaData.Operator = e4aMetaData.Operator;
            dataFlatStrangeMetaData.ProcessOwner = e4aMetaData.ProcessOwner;
            dataFlatStrangeMetaData.ProductName = e4aMetaData.ProductName;
            dataFlatStrangeMetaData.RawValPos = e4aMetaData.RawValPos;
            dataFlatStrangeMetaData.Report = e4aMetaData.Report;
            dataFlatStrangeMetaData.Status = e4aMetaData.Status;
            dataFlatStrangeMetaData.TargetCpk = e4aMetaData.TargetCpk;
            dataFlatStrangeMetaData.ParameterClass = e4aMetaData.ParameterClass;
            dataFlatStrangeMetaData.UnitLength = e4aMetaData.UnitLength;
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
            data1List.ParameterType = e4aMetaData.ParameterType;
            data1List.ProcessEquipmentName = e4aMetaData.ProcessEquipmentName;
            data1List.LongParameterName = e4aMetaData.LongParameterName;
            data1List.ProcessEquipment = e4aMetaData.ProcessEquipment;
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
            rawValues.ProcessEquipment = e4aMetadata.ProcessEquipment;
            rawValues.ProcessEquipmentName = e4aMetadata.ProcessEquipmentName;
            rawValues.MeasurementEquipment = e4aMetadata.Equipment;
            rawValues.MeasurementEquipmentName = e4aMetadata.EquipmentName;
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
            data1List.ParameterType = e4aMetaData.ParameterType;
            data1List.ProcessEquipment = e4aMetaData.ProcessEquipment;
            data1List.ProcessEquipmentName = e4aMetaData.ProcessEquipmentName;
            data1List.LongParameterName = e4aMetaData.LongParameterName;
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
            dataFlatStrangeMetaData.F56Parameter = SpaceAggregationUtils.JoinStrings(e4aMetaData.F56Parameter, checkMetaData.F56Parameter);
            dataFlatStrangeMetaData.GroupId = SpaceAggregationUtils.JoinStrings(e4aMetaData.GroupId, checkMetaData.GroupId);
            dataFlatStrangeMetaData.LengthX = SpaceAggregationUtils.JoinStrings(e4aMetaData.LengthX, checkMetaData.LengthX);
            dataFlatStrangeMetaData.Package = SpaceAggregationUtils.JoinStrings(e4aMetaData.Package, checkMetaData.Package);
            dataFlatStrangeMetaData.MaterialNumberText = SpaceAggregationUtils.JoinStrings(e4aMetaData.MaterialNumberText, checkMetaData.MaterialNumberText);
            dataFlatStrangeMetaData.Module = SpaceAggregationUtils.JoinStrings(e4aMetaData.Module, checkMetaData.Module);
            dataFlatStrangeMetaData.Operator = SpaceAggregationUtils.JoinStrings(e4aMetaData.Operator, checkMetaData.Operator);
            dataFlatStrangeMetaData.ProcessOwner = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProcessOwner, checkMetaData.ProcessOwner);
            dataFlatStrangeMetaData.ProductName = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProductName, checkMetaData.ProductName);
            dataFlatStrangeMetaData.RawValPos = SpaceAggregationUtils.JoinStrings(e4aMetaData.RawValPos, checkMetaData.RawValPos);
            dataFlatStrangeMetaData.Report = SpaceAggregationUtils.JoinStrings(e4aMetaData.Report, checkMetaData.Report);
            dataFlatStrangeMetaData.Status = SpaceAggregationUtils.JoinStrings(e4aMetaData.Status, checkMetaData.Status);
            dataFlatStrangeMetaData.TargetCpk = SpaceAggregationUtils.JoinStrings(e4aMetaData.TargetCpk, checkMetaData.TargetCpk);
            dataFlatStrangeMetaData.ParameterClass = SpaceAggregationUtils.JoinStrings(e4aMetaData.ParameterClass, checkMetaData.ParameterClass);
            dataFlatStrangeMetaData.UnitLength = SpaceAggregationUtils.JoinStrings(e4aMetaData.UnitLength, checkMetaData.UnitLength);
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
            dataFlatMetaData.MeasLot = e4aMetaData.ProductionOrder;
            dataFlatMetaData.ProductionOrder = e4aMetaData.ProductionOrder;
            dataFlatMetaData.Line = e4aMetaData.Line;
            dataFlatMetaData.BondParameter = SpaceAggregationUtils.JoinStrings(e4aMetaData.BondParameter, checkMetaData.BondParameter);
            dataFlatMetaData.Batch = SpaceAggregationUtils.JoinStrings(e4aMetaData.Batch, checkMetaData.Batch);
            dataFlatMetaData.CustomComment = SpaceAggregationUtils.JoinStrings(e4aMetaData.ParameterOper, checkMetaData.CustomComment);
            dataFlatMetaData.MaterialNumber = SpaceAggregationUtils.JoinStrings(e4aMetaData.MaterialNumber, checkMetaData.MaterialNumber);
            dataFlatMetaData.EquipmentName = SpaceAggregationUtils.JoinStrings(e4aMetaData.EquipmentName, checkMetaData.EquipmentName);
            dataFlatMetaData.MeasurementRecipe = SpaceAggregationUtils.JoinStrings(e4aMetaData.MeasurementRecipe, checkMetaData.MeasurementRecipe);
            dataFlatMetaData.OrderType = SpaceAggregationUtils.JoinStrings(e4aMetaData.OrderType, checkMetaData.OrderType);
            dataFlatMetaData.ProcessGroup = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProcessGroup, checkMetaData.ProcessGroup);
            dataFlatMetaData.QrkGroup = SpaceAggregationUtils.JoinStrings(e4aMetaData.QrkGroup, checkMetaData.QrkGroup);
            dataFlatMetaData.Segment = SpaceAggregationUtils.JoinStrings(e4aMetaData.Segment, checkMetaData.Segment);
            dataFlatMetaData.ProductGroup = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProductGroup, checkMetaData.ProductGroup);
            dataFlatMetaData.SpcClass = SpaceAggregationUtils.JoinStrings(e4aMetaData.SpcClass, checkMetaData.SpcClass);
            dataFlatMetaData.ProcessStep = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProcessStep, checkMetaData.ProcessStep);
            dataFlatMetaData.CleaningType = SpaceAggregationUtils.JoinStrings(e4aMetaData.CleaningType, checkMetaData.CleaningType);
            dataFlatMetaData.Robot = SpaceAggregationUtils.JoinStrings(e4aMetaData.Robot, checkMetaData.Robot);
            dataFlatMetaData.CuringProcess = SpaceAggregationUtils.JoinStrings(e4aMetaData.CuringProcess, checkMetaData.CuringProcess);
            dataFlatMetaData.BasePlatePosition = SpaceAggregationUtils.JoinStrings(e4aMetaData.BasePlatePosition, checkMetaData.BasePlatePosition);
            dataFlatMetaData.CleaningChamber = SpaceAggregationUtils.JoinStrings(e4aMetaData.CleaningChamber, checkMetaData.CleaningChamber);
            dataFlatMetaData.USPower = SpaceAggregationUtils.JoinStrings(e4aMetaData.USPower, checkMetaData.USPower);
            dataFlatMetaData.ChipSolderingLine = SpaceAggregationUtils.JoinStrings(e4aMetaData.ChipSolderingLine, checkMetaData.ChipSolderingLine);
            dataFlatMetaData.ShearSurface = SpaceAggregationUtils.JoinStrings(e4aMetaData.ShearSurface, checkMetaData.ShearSurface);
            dataFlatMetaData.StencilThickness = SpaceAggregationUtils.JoinStrings(e4aMetaData.StencilThickness, checkMetaData.StencilThickness);
            dataFlatMetaData.ProcessType = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProcessType, checkMetaData.ProcessType);
            dataFlatMetaData.CarrierID = SpaceAggregationUtils.JoinStrings(e4aMetaData.CarrierID, checkMetaData.CarrierID);
            dataFlatMetaData.Surface = SpaceAggregationUtils.JoinStrings(e4aMetaData.Surface, checkMetaData.Surface);
            dataFlatMetaData.SonotrodeNumber = SpaceAggregationUtils.JoinStrings(e4aMetaData.SonotrodeNumber, checkMetaData.SonotrodeNumber);
            dataFlatMetaData.DataType = SpaceAggregationUtils.JoinStrings(e4aMetaData.DataType, checkMetaData.DataType);
            dataFlatMetaData.FootPosition = SpaceAggregationUtils.JoinStrings(e4aMetaData.FootPosition, checkMetaData.FootPosition);
            dataFlatMetaData.Zone = SpaceAggregationUtils.JoinStrings(e4aMetaData.Zone, checkMetaData.Zone);
            dataFlatMetaData.CarrierFamily = SpaceAggregationUtils.JoinStrings(e4aMetaData.CarrierFamily, checkMetaData.CarrierFamily);
            dataFlatMetaData.PadSize = SpaceAggregationUtils.JoinStrings(e4aMetaData.PadSize, checkMetaData.PadSize);
            dataFlatMetaData.WireDiameter = SpaceAggregationUtils.JoinStrings(e4aMetaData.WireDiameter, checkMetaData.WireDiameter);
            dataFlatMetaData.TerminalConnector = SpaceAggregationUtils.JoinStrings(e4aMetaData.TerminalConnector, checkMetaData.TerminalConnector);
            dataFlatMetaData.TerminalType = SpaceAggregationUtils.JoinStrings(e4aMetaData.TerminalType, checkMetaData.TerminalType);
            dataFlatMetaData.WireMaterial = SpaceAggregationUtils.JoinStrings(e4aMetaData.WireMaterial, checkMetaData.WireMaterial);
            dataFlatMetaData.Material = SpaceAggregationUtils.JoinStrings(e4aMetaData.Material, checkMetaData.Material);
            dataFlatMetaData.Identifier = SpaceAggregationUtils.JoinStrings(e4aMetaData.Identifier, checkMetaData.Identifier);
            dataFlatMetaData.CarrierAnvilPosition = SpaceAggregationUtils.JoinStrings(e4aMetaData.CarrierAnvilPosition, checkMetaData.CarrierAnvilPosition);
            dataFlatMetaData.PastePrinter = SpaceAggregationUtils.JoinStrings(e4aMetaData.PastePrinter, checkMetaData.PastePrinter);
            dataFlatMetaData.VaduFrameID = SpaceAggregationUtils.JoinStrings(e4aMetaData.VaduFrameID, checkMetaData.VaduFrameID);
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
            data1List.ParameterType = SpaceAggregationUtils.JoinStrings(e4aMetaData.ParameterType, checkDataList.ParameterType);
            data1List.ProcessEquipment = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProcessEquipment, checkDataList.ProcessEquipment);
            data1List.ProcessEquipmentName = SpaceAggregationUtils.JoinStrings(e4aMetaData.ProcessEquipmentName, checkDataList.ProcessEquipmentName);
            data1List.Data1ListRawValues = UpdateData1ListRawValuesPADS(e4adocument, checkDataList);
            data1List.LongParameterName = SpaceAggregationUtils.JoinStrings(e4aMetaData.LongParameterName, checkDataList.LongParameterName);
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
            var rawvaluesValid = rawvalues.Where(it => it.IsFlagged == "N").ToList();
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
                    meanvals.Add(rawvalue.SampleMean * rawvalue.SampleSize);
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
            measurementAggregates.Samples = string.Join(", ", samples.Distinct());
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
            rawValues.ProcessEquipment = checkRawValues.ProcessEquipment;
            rawValues.ProcessEquipmentName = checkRawValues.ProcessEquipmentName;
            rawValues.SampleSize = checkRawValues.SampleSize;
            rawValues.MeasurementEquipmentName = checkRawValues.MeasurementEquipmentName;
            rawValues.MeasurementEquipment = checkRawValues.MeasurementEquipment;
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
                var e4aMetadata = e4adocument.DataFlatMetaData;
                var rawValues = new Data1ListRawValuesPads();
                BaseLotAggregation.InitCreateRawValues(rawValues, e4aRawValues, e4aMetaData, spaceAggregates);
                rawValues.SampleSize = e4adocument.Data1List.SampleSize;
                rawValues.ProcessEquipment = e4aMetadata.ProcessEquipment;
                rawValues.ProcessEquipmentName = e4aMetadata.ProcessEquipmentName;
                rawValues.MeasurementEquipment = e4aMetadata.Equipment;
                rawValues.MeasurementEquipmentName = e4aMetadata.EquipmentName;
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
            string line = e4aEntry.DataFlatMetaData.Line;
            string oper = e4aEntry.DataFlatMetaData.ParameterOper;
            
            // At the moment Warstein's SPACE data does not provide any subOper information.
            // This may change in the future.
            string subOper = "";
            
            return $"{id}:SPACEAGGED3:WAR:BE:{line}:{oper}:{subOper}:ProcessControl:1.0";
        }

        private static string GetProductionActionId(SpaceE4A e4aEntry)
        {
            string line = e4aEntry.DataFlatMetaData.Line;
            string oper = e4aEntry.DataFlatMetaData.ParameterOper;
            
            // At the moment Warstein's SPACE data does not provide any subOper information.
            // This may change in the future.
            string subOper = "";
            
            return $"SPACEAGGED3:WAR:BE:{line}:{oper}:{subOper}";
        }
    }
}
