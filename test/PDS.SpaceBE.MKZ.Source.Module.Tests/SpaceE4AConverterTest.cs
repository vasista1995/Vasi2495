using System;
using FluentAssertions.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using PDS.Common.ExtractionLog;
using PDS.Common.Utils;
using PDS.SpaceBE.MKZ.Source.Module.Data.SpaceModel;

namespace PDS.SpaceBE.MKZ.Source.Module.Tests
{
    [TestClass]
    public class SpaceE4AConverterTest
    {
        [TestMethod]
        public void TestConvert()
        {
            //read source record from JSON file
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(Properties.Resources.SourceRecord1);
            //var spaceConverter = new SpaceE4AConverter();

            //Convert source record to E4A document
            var spaceE4a = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            //Adjust dynamic time properties to fixed value defined in expected E4A JSON
            var exportedTime = new DateTime(2021, 9, 15);
            spaceE4a.DataFlatMetaData.ExportedTimestamp = exportedTime;
            spaceE4a.DataFlatMetaData.ExportedTimestampUtc = exportedTime;

            //Check equality of expected and actual E4A JSON
            var actualJson = JToken.Parse(JsonUtils.ToJson(spaceE4a));
            var expectedJson = JToken.Parse(Properties.Resources.ExpectedE4a1);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }
    }
}
