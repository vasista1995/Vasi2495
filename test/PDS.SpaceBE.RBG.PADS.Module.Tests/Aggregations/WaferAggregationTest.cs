using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.Common.Utils;
using PDS.SpaceBE.RBG.PADS.Module.Aggregations;
using PDS.SpaceBE.RBG.PADS.Module.Data.PADSModel;
using System;
using System.Collections.Generic;
using Moq;
using PDS.Queue.Api.Message;
using PDS.SpaceBE.RBG.Common.Data.E4AModel;

namespace PDS.SpaceBE.RBG.PADS.Module.Tests.Aggregations
{
    /// <summary>
    /// Unit tests for <see cref="WaferAggregation"/>
    /// </summary>
    [TestClass]
    public class WaferAggregationTest
    {
        [TestMethod]
        public void TestCreateMeasurementAggregates()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "Wafer", IsFlagged = "Y", Value = 3.0 },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "Wafer", IsFlagged = "N", Value = 1.0 },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "Wafer", IsFlagged = "N", Value = 2.0 },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "OtherWafer", IsFlagged = "Y", Value = 2.0 },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "OtherParameter", WaferLot = "Wafer", IsFlagged = "Y", Value = 2.0 },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "OtherParameter", WaferLot = "Wafer", IsFlagged = "N", Value = 2.0 }
            };
            var aggregates = WaferAggregation.CreateMeasurementAggregates("Parameter", "Wafer", rawValues);
            Assert.AreEqual(4, aggregates.ExecCount);
            Assert.AreEqual(2, aggregates.BaseCount);
            Assert.AreEqual(2, aggregates.FlaggedCount);
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
            Assert.IsNull(aggregates.PrimaryViolationComments);
            Assert.IsNull(aggregates.ViolationComments);
            Assert.IsNull(aggregates.ViolationList);
            Assert.AreEqual(0, aggregates.NumViolations);
        }

        [TestMethod]
        public void TestUpdate1ListExistingParams()
        {
            var dataList = new List<Data1ListPads> {
                new Data1ListPads{ ParameterName = "WARPAGE"  },
                new Data1ListPads{ ParameterName = "WARPAGE2" }
            };
            var checkparams = new Data1ListPads { ParameterName = "WARPAGE" };
            WaferAggregation.UpdateData1ListExisting(dataList, checkparams);
        }

        [TestMethod]
        public void TestLoadMeasurementAggregates()
        {
            string parameterName = "WARPAGE";
            var rawvalues = new List<Data1ListRawValuesPads>
            {
                new Data1ListRawValuesPads{ WaferLot = "10", IsFlagged = "Y", SampleSize = 100},
                new Data1ListRawValuesPads{ WaferLot = "10", IsFlagged = "N", SampleSize = 100},
                new Data1ListRawValuesPads{ WaferLot = "10", IsFlagged = "N", SampleSize = 100},
                new Data1ListRawValuesPads{ WaferLot = "10", IsFlagged = "Y", SampleSize = 100}
            };
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer{ParameterName = "WARPAGE", WaferLot = "10"},
                new Data1ListRawValuesPads4Wafer{ParameterName = "WARPAGE2", WaferLot = "10"},
            };
            var measaggs = WaferAggregation.LoadMeasurementAggregates(parameterName, rawvalues, checkValueList);
            Assert.AreEqual(0, measaggs.Min);
        }

        [TestMethod]
        public void TestCreateMeasurementAggregatesFlaggedOnly()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "Wafer", IsFlagged = "Y", Value = 3.0 },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "OtherWafer", IsFlagged = "Y", Value = 2.0 },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "OtherParameter", WaferLot = "Wafer", IsFlagged = "Y", Value = 2.0 }
            };
            var aggregates = WaferAggregation.CreateMeasurementAggregates("Parameter", "Wafer", rawValues);
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
        }

        [TestMethod]
        public void TestCreateMeasurementAggregatesViolations()
        {
            var rawValues = new List<Data1ListRawValuesPads4Wafer>
            {
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "Wafer", IsFlagged = "Y", Value = 1.0, ViolationList = "VL1", ViolationComments = "VC1", PrimaryViolation = "PV1", PrimaryViolationComments = "PVC1"},
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "Wafer", IsFlagged = "N", Value = 1.0 },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "Wafer", IsFlagged = "N", Value = 2.0, ViolationList = "VL2", ViolationComments = "VC2", PrimaryViolation = "PV2", PrimaryViolationComments = "PVC2" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "Wafer", IsFlagged = "N", Value = 3.0, ViolationList = "VL2", ViolationComments = "VC2", PrimaryViolation = "PV2", PrimaryViolationComments = "PVC2" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "Wafer", IsFlagged = "N", Value = 3.0, ViolationList = "", ViolationComments = "", PrimaryViolation = "", PrimaryViolationComments = "" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "Parameter", WaferLot = "OtherWafer", IsFlagged = "N", Value = 3.0, ViolationList = "VL3", ViolationComments = "VC3", PrimaryViolation = "PV3", PrimaryViolationComments = "PVC3" },
                new Data1ListRawValuesPads4Wafer() { ParameterName = "OtherParameter", WaferLot = "Wafer", IsFlagged = "N", Value = 3.0, ViolationList = "VL4", ViolationComments = "VC4", PrimaryViolation = "PV4", PrimaryViolationComments = "PVC4" }
            };
            var aggregates = WaferAggregation.CreateMeasurementAggregates("Parameter", "Wafer", rawValues);
            Assert.AreEqual(6, aggregates.ExecCount);
            Assert.AreEqual(4, aggregates.BaseCount);
            Assert.AreEqual(2, aggregates.FlaggedCount);
            Assert.AreEqual("PV2", aggregates.PrimaryViolation);
            Assert.AreEqual("PVC2", aggregates.PrimaryViolationComments);
            Assert.AreEqual("VC2", aggregates.ViolationComments);
            Assert.AreEqual("VL2", aggregates.ViolationList);
            Assert.AreEqual(1, aggregates.NumViolations);
        }

        [TestMethod]
        public void TestCreateNew()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.WaferResources.TestCreateNew);
            var spacePads = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PadsDoc);
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>()
            {
                new Data1ListRawValuesPads4Wafer(){Seqnr = 0,SampleId = 219254689,Value= 0,IsFlagged = "N", ParameterName = "bumpdiameter_All_bumps",ParameterUnit = "µm",
                    SampleTimestamp = DateTime.Parse("2021-08-06T00:36:42.000Z"),SampleTimestampUtc = DateTime.Parse("2021-08-05T22:36:42.000Z"),
                    CreatedTimestamp = DateTime.Parse("0001-01-01T00:00:00.000Z"),CreatedTimestampUtc = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                    UpdatedTimestamp = DateTime.Parse("0001-01-01T00:00:00.000Z"),UpdatedTimestampUtc = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                    NumViolations= 0,SampleMean = 298.622,SampleMin = 275.246,SampleMax = 332.184,SampleSize = 162899,WaferLot = "22"},
            };
            var aggregation = new WaferAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newPadsDoc = aggregation.CreateNew("MotherlotWafer:3A140739:22:SPACEAGGED2:RBG:BE:HFICR::ProcessControl:1.0", "22", spacePads, checkValueList);
            Assert.AreEqual("MotherlotWafer:3A140739:22:SPACEAGGED2:RBG:BE:HFICR::ProcessControl:1.0", newPadsDoc.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestCreateNewRvStoreTrue()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.WaferResources.TestCreateNewRvStoreTrue);
            var spacePads = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PadsDocRvStoreTrue);
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>()
            {
                new Data1ListRawValuesPads4Wafer(){Seqnr = 0,SampleId = 219254689,Value= 0,IsFlagged = "N", ParameterName = "bumpdiameter_All_bumps",ParameterUnit = "µm",
                    SampleTimestamp = DateTime.Parse("2021-08-06T00:36:42.000Z"),SampleTimestampUtc = DateTime.Parse("2021-08-05T22:36:42.000Z"),
                    CreatedTimestamp = DateTime.Parse("0001-01-01T00:00:00.000Z"),CreatedTimestampUtc = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                    UpdatedTimestamp = DateTime.Parse("0001-01-01T00:00:00.000Z"),UpdatedTimestampUtc = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                    NumViolations= 0,SampleMean = 298.622,SampleMin = 275.246,SampleMax = 332.184,SampleSize = 162899,WaferLot = "22"},
            };
            var aggregation = new WaferAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newPadsDoc = aggregation.CreateNew("MotherlotWafer:3A140739:22:SPACEAGGED2:RBG:BE:HFICR::ProcessControl:1.0", "22", spacePads, checkValueList);
            Assert.AreEqual("MotherlotWafer:3A140739:22:SPACEAGGED2:RBG:BE:HFICR::ProcessControl:1.0", newPadsDoc.SearchPatterns.SpaceKey);
        }

        [TestMethod]
        public void TestUpdateExisting()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.WaferResources.TestCreateNewRvStoreTrue);
            var lotPads = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PadsDocRvStoreTrue);
            var padsDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.CheckDoc);
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>()
            {
                new Data1ListRawValuesPads4Wafer(){Seqnr = 0,SampleId = 219254689,Value= 0,IsFlagged = "N", ParameterName = "bumpdiameter_All_bumps",ParameterUnit = "µm",
                    SampleTimestamp = DateTime.Parse("2021-08-06T00:36:42.000Z"),SampleTimestampUtc = DateTime.Parse("2021-08-05T22:36:42.000Z"),
                    CreatedTimestamp = DateTime.Parse("0001-01-01T00:00:00.000Z"),CreatedTimestampUtc = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                    UpdatedTimestamp = DateTime.Parse("0001-01-01T00:00:00.000Z"),UpdatedTimestampUtc = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                    NumViolations= 0,SampleMean = 298.622,SampleMin = 275.246,SampleMax = 332.184,SampleSize = 162899,WaferLot = "22"},
            };
            var aggregation = new WaferAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newPadsDoc = aggregation.UpdateExisting("MotherlotWafer:3A140739:22:SPACEAGGED2:RBG:BE:HFICR::ProcessControl:1.0", "22", lotPads, padsDoc, checkValueList);
            Assert.AreEqual("MotherlotWafer:3A140739:22:SPACEAGGED2:RBG:BE:HFICR::ProcessControl:1.0", newPadsDoc.SearchPatterns.SpaceKey);
        }
        [TestMethod]
        public void TestUpdateNewRvStoreTrue()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.WaferResources.TestUpdateNewParam);
            var lotPads = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PadsDocUpdateNewParam);
            var padsDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.CheckDoc);
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>()
            {
                new Data1ListRawValuesPads4Wafer(){Seqnr = 0,SampleId = 219254689,Value= 0,IsFlagged = "N", ParameterName = "bumpdiameter_Some_bumps",ParameterUnit = "µm",
                    SampleTimestamp = DateTime.Parse("2021-08-06T00:36:42.000Z"),SampleTimestampUtc = DateTime.Parse("2021-08-05T22:36:42.000Z"),
                    CreatedTimestamp = DateTime.Parse("0001-01-01T00:00:00.000Z"),CreatedTimestampUtc = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                    UpdatedTimestamp = DateTime.Parse("0001-01-01T00:00:00.000Z"),UpdatedTimestampUtc = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                    NumViolations= 0,SampleMean = 298.622,SampleMin = 275.246,SampleMax = 332.184,SampleSize = 162899,WaferLot = "22"},
            };
            var aggregation = new WaferAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newPadsDoc = aggregation.UpdateExisting("MotherlotWafer:3A140739:22:SPACEAGGED2:RBG:BE:HFICR::ProcessControl:1.0", "22", lotPads, padsDoc, checkValueList);
            Assert.AreEqual("MotherlotWafer:3A140739:22:SPACEAGGED2:RBG:BE:HFICR::ProcessControl:1.0", newPadsDoc.SearchPatterns.SpaceKey);
        }

        [TestMethod]
        public void TestUpdatenewRvStorefalse()
        {
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.WaferResources.TestUpdateNewParam);
            var lotPads = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.PadsDocUpdateNewParamRvFalse);
            var padsDoc = JsonUtils.FromJson<SpacePads>(Properties.WaferResources.CheckDoc);
            var checkValueList = new List<Data1ListRawValuesPads4Wafer>()
            {
                new Data1ListRawValuesPads4Wafer(){Seqnr = 0,SampleId = 219254689,Value= 0,IsFlagged = "N", ParameterName = "bumpdiameter_Some_bumps",ParameterUnit = "µm",
                    SampleTimestamp = DateTime.Parse("2021-08-06T00:36:42.000Z"),SampleTimestampUtc = DateTime.Parse("2021-08-05T22:36:42.000Z"),
                    CreatedTimestamp = DateTime.Parse("0001-01-01T00:00:00.000Z"),CreatedTimestampUtc = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                    UpdatedTimestamp = DateTime.Parse("0001-01-01T00:00:00.000Z"),UpdatedTimestampUtc = DateTime.Parse("0001-01-01T00:00:00.000Z"),
                    NumViolations= 0,SampleMean = 298.622,SampleMin = 275.246,SampleMax = 332.184,SampleSize = 162899,WaferLot = "22"},
            };
            var aggregation = new WaferAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newPadsDoc = aggregation.UpdateExisting("MotherlotWafer:3A140739:22:SPACEAGGED2:RBG:BE:HFICR::ProcessControl:1.0", "22", lotPads, padsDoc, checkValueList);
            Assert.AreEqual("MotherlotWafer:3A140739:22:SPACEAGGED2:RBG:BE:HFICR::ProcessControl:1.0", newPadsDoc.SearchPatterns.SpaceKey);
        }
    }
}
