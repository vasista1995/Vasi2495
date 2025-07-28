using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.Common.Utils;
using PDS.Queue.Api.Message;
using PDS.SpaceBE.CEG.Common.Data.E4AModel;
using PDS.SpaceBE.CEG.PADS.Module.Aggregations;
using PDS.SpaceBE.CEG.PADS.Module.Data.PADSModel;
using System.Collections.Generic;
using Moq;

namespace PDS.SpaceBE.CEG.PADS.Module.Tests.Aggregations
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
                new Data1ListRawValuesPads() { IsFlagged = "Y", Value = 3.0 },
                new Data1ListRawValuesPads() { IsFlagged = "N", Value = 1.0 },
                new Data1ListRawValuesPads() { IsFlagged = "N", Value = 2.0 },
                new Data1ListRawValuesPads() { IsFlagged = "Y", Value = 4.0 },
                new Data1ListRawValuesPads() { IsFlagged = "Y", Value = 1.0 }
            };
            var aggregates = LotAggregation.CreateMeasurementAggregates(rawValues);
            Assert.AreEqual(5, aggregates.ExecCount);
            Assert.AreEqual(2, aggregates.BaseCount);
            Assert.AreEqual(3, aggregates.FlaggedCount);
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
        }

        [TestMethod]
        public void TestCreateMeasurementAggregatesFlaggedOnly()
        {
            var rawValues = new List<Data1ListRawValuesPads>
            {
                new Data1ListRawValuesPads() { IsFlagged = "Y", Value = 3.0 },
                new Data1ListRawValuesPads() { IsFlagged = "Y", Value = 3.0 }
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
        }

        [TestMethod]
        public void TestCreateMeasurementAggregatesViolations()
        {
            var rawValues = new List<Data1ListRawValuesPads>
            {
                new Data1ListRawValuesPads() { IsFlagged = "Y", Value = 1.0, ViolationList = "VL1", ViolationComments = "VC1", PrimaryViolation = "PV1", PrimaryViolationComments = "PVC1"},
                new Data1ListRawValuesPads() { IsFlagged = "N", Value = 1.0 },
                new Data1ListRawValuesPads() { IsFlagged = "N", Value = 2.0, ViolationList = "VL2", ViolationComments = "VC2", PrimaryViolation = "PV2", PrimaryViolationComments = "PVC2" },
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
            Assert.AreEqual("VL1, VL2", aggregates.ViolationList);
            Assert.AreEqual(2, aggregates.NumViolations);
        }

        [TestMethod]
        public void TestLoadMeasurementAggregatesViolations()
        {
            var rawValues = new List<Data1ListRawValuesPads>
            {
                new Data1ListRawValuesPads() { SampleSize=1000, IsFlagged = "Y", Value = 0, ViolationList = "VL1,VL2", ViolationComments = "VC1", PrimaryViolation = "PV1", PrimaryViolationComments = "PVC1"},
                new Data1ListRawValuesPads() { SampleSize=1000, IsFlagged = "N", Value = 0 },
                new Data1ListRawValuesPads() { SampleSize=1000, IsFlagged = "N", Value = 0, ViolationList = "VL2", ViolationComments = "VC2", PrimaryViolation = "PV2", PrimaryViolationComments = "PVC2" },
                new Data1ListRawValuesPads() { SampleSize=1000, IsFlagged = "N", Value = 0, ViolationList = "VL2", ViolationComments = "VC2", PrimaryViolation = "PV2", PrimaryViolationComments = "PVC2" },
                new Data1ListRawValuesPads() { SampleSize=1000, IsFlagged = "N", Value = 0, ViolationList = "", ViolationComments = "", PrimaryViolation = "", PrimaryViolationComments = "" }
            };
            var aggregates = LotAggregation.LoadMeasurementAggregates(rawValues);
            Assert.AreEqual(5000, aggregates.ExecCount);
            Assert.AreEqual(4000, aggregates.BaseCount);
            Assert.AreEqual(1000, aggregates.FlaggedCount);
            Assert.AreEqual("PV1, PV2", aggregates.PrimaryViolation);
            Assert.AreEqual("PVC1, PVC2", aggregates.PrimaryViolationComments);
            Assert.AreEqual("VC1, VC2", aggregates.ViolationComments);
            Assert.AreEqual("VL1, VL2", aggregates.ViolationList);
            Assert.AreEqual(2, aggregates.NumViolations);
        }

        [TestMethod]
        public void TestCheckExisitingRawValues()
        {
            var checkRawValues = new Data1ListRawValuesPads { SampleId = 12345, Value = 2.0, Seqnr = 2 };
            var e4aRawValues = new Data1ListRawValuesE4A { SampleId = 12345, Value = 2.0, Seqnr = 2 };
            bool isEqual = LotAggregation.CheckExisitingRawValues(e4aRawValues, false, checkRawValues);
            Assert.AreEqual(true, isEqual);
        }
        [TestMethod]
        public void TestCheckExisitingRawValues2()
        {
            var checkRawValues = new Data1ListRawValuesPads { SampleId = 12345, Value = 2.2, Seqnr = 2 };
            var e4aRawValues = new Data1ListRawValuesE4A { SampleId = 12345, Value = 2.0, Seqnr = 2 };
            bool isEqual = LotAggregation.CheckExisitingRawValues(e4aRawValues, false, checkRawValues);
            Assert.AreEqual(false, isEqual);
        }

        [TestMethod]
        public void TestCreateNew()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNew);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newLotPads = aggregation.CreateNew();
            Assert.AreEqual("HPSModFAUF:81771705:SPACEAGGED3:CEG:BE:62mm:system soldering::ProcessControl:1.0", newLotPads.SearchPatterns.SpaceKey);
        }

        [TestMethod]
        public void TestUpdateExisting()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNew);
            var checkDoc = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PadsDoc);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newLotPads = aggregation.UpdateExisting(checkDoc);
            Assert.AreEqual("HPSModFAUF:81771705:SPACEAGGED3:CEG:BE:62mm:system soldering::ProcessControl:1.0", newLotPads.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestUpdateNewParamRvStoreFalse()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestUpdateNewParamRvStoreFalse);
            var checkDoc = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PadsDoc);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newLotPads = aggregation.UpdateExisting(checkDoc);
            Assert.AreEqual("HPSModFAUF:81771705:SPACEAGGED3:CEG:BE:62mm:system soldering::ProcessControl:1.0", newLotPads.SearchPatterns.SpaceKey);
        }

        [TestMethod]
        public void TestCreateNewRvStoreTrue()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewRvStoreTrue);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newLotPads = aggregation.CreateNew();
            Assert.AreEqual("HPSModFAUF:81771705:SPACEAGGED3:CEG:BE:62mm:system soldering::ProcessControl:1.0", newLotPads.SearchPatterns.SpaceKey);
        }

        [TestMethod]
        public void TestUpdateExistingRvStoreTrue()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewRvStoreTrue);
            var checkDoc = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PadsDocRvStoreTrue);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newLotPads = aggregation.UpdateExisting(checkDoc);
            Assert.AreEqual("HPSModFAUF:81771705:SPACEAGGED3:CEG:BE:62mm:system soldering::ProcessControl:1.0", newLotPads.SearchPatterns.SpaceKey);
            Assert.AreEqual("ProcessControl", newLotPads.Document.Type);
            Assert.AreEqual(null, newLotPads.Id);
        }
        [TestMethod]
        public void TestUpdateNewParamRvStoreTrue()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestUpdateNewParamRvStoreTrue);
            var checkDoc = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PadsDocRvStoreTrue);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newLotPads = aggregation.UpdateExisting(checkDoc);
            Assert.AreEqual("HPSModFAUF:81771705:SPACEAGGED3:CEG:BE:62mm:system soldering::ProcessControl:1.0", newLotPads.SearchPatterns.SpaceKey);
        }

        [TestMethod]
        public void TestUpdateExistingRvStoreFalse()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewRvStoreFalse);
            var checkDoc = JsonUtils.FromJson<SpacePads>(Properties.LotResources.PadsDocRvStoreFalse);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newLotPads = aggregation.UpdateExisting(checkDoc);
            Assert.AreEqual("HPSModFAUF:81771705:SPACEAGGED3:CEG:BE:62mm:system soldering::ProcessControl:1.0", newLotPads.SearchPatterns.SpaceKey);
            Assert.AreEqual("ProcessControl", newLotPads.Document.Type);
            Assert.AreEqual(null, newLotPads.Id);
        }
        [TestMethod]
        public void TestCreateRvStoreFalse()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.LotResources.TestCreateNewRvStoreFalse);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newLotPads = aggregation.CreateNew();
            Assert.AreEqual("HPSModFAUF:81771705:SPACEAGGED3:CEG:BE:62mm:system soldering::ProcessControl:1.0", newLotPads.SearchPatterns.SpaceKey);
            Assert.AreEqual("ProcessControl", newLotPads.Document.Type);
            Assert.AreEqual(null, newLotPads.Id);
        }
    }
}
