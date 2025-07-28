using System.Linq;
using System.Collections.Generic;
using MongoDB.Driver;
using PDS.SpaceFE.RBG.Common.Data.E4AModel;
using PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel;
using PDS.Space.Common.Aggregations;
using PDS.Space.Common.Data.PADSModel;
using PDS.Queue.Api.Message;
using System.Diagnostics.CodeAnalysis;
using PDS.Space.Common;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceFE.RBG.PADS.Module.Aggregations
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
            dataFlatMetaData.SPSNumber = e4aMetaData.SPSNumber;
            dataFlatMetaData.SubOperation = e4aMetaData.SPSNumber;
            dataFlatMetaData.EquipmentType = e4aMetaData.EquipmentType;
            dataFlatMetaData.Recipe = e4aMetaData.Recipe;
            return dataFlatMetaData;
        }
        /// <summary>
        /// Create Strange metadata for a new lot document
        /// </summary>
        /// <param name="e4adocument"></param>
        /// <returns>StrangeDataFlatMetaDataPads object</returns>
        private static StrangeDataFlatMetaDataPads CreateStrangeMetaData(SpaceE4A e4adocument)
        {
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var dataFlatStrangeMetaData = new StrangeDataFlatMetaDataPads();
            BaseLotAggregation.InitCreateStrangeData(dataFlatStrangeMetaData, e4aMetaData);
            dataFlatStrangeMetaData.ProcessGroup = e4aMetaData.ProcessGroup;
            dataFlatStrangeMetaData.ProcessLine = e4aMetaData.ProcessLine;
            dataFlatStrangeMetaData.Design = e4aMetaData.Design;
            dataFlatStrangeMetaData.Layer = e4aMetaData.Layer;
            dataFlatStrangeMetaData.MeasurementBatch = e4aMetaData.MeasurementBatch;
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
            data1List.ProcessBatch = e4aMetaData.ProcessBatch;
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
        /// <returns>List<Data1ListRawValuesPads> object</returns>
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
        /// <returns>Data1ListRawValuesPads object</returns>
        internal static Data1ListRawValuesPads CreatData1ListRawValues(Data1ListRawValuesE4A e4aRawValues, SpaceE4A e4aDocument)
        {
            string proberId = "";
            if (e4aRawValues.MotherLotWafer != null && e4aRawValues.X != null && e4aRawValues.Y != null)
            {
                proberId = $"{e4aRawValues.MotherLotWafer.Split(":")[0]}:{e4aRawValues.MotherLotWafer.Split(":").Last()}:{e4aRawValues.X}:{e4aRawValues.Y}";
            }
            var e4aMetadata = e4aDocument.DataFlatMetaData;
            var spaceAggregates = e4aDocument.Data1List.DataFlatLimits.SpaceAggregates;
            string wafer = e4aRawValues.MotherLotWafer?.Split(":").Last();
            var rawValues = new Data1ListRawValuesPads();
            BaseLotAggregation.InitCreateRawValues(rawValues, e4aRawValues, e4aMetadata, spaceAggregates);
            rawValues.ProcessEquipment = e4aRawValues.ProcessEquipment;
            rawValues.X = e4aRawValues.X;
            rawValues.Y = e4aRawValues.Y;
            rawValues.GOF = e4aRawValues.GOF;
            rawValues.ItemIdMotherlotWafer = e4aRawValues.MotherLotWafer;
            rawValues.ItemIDFEProberChipID = proberId;
            rawValues.Wafer = wafer;
            rawValues.WaferSequence = e4aRawValues.WaferSequence;
            rawValues.Slot = e4aRawValues.Slot;
            rawValues.TestPosition = e4aRawValues.TestPosition;
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
            var measurementAggregates = CreateMeasurementAggregates(rawValues.Count, measurementRawValues, rawValues);
            AddGofAggregates(measurementAggregates, measurementRawValues);
            return measurementAggregates;
        }
        internal static void AddGofAggregates(MeasurementAggregatesPads aggregates, IEnumerable<IData1ListRawValuesPadsGof> rawValues)
        {
            if (!rawValues.Any())
                return;

            var gofValues = new List<double>();
            foreach (var e4aRawValues in rawValues)
            {
                if (e4aRawValues.GOF != null)
                {
                    gofValues.Add(SpaceAggregationUtils.DoubleParse(e4aRawValues.GOF));
                }
            }
            if (gofValues.Count > 0)
            {
                aggregates.GofMin = gofValues.Min();
                aggregates.GofMax = gofValues.Max();
                aggregates.GofMean = gofValues.Average();
            }
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
        /// <returns>Data1ListPads object</returns>
        internal static Data1ListPads UpdateNewData1list(SpaceE4A e4aDocument)
        {
            var e4aMetaData = e4aDocument.DataFlatMetaData;
            var data1List = new Data1ListPads();
            BaseLotAggregation.InitCreateDataList(data1List, e4aMetaData);
            data1List.ProcessBatch = e4aMetaData.ProcessBatch;
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
            return data1List;
        }
        /// <summary>
        /// Update strange data in the already existing lot document
        /// </summary>
        /// <param name="e4adocument"></param>
        /// <param name="checkdocument"></param>
        /// <returns>StrangeDataFlatMetaDataPads object</returns>
        private static StrangeDataFlatMetaDataPads UpdateDataFlatStrangeMetaData(SpaceE4A e4adocument, SpacePads checkdocument)
        {
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var checkMetaData = checkdocument.StrangeDataFlatMetaData;

            var dataFlatStrangeMetaData = new StrangeDataFlatMetaDataPads();
            BaseLotAggregation.InitUpdateStrangeMetaData(dataFlatStrangeMetaData, e4aMetaData, checkMetaData);
            dataFlatStrangeMetaData.ProcessGroup = SpaceAggregationUtils.JoinStrings(checkMetaData.ProcessGroup, e4aMetaData.ProcessGroup);
            dataFlatStrangeMetaData.ProcessLine = SpaceAggregationUtils.JoinStrings(checkMetaData.ProcessLine, e4aMetaData.ProcessLine);
            dataFlatStrangeMetaData.Design = SpaceAggregationUtils.JoinStrings(checkMetaData.Design, e4aMetaData.Design);
            dataFlatStrangeMetaData.Layer = SpaceAggregationUtils.JoinStrings(checkMetaData.Layer, e4aMetaData.Layer);
            dataFlatStrangeMetaData.MeasurementBatch = SpaceAggregationUtils.JoinStrings(checkMetaData.MeasurementBatch, e4aMetaData.MeasurementBatch);
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
            dataFlatMetaData.EquipmentType = SpaceAggregationUtils.JoinStrings(checkMetaData.EquipmentType, e4aMetaData.EquipmentType);
            dataFlatMetaData.SPSNumber = SpaceAggregationUtils.JoinStrings(checkMetaData.SPSNumber, e4aMetaData.SPSNumber);
            dataFlatMetaData.SubOperation = SpaceAggregationUtils.JoinStrings(checkMetaData.SPSNumber, e4aMetaData.SPSNumber);
            dataFlatMetaData.Recipe = SpaceAggregationUtils.JoinStrings(checkMetaData.Recipe, e4aMetaData.Recipe);
            return dataFlatMetaData;
        }
        /// <summary>
        /// update the datalist with new data of already existing parameter in an already existing lot document
        /// </summary>
        /// <param name="e4adocument"></param>
        /// <param name="checkDataList"></param>
        /// <returns>Data1ListPads object</returns>
        internal static Data1ListPads UpdateOldData1list(SpaceE4A e4adocument, Data1ListPads checkDataList)
        {
            var e4aMetaData = e4adocument.DataFlatMetaData;
            var data1List = new Data1ListPads();
            BaseLotAggregation.InitUpdateDataList(data1List, e4aMetaData, checkDataList);
            data1List.ProcessEquipment = SpaceAggregationUtils.JoinStrings(checkDataList.ProcessEquipment, e4aMetaData.ProcessEquipment);
            data1List.ProcessBatch = SpaceAggregationUtils.JoinStrings(checkDataList.ProcessBatch, e4aMetaData.ProcessBatch);
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
        /// <returns>MeasurementAggregatesPads object</returns>
        private static MeasurementAggregatesPads LoadMeasurementAggregates(List<Data1ListRawValuesPads> rawvalues)
        {
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

            var measurementAggregates = new MeasurementAggregatesPads();
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
                measurementAggregates.NumViolations = violationList.Distinct().Count();
            }
            measurementAggregates.Samples = samples.Count > 0 ? string.Join(", ", samples) : null;

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
            string proberId = "";
            if (checkRawValues.ItemIdMotherlotWafer != null && checkRawValues.X != null && checkRawValues.Y != null)
            {
                proberId = $"{checkRawValues.ItemIdMotherlotWafer}:{checkRawValues.X}:{checkRawValues.Y}";
            }
            var rawValues = new Data1ListRawValuesPads();
            BaseLotAggregation.InitUpdateRawValues(rawValues, checkRawValues, metaData, e4aRawSampleId); 
            rawValues.ProcessEquipment = checkRawValues.ProcessEquipment;
            rawValues.ItemIdMotherlotWafer = checkRawValues.ItemIdMotherlotWafer;
            rawValues.Wafer = checkRawValues.Wafer;
            rawValues.ItemIDFEProberChipID = proberId;
            rawValues.X = checkRawValues.X;
            rawValues.Y = checkRawValues.Y;
            rawValues.GOF = checkRawValues.GOF;
            rawValues.WaferSequence = checkRawValues.WaferSequence;
            rawValues.Slot = checkRawValues.Slot;
            rawValues.TestPosition = checkRawValues.TestPosition;
            rawValues.SampleSize = checkRawValues.SampleSize;
            padsRawValuesList.Add(rawValues);
        }

        private static void CreateNewRawValues(SpaceE4A e4adocument, Data1ListPads checkdocument, List<Data1ListRawValuesPads> padsRawValuesList, DataFlatMetaDataE4A e4aMetaData, BaseSpaceAggregatesE4A spaceAggregates, Data1ListRawValuesE4A e4aRawValues)
        {
            bool isEqual = false;
            string proberId = "";
            foreach (var checkRawValues in checkdocument.Data1ListRawValues)
            {
                isEqual = CheckExisitingRawValues(e4aRawValues, isEqual, checkRawValues);
            }
            if (!isEqual)
            {
                if (e4aRawValues.MotherLotWafer != null && e4aRawValues.X != null && e4aRawValues.Y != null)
                {
                    proberId = $"{e4aRawValues.MotherLotWafer.Split(":")[0]}:{e4aRawValues.MotherLotWafer.Split(":").Last()}:{e4aRawValues.X}:{e4aRawValues.Y}";
                }
                string wafer = e4aRawValues.MotherLotWafer?.Split(":").Last();
                var rawValues = new Data1ListRawValuesPads();
                BaseLotAggregation.InitCreateRawValues(rawValues, e4aRawValues, e4aMetaData, spaceAggregates);
                rawValues.ProcessEquipment = e4aRawValues.ProcessEquipment;
                rawValues.X = e4aRawValues.X;
                rawValues.Y = e4aRawValues.Y;
                rawValues.GOF = e4aRawValues.GOF;
                rawValues.ItemIdMotherlotWafer = e4aRawValues.MotherLotWafer;
                rawValues.ItemIDFEProberChipID = proberId;
                rawValues.Wafer = wafer;
                rawValues.WaferSequence = e4aRawValues.WaferSequence;
                rawValues.Slot = e4aRawValues.Slot;
                rawValues.TestPosition = e4aRawValues.TestPosition;
                rawValues.SampleSize = e4adocument.Data1List.SampleSize;
                padsRawValuesList.Add(rawValues);
            }
        }

        private static bool CheckExisitingRawValues(Data1ListRawValuesE4A e4aRawValues, bool isEqual, Data1ListRawValuesPads checkRawValues)
        {
            if (e4aRawValues.MotherLotWafer == null)
            {
                if (e4aRawValues.SampleId == checkRawValues.SampleId
                    && e4aRawValues.Value.Equals(checkRawValues.Value)
                    && checkRawValues.ItemIdMotherlotWafer == null
                    && e4aRawValues.Seqnr.Equals(checkRawValues.Seqnr))
                {
                    isEqual = true;
                }
            }
            else if (e4aRawValues.SampleId == checkRawValues.SampleId
                    && e4aRawValues.Value.Equals(checkRawValues.Value)
                    && e4aRawValues.MotherLotWafer.Equals(checkRawValues.ItemIdMotherlotWafer, System.StringComparison.Ordinal)
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
            string subOper = e4aEntry.DataFlatMetaData.SPSNumber;
            return $"{id}:SPACEAGGED1:RBG:FE:{facility}:{oper}:{subOper}:ProcessControl:1.0";
        }

        private static string GetProductionActionId(SpaceE4A e4aEntry)
        {
            string facility = e4aEntry.DataFlatMetaData.ParameterFacility;
            string oper = e4aEntry.DataFlatMetaData.ParameterOper;
            string subOper = e4aEntry.DataFlatMetaData.SPSNumber;
            return $"SPACEAGGED1:RBG:FE:{facility}:{oper}:{subOper}";
        }
    }
}
