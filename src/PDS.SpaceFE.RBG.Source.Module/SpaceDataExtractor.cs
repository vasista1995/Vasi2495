using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PDS.Common.ExtractionInbox;
using PDS.Common.ExtractionLog;
using PDS.Common.Source;
using PDS.Core.Api.Inject;
using PDS.Core.Api.Utils;
using PDS.SpaceFE.RBG.Common.Config;
using PDS.SpaceFE.RBG.Common.Data.E4AModel;
using PDS.SpaceFE.RBG.Source.Module.Data;
using PDS.SpaceFE.RBG.Source.Module.Data.SpaceModel;

namespace PDS.SpaceFE.RBG.Source.Module
{
    /// <summary>
    /// This class creates a kafka producer, a connection to space rbg fe offline database.
    /// Extracts data from space offline db, creates e4a messages.
    /// pushes the e4a document messages into the producer queue topic.
    /// </summary>
    ///
    [ExcludeFromCodeCoverage]
    public class SpaceDataExtractor : BaseDateRangeExtractor<SpaceEntry, SpaceE4A>
    {
        private const string DefaultLoadingJob = "RBG";

        private readonly SpaceDao _spaceDao;

        [Inject]
        public SpaceDataExtractor([NotNull] SourceExtractorManager manager, [NotNull] SpaceDao spaceDao)
            : base(SpaceConfigs.AppName, DefaultLoadingJob, manager)
        {
            _spaceDao = Ensure.NotNull(spaceDao, nameof(spaceDao));
        }

        protected override IEnumerable<SpaceEntry> GetSourceRecords(DateRangeExtractionJobRun runLog, SourceExtractContext context)
        {
            return _spaceDao.GetSpaceDatabaseEntries(runLog.StartValue, runLog.EndValue);
        }

        protected override SpaceE4A ConvertToE4aDocument(SpaceEntry sourceRecord, DateRangeExtractionJobRun runLog, SourceExtractContext context)
        {
            return SpaceE4AConverter.Convert(sourceRecord, runLog);
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
