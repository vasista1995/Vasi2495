using System.Diagnostics.CodeAnalysis;
using PDS.Core.Api.Inject;
using PDS.Core.Api.Utils;
using PDS.SpaceBE.CJJ.Source.Module.Data;
using PDS.SpaceBE.CJJ.Source.Module.Data.SpaceModel;
using System.Linq;
using System.Collections.Generic;
using PDS.SpaceBE.CJJ.Common.Data.E4AModel;
using PDS.Common.ExtractionLog;
using PDS.SpaceBE.CJJ.Common.Config;
using PDS.Common.Source;
using PDS.Common.ExtractionInbox;
using PDS.Space.Common.Data;
using PDS.Space.Common;

namespace PDS.SpaceBE.CJJ.Source.Module
{
    /// <summary>
    /// This class creates a kafka producer, a connection to space rbg fe offline database.
    /// Extracts data from space offline db, creates e4a messages.
    /// pushes the e4a document messages into the producer queue topic.
    /// </summary>

    [ExcludeFromCodeCoverage]
    public class SpaceDataExtractor : BaseDateRangeExtractor<SpaceEntry, SpaceE4A>
    {
        private const string DefaultLoadingJob = "CJJ";
        private readonly string _regressionTestsDirectoryPath = @"..\..\..\..\..\test\PDS.SpaceBE.CJJ.Source.Module.Tests\Resources\RegressionTests\";
        private readonly bool _createRegressionTests;

        private readonly SpaceDao _spaceDao;
        private readonly AttributeAnalyzer _analyzer;

        [Inject]
        public SpaceDataExtractor([NotNull] SourceExtractorManager manager, [NotNull] SpaceDao spaceDao)
            : base(SpaceConfigs.AppName, DefaultLoadingJob, manager)
        {
            _spaceDao = Ensure.NotNull(spaceDao, nameof(spaceDao));
            _analyzer = new AttributeAnalyzer(_regressionTestsDirectoryPath);
            _createRegressionTests = System.Environment.GetEnvironmentVariable(SpaceConfigVariables.CreateRegressionTests) == "True";

        }

        protected override IEnumerable<SpaceEntry> GetSourceRecords(DateRangeExtractionJobRun runLog, SourceExtractContext context)
        {
            return _spaceDao.GetSpaceDatabaseEntries(runLog.StartValue, runLog.EndValue);
        }

        protected override SpaceE4A ConvertToE4aDocument(SpaceEntry sourceRecord, DateRangeExtractionJobRun runLog, SourceExtractContext context)
        {
            var e4ADocument = SpaceE4AConverter.Convert(sourceRecord, runLog);
#if DEBUG
            if (_createRegressionTests)
                _analyzer.AnalyzeFile(sourceRecord, sourceRecord.LdsID);
#endif
            return e4ADocument;
        }

        protected override void UpdateRunLog(DateRangeExtractionJobRun runLog, IEnumerable<SpaceEntry> sourceRecords,
            IEnumerable<SpaceE4A> e4aDocuments, SourceExtractContext context)
        {
            runLog.ExtractedRecords = e4aDocuments.Count();
            if (runLog.JobType.Equals(JobType.Continuous) && runLog.ExtractedRecords != 0)
            {
                runLog.EndValue = e4aDocuments.Max(doc => doc.DataFlatMetaData.UpdatedTimestamp);
            }
        }
    }
}
