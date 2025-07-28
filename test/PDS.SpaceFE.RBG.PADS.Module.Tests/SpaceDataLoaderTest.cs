using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.Common.Utils;
using PDS.SpaceFE.RBG.Common.Data.E4AModel;
using PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel;
using PDS.Core.Config;
using Moq;
using PDS.SpaceFE.RBG.PADS.Module.Data;
using System.Collections.Generic;
using PDS.Queue.Api.Message;
using PDS.Queue.Api;

namespace PDS.SpaceFE.RBG.PADS.Module.Tests
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
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNewValidWafer);
            var spacePads = loader.CreateOperLotAggregates(spaceE4A, new Mock<IQueueMessage>().Object);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
                spacePads.SearchPatterns.SpaceKey);
            appMock.Verify(m => m.InsertDoc(spacePads), Times.Once);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SpacePads>()), Times.Never);
        }

        [TestMethod]
        public void TestCreateOperLotAggregatesUpdate()
        {
            var appMock = new Mock<IPadsDao>();
            var padsExisting = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.PADSOperdoc);
            appMock.Setup(m => m.FindExistingDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(padsExisting);

            var queueMock = new Mock<IMessageQueueProvider>();

            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNewValidWafer);
            var spacePads = loader.CreateOperLotAggregates(spaceE4A, new Mock<IQueueMessage>().Object);
            Assert.AreEqual("Measlot:3A110879:SPACEAGGED1:RBG:FE:WLADR:8667:K84002:ProcessControl:1.0",
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
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNewValidWafer);
            var spacePADS = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.Lotdoc);
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
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "Y",
                Value = 23.45,
                SampleId = 543245,
                Seqnr = 1,
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123"
            };
            var checkvalues2 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.345,
                SampleId = 543245,
                Seqnr = 2,
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123"
            };
            var checkvalues3 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.455,
                SampleId = 543245,
                Seqnr = 3,
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123"
            };
            checkvalueslist.Add(checkvalues1);
            checkvalueslist.Add(checkvalues2);
            checkvalueslist.Add(checkvalues3);
            appMock.Setup(m => m.FindExistingWafDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(checkvalueslist);

            var queueMock = new Mock<IMessageQueueProvider>();

            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNewValidWafer);
            var spacePADS = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.Lotdoc);
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
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNewInValidWafer);
            var spacePADS = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.LotDocInvalidWafer);
            var wafdoc = loader.CreateOperWaferAggregates(spaceE4A, spacePADS, new Mock<IQueueMessage>().Object);
            appMock.Verify(m => m.InsertDoc(wafdoc), Times.Never);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), wafdoc), Times.Never);
        }

        [TestMethod]
        public void TestCreateOperWaferAggregatesUpdate()
        {
            var appMock = new Mock<IPadsDao>();
            appMock.Setup(m => m.FindExistingDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(JsonUtils.FromJson<SpacePads>(Properties.LotResources.PADSdoc));
            var checkvalueslist = new List<Data1ListRawValuesPads4Wafer>();
            var checkvalues1 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "Y",
                Value = 23.45,
                SampleId = 543245,
                Seqnr = 1,
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                ParameterUnit = "s"
            };
            var checkvalues2 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.345,
                SampleId = 543245,
                Seqnr = 2,
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                ParameterUnit = "s"
            };
            var checkvalues3 = new Data1ListRawValuesPads4Wafer()
            {
                ParameterName = "WARPAGE",
                ChannelId = "161310",
                IsFlagged = "N",
                Value = 23.455,
                SampleId = 543245,
                Seqnr = 3,
                ItemIdMotherlotWafer = "3A110879:01",
                ProcessEquipment = "543-123",
                ParameterUnit = "s"
            };
            checkvalueslist.Add(checkvalues1);
            checkvalueslist.Add(checkvalues2);
            checkvalueslist.Add(checkvalues3);
            appMock.Setup(m => m.FindExistingWafDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(checkvalueslist);

            var queueMock = new Mock<IMessageQueueProvider>();

            var configManager = new ConfigManager(new AppSettingsConfigProvider());
            var loader = new SpaceDataLoader(configManager, queueMock.Object, appMock.Object);
            var spaceE4A = JsonUtils.FromJson<SpaceE4A>(Properties.CommonResources.TestCreateNewValidWafer);
            var spacePADS = JsonUtils.FromJson<SpacePads>(Properties.CommonResources.Lotdoc);
            var wafdoc = loader.CreateOperWaferAggregates(spaceE4A, spacePADS, new Mock<IQueueMessage>().Object);
            appMock.Verify(m => m.InsertDoc(wafdoc), Times.Never);
            appMock.Verify(m => m.UpdateDoc(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), wafdoc), Times.Once);
        }


    }
}
