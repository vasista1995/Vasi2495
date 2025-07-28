using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.Common.ExtractionLog;
using PDS.Common.Utils;
using PDS.SpaceBE.Common.Source.Module.Tests;
using PDS.SpaceBE.CEG.Source.Module.Data.SpaceModel;
using System.IO;

namespace PDS.SpaceBE.CEG.Source.Module.Tests
{

    [TestClass]
    public class RegressionTests : RegressionTestsCommon
    {
        public override string Gete4ADocumentJson(string sourceFilePath)
        {
            string sourceJson = File.ReadAllText(sourceFilePath);
            var spaceEntry = JsonUtils.FromJson<SpaceEntry>(sourceJson);
            var e4Adocument = SpaceE4AConverter.Convert(spaceEntry, new Mock<IExtractionJobRun>().Object);

            e4Adocument.DataFlatMetaData.ExportedTimestamp = e4Adocument.DataFlatMetaData.SampleTimestamp;
            e4Adocument.DataFlatMetaData.ExportedTimestampUtc = e4Adocument.DataFlatMetaData.SampleTimestamp;

            string e4AdocumentJson = JsonUtils.ToJson(e4Adocument, indent: true);
            return e4AdocumentJson;
        }
    }
}
