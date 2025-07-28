using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceFE.RBG.PADS.Module.Aggregations;
using PDS.Common.Utils;
using PDS.SpaceFE.RBG.Common.Data.E4AModel;
using PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel;
using System.Collections.Generic;
using Moq;
using PDS.Queue.Api.Message;

namespace PDS.SpaceFE.RBG.PADS.Module.Tests.Aggregations
{
    /// <summary>
    /// Unit tests for <see cref="WaferAggregation"/>
    /// </summary>
    [TestClass]
    public class WaferAggregationTest
    {
        [TestMethod]
        public void TestCreateNew()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.WaferResources.TestCreateNewValidWafer);
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.Lotdoc);
            string idWafAgg = "MotherlotWafer:3A110879:01" +
                    $":SPACEAGGED1:RBG:FE:{spaceE4A.DataFlatMetaData.ParameterFacility}:" +
                    $"{spaceE4A.DataFlatMetaData.ParameterOper}:{spaceE4A.DataFlatMetaData.SPSNumber}:ProcessControl:1.0";
            var checkvalueslist = new List<Data1ListRawValuesPads4Wafer>();
            var checkvalues1 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "Y",
                Value = 23.45,
                SampleId = 543245,
                Seqnr = 1,
                GOF = "0.88",
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                NumViolations = 0
            };
            var checkvalues2 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "Height_Bumps",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.345,
                SampleId = 543245,
                Seqnr = 2,
                GOF = "0.88",
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                NumViolations = 0
            };
            var checkvalues3 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.455,
                SampleId = 543245,
                Seqnr = 3,
                GOF = "0.88",
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                NumViolations = 0
            };
            checkvalueslist.Add(checkvalues1);
            checkvalueslist.Add(checkvalues2);
            checkvalueslist.Add(checkvalues3);
            var aggregation = new WaferAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.CreateNew(idWafAgg, "1", lotPADS, checkvalueslist);
            Assert.AreEqual("MotherlotWafer:3A110879:01:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestUpdate1ListExistingParams()
        {
            var dataList = new List<Data1ListPads> {
                new Data1ListPads{ ParameterName = "WARPAGE"  },
                new Data1ListPads{ ParameterName = "WARPAGE2" }
            };
            var checkparams = new Data1ListPads { ParameterName = "WARPAGE" };
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>()
            {
                new Data1ListRawValuesPads4Wafer{ Value = 10, ItemIdMotherlotWafer = "RU123456:10" },
                new Data1ListRawValuesPads4Wafer{ Value = 10, ItemIdMotherlotWafer = "RU123456:10" },
            };
            WaferAggregation.UpdateData1ListExisting(dataList, checkparams, checkValueList, "10");
        }

        [TestMethod]
        public void TestCreateNewWaferDocInvalidvalues()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.WaferResources.TestCreateNewInvalidrawvalues);
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.LotdocInvalidValues);
            string idWafAgg = "MotherlotWafer:3A110879:01" +
                    $":SPACEAGGED1:RBG:FE:{spaceE4A.DataFlatMetaData.ParameterFacility}:" +
                    $"{spaceE4A.DataFlatMetaData.ParameterOper}:{spaceE4A.DataFlatMetaData.SPSNumber}:ProcessControl:1.0";
            var checkvalueslist = new List<Data1ListRawValuesPads4Wafer>();
            var checkvalues1 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "Y",
                Value = 23.45,
                SampleId = 543245,
                Seqnr = 1,
                GOF = "0.88",
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                NumViolations = 0
            };
            var checkvalues2 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.345,
                SampleId = 543245,
                Seqnr = 2,
                GOF = "0.88",
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                NumViolations = 0
            };
            var checkvalues3 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.455,
                SampleId = 543245,
                Seqnr = 3,
                GOF = "0.88",
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                NumViolations = 0,
                PrimaryViolation = "PV1"
            };
            checkvalueslist.Add(checkvalues1);
            checkvalueslist.Add(checkvalues2);
            checkvalueslist.Add(checkvalues3);
            var aggregation = new WaferAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.CreateNew(idWafAgg, "1", lotPADS, checkvalueslist);
            Assert.AreEqual("MotherlotWafer:3A110879:01:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }

        [TestMethod]
        public void TestUpdateExisting()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.WaferResources.TestCreateNewValidWafer);
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.Lotdoc);
            var existingDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PADSdoc);
            string idWafAgg = "MotherlotWafer:3A110879:01" +
                    $":SPACEAGGED1:RBG:FE:{spaceE4A.DataFlatMetaData.ParameterFacility}:" +
                    $"{spaceE4A.DataFlatMetaData.ParameterOper}:{spaceE4A.DataFlatMetaData.SPSNumber}:ProcessControl:1.0";
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>();
            var checkvalues1 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "Y",
                Value = 23.45,
                SampleId = 543245,
                Seqnr = 1,
                ItemIdMotherlotWafer = "3A110879:01",
                GOF = "0.88",
                ProcessEquipment = "543-123",
                NumViolations = 0,
                ViolationList = "1,2",
                PrimaryViolation = "2",
                PrimaryViolationComments = "sfgfhgfh",
                ViolationComments = "sdfdrfh"
            };
            var checkvalues2 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.345,
                SampleId = 543245,
                Seqnr = 2,
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                NumViolations = 0,
                ViolationList = "1,2",
                PrimaryViolation = "2",
                PrimaryViolationComments = "sffdgfuzzuggfhgfh",
                ViolationComments = "sdfwtrtzzdrfh"
            };
            var checkvalues3 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                IsFlagged = "N",
                ChannelId = "161310",
                Value = 23.455,
                SampleId = 543245,
                Seqnr = 3,
                GOF = "0.88",
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                NumViolations = 0,
                ViolationList = "1,3",
                PrimaryViolation = "3",
                PrimaryViolationComments = "sfgfdgfghfhgfh",
                ViolationComments = "sdffgfdgdrfh"
            };
            checkValueList.Add(checkvalues1);
            checkValueList.Add(checkvalues2);
            checkValueList.Add(checkvalues3);
            var aggregation = new WaferAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(idWafAgg, "01", lotPADS, existingDoc, checkValueList);
            Assert.AreEqual("MotherlotWafer:3A110879:01:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestUpdateExistingWaferambigiouscheck()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.WaferResources.TestCreateNewValidWafer);
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.Lotdocisambigioustrue);
            var existingDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PADSdoc);
            string idWafAgg = "MotherlotWafer:3A110879:01" +
                    $":SPACEAGGED1:RBG:FE:{spaceE4A.DataFlatMetaData.ParameterFacility}:" +
                    $"{spaceE4A.DataFlatMetaData.ParameterOper}:{spaceE4A.DataFlatMetaData.SPSNumber}:ProcessControl:1.0";
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>();
            var checkvalues1 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "Y",
                Value = 23.45,
                SampleId = 543245,
                Seqnr = 1,
                ItemIdMotherlotWafer = "3A110879:01",
                GOF = "0.88",
                ProcessEquipment = "543-123",
                NumViolations = 0
            };
            var checkvalues2 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.345,
                SampleId = 543245,
                Seqnr = 2,
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                NumViolations = 0
            };
            var checkvalues3 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.455,
                SampleId = 543245,
                Seqnr = 3,
                GOF = "0.88",
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                NumViolations = 0
            };
            checkValueList.Add(checkvalues1);
            checkValueList.Add(checkvalues2);
            checkValueList.Add(checkvalues3);
            var aggregation = new WaferAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(idWafAgg, "1", lotPADS, existingDoc, checkValueList);
            Assert.AreEqual("MotherlotWafer:3A110879:01:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestCreateMeasurementAggregates()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", ItemIdMotherlotWafer = "Lot:Wafer",
                    IsFlagged = "Y", Value = 3.0, GOF = "3.0",Slot ="24", WaferSequence = "27", TestPosition = "2",
                    InternalComment ="BlaBLa"},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 1.0, GOF = "1.0" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 2.0, GOF = "2.0"},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 2.0, GOF = "2.0"},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "OtherParameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "Y", Value = 2.0, GOF = "2.0"},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "OtherParameter", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 2.0, GOF = "2.0"}
            };
            var aggregates = WaferAggregation.CreateMeasurementAggregates("Parameter", "161310", rawValues);
            Assert.AreEqual(3, aggregates.ExecCount);
            Assert.AreEqual(3, aggregates.BaseCount);
            Assert.AreEqual(0, aggregates.FlaggedCount);
            Assert.AreEqual(1.0, aggregates.Min);
            Assert.AreEqual(2.0, aggregates.Max);
            Assert.AreEqual(1.6666666666666667, aggregates.Mean);
            Assert.AreEqual(1.0, aggregates.Range);
            Assert.AreEqual(0.4714045207910317, aggregates.Sigma);
            Assert.AreEqual(1.04, aggregates.Q2);
            Assert.AreEqual(1.1, aggregates.Q5);
            Assert.AreEqual(1.5, aggregates.Q25);
            Assert.AreEqual(2, aggregates.Median);
            Assert.AreEqual(2, aggregates.Q75);
            Assert.AreEqual(2, aggregates.Q95);
            Assert.AreEqual(2, aggregates.Q98);
            Assert.IsNull(aggregates.PrimaryViolation);
            Assert.IsNull(aggregates.PrimaryViolationComments);
            Assert.IsNull(aggregates.ViolationComments);
            Assert.IsNull(aggregates.ViolationList);
            Assert.AreEqual(0, aggregates.NumViolations);
            Assert.AreEqual(1.0, aggregates.GofMin);
            Assert.AreEqual(1.6666666666666667, aggregates.GofMean);
            Assert.AreEqual(2.0, aggregates.GofMax);
        }

        [TestMethod]
        public void TestCreateMeasurementAggregatesFlaggedOnly()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "Y", Value = 3.0, GOF = "3.0" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:OtherWafer", IsFlagged = "Y", Value = 3.0, GOF = "3.0" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "OtherParameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "Y", Value = 2.0, GOF = "2.0"}
            };
            var aggregates = WaferAggregation.CreateMeasurementAggregates("Parameter", "161310", rawValues);
            Assert.AreEqual(2, aggregates.ExecCount);
            Assert.AreEqual(0, aggregates.BaseCount);
            Assert.AreEqual(2, aggregates.FlaggedCount);
            Assert.IsNull(aggregates.Min);
            Assert.IsNull(aggregates.Max);
            Assert.IsNull(aggregates.Mean);
            Assert.IsNull(aggregates.Range);
            Assert.IsNull(aggregates.Sigma);
            Assert.IsNull(aggregates.Q2);
            Assert.IsNull(aggregates.Q5);
            Assert.IsNull(aggregates.Q25);
            Assert.IsNull(aggregates.Median);
            Assert.IsNull(aggregates.Q75);
            Assert.IsNull(aggregates.Q95);
            Assert.IsNull(aggregates.Q98);
            Assert.IsNull(aggregates.PrimaryViolation);
            Assert.IsNull(aggregates.PrimaryViolationComments);
            Assert.IsNull(aggregates.ViolationComments);
            Assert.IsNull(aggregates.ViolationList);
            Assert.AreEqual(0, aggregates.NumViolations);
            Assert.IsNull(aggregates.GofMin);
            Assert.IsNull(aggregates.GofMean);
            Assert.IsNull(aggregates.GofMax);
        }

        [TestMethod]
        public void TestCreateMeasurementAggregatesViolations()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "Y", Value = 1.0, ViolationList = "VL1", ViolationComments = "VC1", PrimaryViolation = "PV1", PrimaryViolationComments = "PVC1"},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 1.0 },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 2.0, ViolationList = "VL2", ViolationComments = "VC2", PrimaryViolation = "PV2", PrimaryViolationComments = "PVC2" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 3.0, ViolationList = "VL2", ViolationComments = "VC2", PrimaryViolation = "PV2", PrimaryViolationComments = "PVC2" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 3.0, ViolationList = "", ViolationComments = "", PrimaryViolation = "", PrimaryViolationComments = "" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 3.0, ViolationList = "VL3", ViolationComments = "VC3", PrimaryViolation = "PV3", PrimaryViolationComments = "PVC3" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "OtherParameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 3.0, ViolationList = "VL4", ViolationComments = "VC4", PrimaryViolation = "PV4", PrimaryViolationComments = "PVC4" }
            };
            var aggregates = WaferAggregation.CreateMeasurementAggregates("Parameter", "161310", rawValues);
            Assert.AreEqual(6, aggregates.ExecCount);
            Assert.AreEqual(5, aggregates.BaseCount);
            Assert.AreEqual(1, aggregates.FlaggedCount);
            Assert.AreEqual("PV1, PV2, PV3", aggregates.PrimaryViolation);
            Assert.AreEqual("PVC1, PVC2, PVC3", aggregates.PrimaryViolationComments);
            Assert.AreEqual("VC1, VC2, VC3", aggregates.ViolationComments);
            Assert.AreEqual("VL1, VL2, VL3", aggregates.ViolationList);
            Assert.AreEqual(3, aggregates.NumViolations);
        }
        [TestMethod]
        public void TestCreateData1ListRvStoreTrue()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "Y", Value = 3},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 9},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 4},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 2}
            };
            const string wafer = "Wafer";
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.LotDocRvstoreTrue);
            var data1List = WaferAggregation.CreateData1List(lotPADS, wafer, rawValues);
            Assert.AreEqual(data1List[0].ParameterName, "Parameter");
        }
        [TestMethod]
        public void TestCreateData1ListRvStoreFalse()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "Y", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize=9,ViolationList = "VL1", ViolationComments = "VC1", PrimaryViolation = "PV1", PrimaryViolationComments = "PVC1"},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize = 9,ViolationList = "VL2", ViolationComments = "VC2", PrimaryViolation = "PV2", PrimaryViolationComments = "PVC2"},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize=9,ViolationList = "VL1, VL3", ViolationComments = "VC1,VC3", PrimaryViolation = "PV1", PrimaryViolationComments = "PVC1"},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter1", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize=9}            };
            const string wafer = "Wafer";
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.LotDocRvstoreFalse);
            var data1List = WaferAggregation.CreateData1List(lotPADS, wafer, rawValues);
            Assert.AreEqual(data1List[0].ParameterName, "Parameter");
        }
        [TestMethod]
        public void TestUpdateData1ListRvStoreTrue()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "Y", Value = 3},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 9},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 4},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", Value = 2}
            };
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.LotDocRvstoreTrue);
            var padsDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PADSdocRvStoreTrue);
            var data1List = WaferAggregation.UpdateData1List(lotPADS, padsDoc, rawValues, "Wafer");
            Assert.AreEqual(data1List[0].ParameterName, "Parameter");
        }
        [TestMethod]
        public void TestUpdateData1ListRvStoreFalse()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "Y", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize=9},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize = 9},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize = 9},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize=9},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize=9}            };
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.LotDocRvstoreFalse);
            var padsDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PADSdocRvStoreFalse);
            var data1List = WaferAggregation.UpdateData1List(lotPADS, padsDoc, rawValues, "Wafer");
            Assert.AreEqual(data1List[0].ParameterName, "Parameter");
        }

        [TestMethod]
        public void TestUpdateData1ListExisting()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "Y", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize=9},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize = 9},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize = 9},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", ChannelId = "161310",ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize=9},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter",ChannelId = "161310", ItemIdMotherlotWafer = "Lot:Wafer", IsFlagged = "N", SampleId = 1, SampleMax = 3, SampleMin = 1,SampleMean = 2,SampleSize=9}
            };
            var padsDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PADSdoc);
            var dataList = new List<Data1ListPads>()
            {
                new Data1ListPads(){ParameterName = "WARPAGE",ChannelDescr ="drgg",ChannelId ="345543",ChannelName = "dgdfgdfh",RvStoreFlag="Y"}
            };
            WaferAggregation.UpdateData1ListExisting(dataList, padsDoc.Data1List[0], rawValues, "");
        }

        [TestMethod]
        public void TestUpdateData1ListNewRVTrue()
        {
            var padsDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PADSdoc);
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.LotDocRvstoreFalse);
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>()
            {  new Data1ListRawValuesPads4Wafer()
                {
                    ParameterName = "Parameter",
                    ChannelId = "161310",
                    IsFlagged = "Y",
                    Value = 23.45,
                    SampleId = 543245,
                    Seqnr = 1,
                    ItemIdMotherlotWafer = "3A110879:1",
                    GOF = "0.88",
                    ProcessEquipment = "543-123",
                    NumViolations = 0
                },
               new Data1ListRawValuesPads4Wafer() {
                    ParameterName = "Parameter",
                    ChannelId = "161310",
                    IsFlagged = "N",
                    Value = 23.345,
                    SampleId = 543245,
                    Seqnr = 2,
                    ItemIdMotherlotWafer = "3A110879:1",
                    ProcessEquipment = "543-123",
                    NumViolations = 0
                },
               new Data1ListRawValuesPads4Wafer() {
                    ParameterName = "Parameter",
                    ChannelId = "161310",
                    IsFlagged = "N",
                    Value = 23.455,
                    SampleId = 543245,
                    Seqnr = 3,
                    GOF = "0.88",
                    ItemIdMotherlotWafer = "3A110879:1",
                    ProcessEquipment = "543-123",
                    NumViolations = 0
                }
            };
            var lotData1List = new Data1ListPads() { ParameterName = "Parameter", ChannelDescr = "drgg", ChannelId = "161310", ChannelName = "dgdfgdfh", RvStoreFlag = "Y" };
            var dataList = new List<Data1ListPads>();
            const string wafer = "1";
            WaferAggregation.UpdateData1ListNew(lotPADS, padsDoc, checkValueList, dataList, wafer, lotData1List);
        }

        [TestMethod]
        public void TestUpdateData1ListNewRvf()
        {
            var padsDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PADSdoc);
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.LotDocRvstoreFalse);
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>()
            {  new Data1ListRawValuesPads4Wafer()
                {
                    ParameterName = "Parameter",
                    ChannelId = "161310",
                    IsFlagged = "Y",
                    Value = 23.45,
                    SampleId = 543245,
                    Seqnr = 1,
                    ItemIdMotherlotWafer = "3A110879:1",
                    GOF = "0.88",
                    ProcessEquipment = "543-123",
                    NumViolations = 0
                },
               new Data1ListRawValuesPads4Wafer() {
                    ParameterName = "Parameter",
                    ChannelId = "161310",
                    IsFlagged = "N",
                    Value = 23.345,
                    SampleId = 543245,
                    Seqnr = 2,
                    ItemIdMotherlotWafer = "3A110879:1",
                    ProcessEquipment = "543-123",
                    NumViolations = 0
                },
               new Data1ListRawValuesPads4Wafer() {
                    ParameterName = "Parameter",
                    ChannelId = "161310",
                    IsFlagged = "N",
                    Value = 23.455,
                    SampleId = 543245,
                    Seqnr = 3,
                    GOF = "0.88",
                    ItemIdMotherlotWafer = "3A110879:1",
                    ProcessEquipment = "543-123",
                    NumViolations = 0
                }
            };
            var lotData1List = new Data1ListPads() { ParameterName = "Parameter", ChannelDescr = "drgg", ChannelId = "161310", ChannelName = "dgdfgdfh", RvStoreFlag = "N" };
            var dataList = new List<Data1ListPads>();
            const string wafer = "1";
            WaferAggregation.UpdateData1ListNew(lotPADS, padsDoc, checkValueList, dataList, wafer, lotData1List);
        }

        [TestMethod]
        public void TestUpdateData1ListNewIsEqualT()
        {
            var padsDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.CheckDoc);
            var lotPADS = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.LotDocRvstoreFalse);
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>()
            {  new Data1ListRawValuesPads4Wafer()
                {
                    ParameterName = "WARPAGE",
                    ChannelId = "161310",
                    IsFlagged = "Y",
                    Value = 23.45,
                    SampleId = 543245,
                    Seqnr = 1,
                    ItemIdMotherlotWafer = "3A110879:1",
                    GOF = "0.88",
                    ProcessEquipment = "543-123",
                    NumViolations = 0
                },
               new Data1ListRawValuesPads4Wafer() {
                    ParameterName = "WARPAGE",
                    ChannelId = "161310",
                    IsFlagged = "N",
                    Value = 23.345,
                    SampleId = 543245,
                    Seqnr = 2,
                    ItemIdMotherlotWafer = "3A110879:1",
                    ProcessEquipment = "543-123",
                    NumViolations = 0
                },
               new Data1ListRawValuesPads4Wafer() {
                    ParameterName = "WARPAGE",
                    ChannelId = "161310",
                    IsFlagged = "N",
                    Value = 23.455,
                    SampleId = 543245,
                    Seqnr = 3,
                    GOF = "0.88",
                    ItemIdMotherlotWafer = "3A110879:1",
                    ProcessEquipment = "543-123",
                    NumViolations = 0
                }
            };
            var lotData1List = new Data1ListPads() { ParameterName = "Parameter", ChannelDescr = "drgg", ChannelId = "161310", ChannelName = "dgdfgdfh", RvStoreFlag = "N" };
            var dataList = new List<Data1ListPads>();
            const string wafer = "1";
            WaferAggregation.UpdateData1ListNew(lotPADS, padsDoc, checkValueList, dataList, wafer, lotData1List);
        }
    }
}
