using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.Common.Utils;
using PDS.SpaceBE.MKZ.Common.Data.E4AModel;
using PDS.SpaceBE.MKZ.PADS.Module.Data.PADSModel;
using PDS.Core.Config;
using Moq;
using PDS.SpaceBE.MKZ.PADS.Module;
using System.Collections.Generic;
using PDS.SpaceBE.MKZ.PADS.Module.Data;
using PDS.Queue.Api.Message;
using PDS.Queue.Api;

namespace PDS.SpaceBE.MKZ.PADS.Module.Tests
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
            Assert.AreEqual("Measlot:3A140739:SPACEAGGED2:MKZ:BE:HFICR:::ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
            appMock.Verify(m => m.InsertDoc(spacePads), Times.Once);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SpacePads>()), Times.Never);
        }
    }
}
