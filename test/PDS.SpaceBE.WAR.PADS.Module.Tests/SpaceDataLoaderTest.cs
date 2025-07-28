using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.Common.Utils;
using PDS.SpaceBE.WAR.Common.Data.E4AModel;
using PDS.SpaceBE.WAR.PADS.Module.Data.PADSModel;
using PDS.Core.Config;
using Moq;
using PDS.SpaceBE.WAR.PADS.Module.Data;
using PDS.Queue.Api.Message;
using PDS.Queue.Api;

namespace PDS.SpaceBE.WAR.PADS.Module.Tests
{
    /// <summary>
    /// Unit tests for <see cref="SpaceDataLoader"/>
    /// </summary>
    [TestClass]
    public class SpaceDataLoaderTest
    {
        [TestMethod]
        public void TestCreateOperLotAggregatesNew()
        {
            var appMock = new Mock<IPadsDao>();
            appMock.Setup(m => m.FindExistingDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(default(SpacePads));

            var queueMock = new Mock<IMessageQueueProvider>();

            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNew);
            var spacePads = loader.CreateOperLotAggregates(spaceE4A, new Mock<IQueueMessage>().Object);
            Assert.AreEqual("Measlot:3A140739:SPACEAGGED3:WAR:BE:HD:Paste Printer::ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
            appMock.Verify(m => m.InsertDoc(spacePads), Times.Once);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SpacePads>()), Times.Never);
        }

        [TestMethod]
        public void TestCreateOperLotAggregatesUpdate()
        {
            var appMock = new Mock<IPadsDao>();
            var padsExisting = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.PadsDoc);
            appMock.Setup(m => m.FindExistingDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(padsExisting);

            var queueMock = new Mock<IMessageQueueProvider>();

            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNew);
            var spacePads = loader.CreateOperLotAggregates(spaceE4A, new Mock<IQueueMessage>().Object);
            Assert.AreEqual("Measlot:3A140739:SPACEAGGED3:WAR:BE:HD:Paste Printer::ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
            appMock.Verify(m => m.InsertDoc(spacePads), Times.Never);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), spacePads), Times.Once);
        }
    }
}
