using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.Common.Utils;
using PDS.SpaceBE.RBG.Common.Data.E4AModel;
using PDS.SpaceBE.RBG.PADS.Module.Data.PADSModel;
using PDS.Core.Config;
using Moq;
using PDS.SpaceBE.RBG.PADS.Module.Data;
using System.Collections.Generic;
using PDS.Queue.Api.Message;
using PDS.Queue.Api;

namespace PDS.SpaceBE.RBG.PADS.Module.Tests
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
            Assert.AreEqual("Measlot:3A140739:SPACEAGGED2:RBG:BE:HFICR:::ProcessControl:1.0",
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
            Assert.AreEqual("Measlot:3A140739:SPACEAGGED2:RBG:BE:HFICR:::ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
            appMock.Verify(m => m.InsertDoc(spacePads), Times.Never);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), spacePads), Times.Once);
        }
        [TestMethod]
        public void TestCreateOperWaferAggregatesNewValidWaferCheckListNull()
        {
            var appMock = new Mock<IPadsDao>();
            appMock.Setup(m => m.FindExistingDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(default(SpacePads));

            var queueMock = new Mock<IMessageQueueProvider>();

            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNew);
            var spacePADS = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.PadsDoc);
            var checkvalueslist = new List<Data1ListRawValuesPads4Wafer>();
            appMock.Setup(m => m.FindExistingWafDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(checkvalueslist);
            var wafdoc = loader.CreateOperWaferAggregates(spaceE4A, spacePADS, new Mock<IQueueMessage>().Object);
            appMock.Verify(m => m.InsertDoc(wafdoc), Times.Never);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), wafdoc), Times.Never);
        }

        [TestMethod]
        public void TestCreateOperWaferAggregatesNewValidWafer()
        {
            var appMock = new Mock<IPadsDao>();
            appMock.Setup(m => m.FindExistingDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(default(SpacePads));
            var checkvalueslist = new List<Data1ListRawValuesPads4Wafer>();
            var checkvalues1 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "bumpdiameter_All_bumps",
                IsFlagged = "Y",
                Value = 23.45,
                SampleId = 543245,
                Seqnr = 1,
                WaferLot = "15"
            };
            var checkvalues2 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "bumpdiameter_All_bumps",
                IsFlagged = "N",
                Value = 23.345,
                SampleId = 543245,
                Seqnr = 2,
                WaferLot = "22"
            };
            var checkvalues3 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "bumpdiameter_All_bumps",
                IsFlagged = "N",
                Value = 23.455,
                SampleId = 543245,
                Seqnr = 3,
                WaferLot = "24"
            };
            checkvalueslist.Add(checkvalues1);
            checkvalueslist.Add(checkvalues2);
            checkvalueslist.Add(checkvalues3);
            appMock.Setup(m => m.FindExistingWafDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(checkvalueslist);

            var queueMock = new Mock<IMessageQueueProvider>();

            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNew);
            var spacePADS = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.PadsDoc);
            var wafdoc = loader.CreateOperWaferAggregates(spaceE4A, spacePADS, new Mock<IQueueMessage>().Object);
            appMock.Verify(m => m.InsertDoc(wafdoc), Times.Once);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), wafdoc), Times.Never);
        }

        [TestMethod]
        public void TestCreateOperWaferAggregatesNewInvalidWafer()
        {
            var appMock = new Mock<IPadsDao>();
            appMock.Setup(m => m.FindExistingDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(default(SpacePads));

            var queueMock = new Mock<IMessageQueueProvider>();

            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNewInvalidWafer);
            var spacePADS = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.PadsDocRvStoreTrue);
            var wafdoc = loader.CreateOperWaferAggregates(spaceE4A, spacePADS, new Mock<IQueueMessage>().Object);
            appMock.Verify(m => m.InsertDoc(wafdoc), Times.Never);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), wafdoc), Times.Never);
        }
        [TestMethod]
        public void TestCreateOperWaferAggregatesNewNullWafer()
        {
            var appMock = new Mock<IPadsDao>();
            appMock.Setup(m => m.FindExistingDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(default(SpacePads));

            var queueMock = new Mock<IMessageQueueProvider>();

            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNewNullWafer);
            var spacePADS = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.PadsDocRvStoreTrue);
            var wafdoc = loader.CreateOperWaferAggregates(spaceE4A, spacePADS, new Mock<IQueueMessage>().Object);
            appMock.Verify(m => m.InsertDoc(wafdoc), Times.Never);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), wafdoc), Times.Never);
        }

        [TestMethod]
        public void TestCreateOperWaferAggregatesUpdate()
        {
            var appMock = new Mock<IPadsDao>();
            appMock.Setup(m => m.FindExistingDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(JsonUtils.FromJson<SpacePads>(Properties.CommonResources.PadsDocExis));
            var checkvalueslist = new List<Data1ListRawValuesPads4Wafer>();
            var checkvalues1 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "bumpdiameter_All_bumps",
                IsFlagged = "Y",
                Value = 23.45,
                SampleId = 543245,
                Seqnr = 1,
                WaferLot = "24",
                ParameterUnit = "s"
            };
            var checkvalues2 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "bumpdiameter_All_bumps",
                IsFlagged = "N",
                Value = 23.345,
                SampleId = 543245,
                Seqnr = 2,
                WaferLot = "22",
                ParameterUnit = "s"
            };
            var checkvalues3 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "bumpdiameter_All_bumps",
                IsFlagged = "N",
                Value = 23.455,
                SampleId = 543245,
                Seqnr = 3,
                WaferLot = "15",
                ParameterUnit = "s"
            };
            checkvalueslist.Add(checkvalues1);
            checkvalueslist.Add(checkvalues2);
            checkvalueslist.Add(checkvalues3);
            appMock.Setup(m => m.FindExistingWafDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(checkvalueslist);

            var queueMock = new Mock<IMessageQueueProvider>();

            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNew);
            var spacePADS = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.PadsDoc);
            var wafdoc = loader.CreateOperWaferAggregates(spaceE4A, spacePADS, new Mock<IQueueMessage>().Object);
            appMock.Verify(m => m.InsertDoc(wafdoc), Times.Never);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), wafdoc), Times.Once);
        }
    }
}
