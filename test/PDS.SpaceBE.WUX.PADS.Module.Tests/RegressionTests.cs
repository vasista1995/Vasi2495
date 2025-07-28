using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.Common.Utils;
using PDS.Queue.Api.Message;
using PDS.SpaceBE.Common.PADS.Module.Tests;
using PDS.SpaceBE.WUX.Common.Data.E4AModel;
using System.IO;
using PDS.SpaceBE.WUX.PADS.Module.Aggregations;
using PDS.Core.Config;
using PDS.Queue.Api;
using PDS.SpaceBE.WUX.PADS.Module.Data;
using PDS.SpaceBE.WUX.PADS.Module.Data.PADSModel;

namespace PDS.SpaceBE.WUX.PADS.Module.Tests
{

    [TestClass]
    public class RegressionTests : RegressionTestsCommon
    {
        public override string GetSpacePadsJson(string sourceFilePath)
        {
            string sourceJson = File.ReadAllText(sourceFilePath);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(sourceJson);
            var aggregation = new LotAggregation(spaceE4A, new Mock<IQueueMessage>().Object);
            var newLotPads = aggregation.CreateNew();

            newLotPads.SystemLog.DocCreatedTimestampUtc = spaceE4A.DataFlatMetaData.SampleTimestamp;

            string spacePadsJson = JsonUtils.ToJson(newLotPads, indent: true);
            return spacePadsJson;
        }

        public override void TestCreateOperLotAggregates(string sourceFilePath, string expectedFilePath)
        {
            string expectedJson = File.ReadAllText(expectedFilePath);
            var expectedSpacePADS = JsonUtils.FromJson<SpacePads>(expectedJson);

            string sourceJson = File.ReadAllText(sourceFilePath);
            var appMock = new Mock<IPadsDao>();
            appMock.Setup(m => m.FindExistingDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(default(SpacePads));
            var queueMock = new Mock<IMessageQueueProvider>();
            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var sourceSpaceE4A = JsonUtils.FromJson<SpaceE4A>(sourceJson);
            var sourceSpacePADS = loader.CreateOperLotAggregates(sourceSpaceE4A, new Mock<IQueueMessage>().Object);

            Assert.AreEqual(expectedSpacePADS.SearchPatterns.SpaceKey, sourceSpacePADS.SearchPatterns.SpaceKey);
            appMock.Verify(m => m.InsertDoc(sourceSpacePADS), Times.Once);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SpacePads>()), Times.Never);
        }
    }
}
