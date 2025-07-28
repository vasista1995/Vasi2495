using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceFE.RBG.PADS.Module.Aggregations;
using PDS.Common.Utils;
using PDS.SpaceFE.RBG.Common.Data.E4AModel;
using PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel;
using System.Collections.Generic;
using Moq;
using PDS.Queue.Api.Message;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceFE.RBG.PADS.Module.Tests.Aggregations
{
    /// <summary>
    /// Unit tests for <see cref="LotAggregation"/>
    /// </summary>
    [TestClass]
    public class LotAggregationTest
    {
        [TestMethod]
        public void TestCreateMeasurementAggregates()
        {
            var rawValues = new List<Data1ListRawValuesPads>
            {
                new Data1ListRawValuesPads() { IsFlagged = "Y", Value = 3.0, GOF = "3.0" },
                new Data1ListRawValuesPads() { IsFlagged = "N", Value = 1.0, GOF = "1.0" },
                new Data1ListRawValuesPads() { IsFlagged = "N", Value = 2.0, GOF = "2.0" }
            };
            var aggregates = LotAggregation.CreateMeasurementAggregates(rawValues);
            Assert.AreEqual(3, aggregates.ExecCount);
            Assert.AreEqual(2, aggregates.BaseCount);
            Assert.AreEqual(1, aggregates.FlaggedCount);
            Assert.AreEqual(1.0, aggregates.Min);
            Assert.AreEqual(2.0, aggregates.Max);
            Assert.AreEqual(1.5, aggregates.Mean);
            Assert.AreEqual(1.0, aggregates.Range);
            Assert.AreEqual(0.5, aggregates.Sigma);
            Assert.AreEqual(1.02, aggregates.Q2);
            Assert.AreEqual(1.05, aggregates.Q5);
            Assert.AreEqual(1.25, aggregates.Q25);
            Assert.AreEqual(1.5, aggregates.Median);
            Assert.AreEqual(1.75, aggregates.Q75);
            Assert.AreEqual(1.95, aggregates.Q95);
            Assert.AreEqual(1.98, aggregates.Q98);
            Assert.IsNull(aggregates.PrimaryViolation);
            Assert.AreEqual(0, aggregates.NumViolations);
            Assert.IsNull(aggregates.PrimaryViolationComments);
            Assert.IsNull(aggregates.ViolationComments);
            Assert.IsNull(aggregates.ViolationList);
            Assert.AreEqual(1.0, aggregates.GofMin);
            Assert.AreEqual(1.5, aggregates.GofMean);
            Assert.AreEqual(2.0, aggregates.GofMax);
        }
        [TestMethod]
        public void TestCreateMeasurementAggregatesFlaggedOnly()
        {
            var rawValues = new List<Data1ListRawValuesPads>
            {
                new Data1ListRawValuesPads() { IsFlagged = "Y", Value = 3.0, GOF = "3.0" },
                new Data1ListRawValuesPads() { IsFlagged = "Y", Value = 3.0, GOF = "3.0" }
            };
            var aggregates = LotAggregation.CreateMeasurementAggregates(rawValues);
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
            Assert.AreEqual(0, aggregates.NumViolations);
            Assert.IsNull(aggregates.PrimaryViolationComments);
            Assert.IsNull(aggregates.ViolationComments);
            Assert.IsNull(aggregates.ViolationList);
            Assert.IsNull(aggregates.GofMin);
            Assert.IsNull(aggregates.GofMean);
            Assert.IsNull(aggregates.GofMax);
        }
        [TestMethod]
        public void TestCreateMeasurementAggregatesViolations()
        {
            var rawValues = new List<Data1ListRawValuesPads>
            {
                new Data1ListRawValuesPads() { IsFlagged = "Y", Value = 1.0, ViolationList = "VL1", ViolationComments = "VC1", PrimaryViolation = "PV1", PrimaryViolationComments = "PVC1"},
                new Data1ListRawValuesPads() { IsFlagged = "N", Value = 1.0 },
                new Data1ListRawValuesPads() { IsFlagged = "N", Value = 2.0, ViolationList = "VL2,VL3", ViolationComments = "VC2", PrimaryViolation = "PV2", PrimaryViolationComments = "PVC2" },
                new Data1ListRawValuesPads() { IsFlagged = "N", Value = 3.0, ViolationList = "VL2", ViolationComments = "VC2", PrimaryViolation = "PV2", PrimaryViolationComments = "PVC2" },
                new Data1ListRawValuesPads() { IsFlagged = "N", Value = 3.0, ViolationList = "", ViolationComments = "", PrimaryViolation = "", PrimaryViolationComments = "" }
            };
            var aggregates = LotAggregation.CreateMeasurementAggregates(rawValues);
            Assert.AreEqual(5, aggregates.ExecCount);
            Assert.AreEqual(4, aggregates.BaseCount);
            Assert.AreEqual(1, aggregates.FlaggedCount);
            Assert.AreEqual("PV1, PV2", aggregates.PrimaryViolation);
            Assert.AreEqual("PVC1, PVC2", aggregates.PrimaryViolationComments);
            Assert.AreEqual("VC1, VC2", aggregates.ViolationComments);
            Assert.AreEqual("VL1, VL2, VL3", aggregates.ViolationList);
            Assert.AreEqual(3, aggregates.NumViolations);
        }
        [TestMethod]
        public void TestCreateNew()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewValidWafer);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.CreateNew();
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
            Assert.AreEqual("Rohdatensammel    karte RADAR", spacePads.Data1List[0].ChannelDescr);
        }
        [TestMethod]
        public void TestCreateNewInvalidWafer()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewInValidWafer);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.CreateNew();
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestCheckExisitingRawValues()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewInvalidrawvalues);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.CreateNew();
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestUpdateNewParam()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewInValidWafer);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdoc);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestUpdateExisting()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestUpdateValidWafer);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdoc);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestUpdateambiguitytrueckc0()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestUpdateambiguitytrueckc0);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdoc);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestUpdateambiguityfalse()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestUpdateambiguityfalse);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdoc);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestUpdateInvalidRawvalues()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestUpdateInvalidRawvalues);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdocInvalidRawvalues);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestUpdateLimitambigiousckc0()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestUpdateInvalidRawvalues);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdocInvalidRawvalues);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
            Assert.AreEqual("Y", spacePads.Data1List[0].DataFlatLimits.ControlLimits.RemovalDue2Ambiguity);
        }
        [TestMethod]
        public void TestRawValuesifallequal()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestUpdateSameRawvalues);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdocSameRawvalues);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
            Assert.AreEqual("Y", spacePads.Data1List[0].DataFlatLimits.ControlLimits.RemovalDue2Ambiguity);
        }
        [TestMethod]
        public void TestRawValuesifallnotequal()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestUpdateNotSameRawvalues);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdocNotSameRawvalues);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
            Assert.AreEqual("Y", spacePads.Data1List[0].DataFlatLimits.ControlLimits.RemovalDue2Ambiguity);
        }
        [TestMethod]
        public void TestLoadMeasurementAggregatesViolations()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewNoMeasurementAggregs);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdocNoMeasurementAggregs);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestIfParameterNullPadsDoc()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewNoMeasurementAggregs);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdocNotSameParam);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestviolationsDoc()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewViolations);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdocwithViolations);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void Testviolationswithoutmeas()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewViolationswithoutmeas);
            var existingSpacePads = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdocwithViolationswithoutmeas);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var spacePads = aggregation.UpdateExisting(existingSpacePads);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestCreateData1ListRvStoreFalse()
        {
            var lotPADS = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewRvStoreFalse);
            var data1List = LotAggregation.CreateData1list(lotPADS);
            Assert.AreEqual(data1List[0].ParameterName, "WARPAGE");
        }
        [TestMethod]
        public void TestUpdateData1ListRvStoreFalse()
        {
            var lotPADS = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewRvStoreFalse);
            var data1List = LotAggregation.UpdateNewData1list(lotPADS);
            Assert.AreEqual(data1List.ParameterName, "WARPAGE");
        }
        [TestMethod]
        public void TestUpdateExistingData1ListRvStoreFalse()
        {
            var checkDataList = new Data1ListPads
            {
                ParameterName = "WARPAGE",
                ParameterUnit = "s",
                ProcessEquipment = "509-681",
                ChannelId = "161310",
                RvStoreFlag = "N",
                Data1ListRawValues = new List<Data1ListRawValuesPads>()
                {
                    new Data1ListRawValuesPads() { Value = 2, Seqnr = 1, SampleId = 123, IsFlagged = "Y",
                        ViolationList ="2, 3",ViolationComments = "sgtsdg",PrimaryViolation = "3",PrimaryViolationComments = "qww2e" },
                    new Data1ListRawValuesPads() { Value = 2, Seqnr = 2, SampleId = 1234, IsFlagged = "N"
                        },
                    new Data1ListRawValuesPads() { Value = 1, Seqnr = 1, SampleId = 1234, IsFlagged = "N",
                        ViolationList ="1",ViolationComments = "sgtsdg",PrimaryViolation = "3",PrimaryViolationComments = "qww2e" },
                    new Data1ListRawValuesPads() { Value = 3, Seqnr = 3, SampleId = 1234, IsFlagged = "N" },
                    new Data1ListRawValuesPads() { Value = 4, Seqnr = 4, SampleId = 1234, IsFlagged = "N"}
                },
                DataFlatLimits = new DataFlatLimitsPads()
                {
                    CKCId = "0",
                    MeasurementSpecLimits = new MeasurementSpecLimitsPads()
                    {
                        SpecHigh = 234,
                        SpecLow = 120,
                        SpecTarget = 170
                    },
                    ControlLimits = new ControlLimitsPads()
                    {
                        CntrlHigh = 12,
                        CntrlLow = 10
                    }
                }
            };
            var lotPADS = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewRvStoreFalse);
            var data1List = LotAggregation.UpdateOldData1list(lotPADS,checkDataList);
            Assert.AreEqual(data1List.ParameterName, "WARPAGE");
        }
    }
}
