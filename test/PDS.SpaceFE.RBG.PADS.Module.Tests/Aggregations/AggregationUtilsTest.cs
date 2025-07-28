using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.Space.Common.Aggregations;

namespace PDS.SpaceFE.RBG.PADS.Module.Tests.Aggregations
{
    [TestClass]
    public class AggregationUtilsTest
    {
        [TestMethod]
        public void TestMinDatePadsRecord()
        {
            var padsRecord = new DateTime(2021, 3, 5, 9, 0, 0);
            var e4aRecord = new DateTime(2021, 3, 7, 10, 0, 0);
            var date = SpaceAggregationUtils.MinDate(padsRecord, e4aRecord);
            Assert.AreEqual(date, padsRecord);
        }

        [TestMethod]
        public void TestMinDateE4ARecord()
        {
            var padsRecord = new DateTime(2021, 3, 8, 9, 0, 0);
            var e4aRecord = new DateTime(2021, 3, 7, 10, 0, 0);
            var date = SpaceAggregationUtils.MinDate(padsRecord, e4aRecord);
            Assert.AreEqual(date, e4aRecord);
        }

        [TestMethod]
        public void TestMaxDatePadsRecord()
        {
            var padsRecord = new DateTime(2021, 3, 5, 9, 0, 0);
            var e4aRecord = new DateTime(2021, 3, 7, 10, 0, 0);
            var date = SpaceAggregationUtils.MaxDate(padsRecord, e4aRecord);
            Assert.AreEqual(date, e4aRecord);
        }

        [TestMethod]
        public void TestMaxDateE4ARecord()
        {
            var padsRecord = new DateTime(2021, 3, 8, 9, 0, 0);
            var e4aRecord = new DateTime(2021, 3, 7, 10, 0, 0);
            var date = SpaceAggregationUtils.MaxDate(padsRecord, e4aRecord);
            Assert.AreEqual(date, padsRecord);
        }

        [TestMethod]
        public void TestUpdateLimitPadsRecord()
        {
            var padsRecord = 500.0;
            var e4aRecord = 500.00001;
            var updatedLimit = SpaceAggregationUtils.UpdateLimit(padsRecord, e4aRecord);
            Assert.AreEqual(updatedLimit, padsRecord);
        }

        [TestMethod]
        public void TestUpdateLimitNull()
        {
            var padsRecord = 500;
            var e4aRecord = 300;
            var updatedLimit = SpaceAggregationUtils.UpdateLimit(padsRecord, e4aRecord);
            Assert.AreEqual(updatedLimit, null);
        }


        [TestMethod]
        public void TestUpdateLimitEnabledPadsRecord()
        {
            var padsRecord = "Y";
            var e4aRecord = "Y";
            var updatedLimitEnabled = SpaceAggregationUtils.UpdateLimitEnabled(padsRecord, e4aRecord);
            Assert.AreEqual(updatedLimitEnabled, padsRecord);
        }

        [TestMethod]
        public void TestUpdateLimitEnabledNull()
        {
            var padsRecord = "Y";
            var e4aRecord = "N";
            var updatedLimitEnabled = SpaceAggregationUtils.UpdateLimitEnabled(padsRecord, e4aRecord);
            Assert.AreEqual(updatedLimitEnabled, null);
        }

        [TestMethod]
        public void TestUpdateIsLimitAmbigousTrue()
        {
            var padsRecord = 500;
            var e4aRecord = 300;
            var updatedLimitEnabled = SpaceAggregationUtils.IsLimitAmbigous(padsRecord, e4aRecord);
            Assert.IsTrue(updatedLimitEnabled);
        }

        [TestMethod]
        public void TestUpdateIsLimitAmbigousFalse()
        {
            var padsRecord = 500;
            var e4aRecord = 500;
            var updatedLimitEnabled = SpaceAggregationUtils.IsLimitAmbigous(padsRecord, e4aRecord);
            Assert.IsFalse(updatedLimitEnabled);
        }

        [TestMethod]
        public void TestJoinStringPadsRecord()
        {
            var padsRecord = "Hello";
            var e4aRecord = "Hello";
            var jointString = SpaceAggregationUtils.JoinStrings(padsRecord, e4aRecord);
            Assert.AreEqual(jointString, padsRecord);
        }

        [TestMethod]
        public void TestJoinStringJointRecord()
        {
            var padsRecord = "Hello";
            var e4aRecord = "World!";
            var jointString = SpaceAggregationUtils.JoinStrings(padsRecord, e4aRecord);
            Assert.AreEqual(jointString, "Hello, World!");
        }

        [TestMethod]
        public void TestJoinStringnullE4aRecord()
        {
            var padsRecord = "Hello";
            var jointString = SpaceAggregationUtils.JoinStrings(padsRecord, null);
            Assert.AreEqual(jointString, "Hello");
        }

        [TestMethod]
        public void TestJoinStringnullPadsRecord()
        {
            var e4aString = "Hello";
            var jointString = SpaceAggregationUtils.JoinStrings(null, e4aString);
            Assert.AreEqual(jointString, "Hello");
        }
    }
}
