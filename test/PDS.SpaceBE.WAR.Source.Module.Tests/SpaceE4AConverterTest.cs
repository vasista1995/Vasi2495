using System;
using FluentAssertions.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using PDS.Common.ExtractionLog;
using PDS.Common.Utils;
using PDS.SpaceBE.WAR.Source.Module.Data.SpaceModel;

namespace PDS.SpaceBE.WAR.Source.Module.Tests
{
    [TestClass]
    public class SpaceE4AConverterTest
    {
        [TestMethod]
        public void TestGetSourceDataLevel()
        {
            //read source record from JSON file
            var spaceEntryRVT = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord9);
            var spaceEntryRVF = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord8);
            Assert.AreEqual("C", SpaceE4AConverter.CreateSourceDataLevel(spaceEntryRVT, "idSource"));
            Assert.AreEqual("L", SpaceE4AConverter.CreateSourceDataLevel(spaceEntryRVF, "idSource"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Not Valid RVStoreFlag: N, Y, corresponding IdSource is ")]
        public void TestGetSourceDataLevelException()
        {
            var spaceEntryRVTF = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord10);
            SpaceE4AConverter.CreateSourceDataLevel(spaceEntryRVTF, "idSource");
        }
        [TestMethod]
        public void TestConvert()
        {
            //read source record from JSON file
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord1);
            //var spaceConverter = new SpaceE4AConverter();

            //Convert source record to E4A document
            var spaceE4 = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            //Adjust dynamic time properties to fixed value defined in expected E4A JSON
            var exportedTime = new DateTime(2021, 9, 15);
            spaceE4.DataFlatMetaData.ExportedTimestamp = exportedTime;
            spaceE4.DataFlatMetaData.ExportedTimestampUtc = exportedTime;

            //Check equality of expected and actual E4A JSON
            var actualJson = JToken.Parse(JsonUtils.ToJson(spaceE4));
            var expectedJson = JToken.Parse(Properties.Resources.ExpectedE4a1);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }

        [TestMethod]
        public void TestConvert2()
        {
            //read source record from JSON file
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord2);
            //var spaceConverter = new SpaceE4AConverter();

            //Convert source record to E4A document
            var spaceE4 = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            //Adjust dynamic time properties to fixed value defined in expected E4A JSON
            var exportedTime = new DateTime(2021, 9, 15);
            spaceE4.DataFlatMetaData.ExportedTimestamp = exportedTime;
            spaceE4.DataFlatMetaData.ExportedTimestampUtc = exportedTime;

            //Check equality of expected and actual E4A JSON
            var actualJson = JToken.Parse(JsonUtils.ToJson(spaceE4));
            var expectedJson = JToken.Parse(Properties.Resources.ExpectedE4a2);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }
        [TestMethod]
        public void TestConvert3()
        {
            //read source record from JSON file
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord3);
            //var spaceConverter = new SpaceE4AConverter();

            //Convert source record to E4A document
            var spaceE4 = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            //Adjust dynamic time properties to fixed value defined in expected E4A JSON
            var exportedTime = new DateTime(2021, 9, 15);
            spaceE4.DataFlatMetaData.ExportedTimestamp = exportedTime;
            spaceE4.DataFlatMetaData.ExportedTimestampUtc = exportedTime;

            //Check equality of expected and actual E4A JSON
            var actualJson = JToken.Parse(JsonUtils.ToJson(spaceE4));
            var expectedJson = JToken.Parse(Properties.Resources.ExpectedE4a3);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }

        [TestMethod]
        public void TestConvert4()
        {
            //read source record from JSON file
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord4);
            //var spaceConverter = new SpaceE4AConverter();

            //Convert source record to E4A document
            var spaceE4 = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            //Adjust dynamic time properties to fixed value defined in expected E4A JSON
            var exportedTime = new DateTime(2021, 9, 15);
            spaceE4.DataFlatMetaData.ExportedTimestamp = exportedTime;
            spaceE4.DataFlatMetaData.ExportedTimestampUtc = exportedTime;

            //Check equality of expected and actual E4A JSON
            var actualJson = JToken.Parse(JsonUtils.ToJson(spaceE4));
            var expectedJson = JToken.Parse(Properties.Resources.ExpectedE4a4);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }

        [TestMethod]
        public void TestConvert5()
        {
            //read source record from JSON file
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord5);
            //var spaceConverter = new SpaceE4AConverter();

            //Convert source record to E4A document
            var spaceE4 = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            //Adjust dynamic time properties to fixed value defined in expected E4A JSON
            var exportedTime = new DateTime(2021, 9, 15);
            spaceE4.DataFlatMetaData.ExportedTimestamp = exportedTime;
            spaceE4.DataFlatMetaData.ExportedTimestampUtc = exportedTime;

            //Check equality of expected and actual E4A JSON
            var actualJson = JToken.Parse(JsonUtils.ToJson(spaceE4));
            var expectedJson = JToken.Parse(Properties.Resources.ExpectedE4a5);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }

        [TestMethod]
        public void TestConvert6()
        {
            //read source record from JSON file
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord6);
            //var spaceConverter = new SpaceE4AConverter();

            //Convert source record to E4A document
            var spaceE4 = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            //Adjust dynamic time properties to fixed value defined in expected E4A JSON
            var exportedTime = new DateTime(2021, 9, 15);
            spaceE4.DataFlatMetaData.ExportedTimestamp = exportedTime;
            spaceE4.DataFlatMetaData.ExportedTimestampUtc = exportedTime;

            //Check equality of expected and actual E4A JSON
            var actualJson = JToken.Parse(JsonUtils.ToJson(spaceE4));
            var expectedJson = JToken.Parse(Properties.Resources.ExpectedE4a6);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }

        [TestMethod]
        public void TestConvert7()
        {
            //read source record from JSON file
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord7);
            //var spaceConverter = new SpaceE4AConverter();

            //Convert source record to E4A document
            var spaceE4 = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            //Adjust dynamic time properties to fixed value defined in expected E4A JSON
            var exportedTime = new DateTime(2021, 9, 15);
            spaceE4.DataFlatMetaData.ExportedTimestamp = exportedTime;
            spaceE4.DataFlatMetaData.ExportedTimestampUtc = exportedTime;

            //Check equality of expected and actual E4A JSON
            var actualJson = JToken.Parse(JsonUtils.ToJson(spaceE4));
            var expectedJson = JToken.Parse(Properties.Resources.ExpectedE4a7);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }

        [TestMethod]
        public void TestConvert8()
        {
            //read source record from JSON file
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord8);
            //var spaceConverter = new SpaceE4AConverter();

            //Convert source record to E4A document
            var spaceE4 = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            //Adjust dynamic time properties to fixed value defined in expected E4A JSON
            var exportedTime = new DateTime(2021, 9, 15);
            spaceE4.DataFlatMetaData.ExportedTimestamp = exportedTime;
            spaceE4.DataFlatMetaData.ExportedTimestampUtc = exportedTime;

            //Check equality of expected and actual E4A JSON
            var actualJson = JToken.Parse(JsonUtils.ToJson(spaceE4));
            var expectedJson = JToken.Parse(Properties.Resources.ExpectedE4a8);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }

        [TestMethod]
        public void TestConvert9()
        {
            //read source record from JSON file
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord9);
            //var spaceConverter = new SpaceE4AConverter();

            //Convert source record to E4A document
            var spaceE4 = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            //Adjust dynamic time properties to fixed value defined in expected E4A JSON
            var exportedTime = new DateTime(2021, 9, 15);
            spaceE4.DataFlatMetaData.ExportedTimestamp = exportedTime;
            spaceE4.DataFlatMetaData.ExportedTimestampUtc = exportedTime;

            //Check equality of expected and actual E4A JSON
            var actualJson = JToken.Parse(JsonUtils.ToJson(spaceE4));
            var expectedJson = JToken.Parse(Properties.Resources.ExpectedE4a9);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }
    }
}
