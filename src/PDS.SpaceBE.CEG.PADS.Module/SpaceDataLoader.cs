using System.Diagnostics.CodeAnalysis;
using PDS.Core.Api.Config;
using PDS.Core.Api.Inject;
using PDS.Queue.Api;
using PDS.SpaceBE.CEG.PADS.Module.Aggregations;
using PDS.SpaceBE.CEG.Common.Data.E4AModel;
using PDS.SpaceBE.CEG.PADS.Module.Data.PADSModel;
using PDS.SpaceBE.CEG.Common.Config;
using PDS.Common.Target;
using PDS.SpaceBE.CEG.PADS.Module.Data;
using PDS.Core.Api.Utils;
using PDS.Queue.Api.Message;

namespace PDS.SpaceBE.CEG.PADS.Module
{
    /// <summary>
    /// This class creates a consumer queue from which each e4a document is taken.
    /// for each e4a document from the consumer queue the lot and wafer aggregates are created and loaded to MongoDB.
    /// </summary>
    public class SpaceDataLoader : BaseE4aMessageProcessor<SpaceE4A>
    {
        private readonly IPadsDao _padsDao;

        [Inject]
        public SpaceDataLoader([NotNull] IConfigManager configManager, [NotNull] IMessageQueueProvider queueProvider, [NotNull] IPadsDao padsDao)
            : base(SpaceConfigs.AppName, configManager, queueProvider)
        {
            _padsDao = Ensure.NotNull(padsDao, nameof(padsDao));
        }

        [ExcludeFromCodeCoverage]
        protected override void Process(SpaceE4A e4aDocument, E4aProcessContext context)
        {
            //Calculating Lot aggregates
            SpacePads operLotPADS;
            using (var split = context.Stopwatch.Start("Lot oper-suboper Aggregates"))
            {
                operLotPADS = CreateOperLotAggregates(e4aDocument, context.QueueMessage);
            }
            using (var split = context.Stopwatch.Start("E4A loading to PADS"))
            {
                LoadE4A2PADS(e4aDocument);
            }
        }

        public void LoadE4A2PADS(SpaceE4A e4aDocument)
        {
            var checkE4aDoc = _padsDao.FindExistingE4ADoc(e4aDocument.IdSource);

            if (checkE4aDoc == null)
            {
                _padsDao.InsertE4ADoc(e4aDocument);
            }
            else
            {
                _padsDao.UpdateE4ADoc(e4aDocument.IdSource, e4aDocument);
            }
        }

        public SpacePads CreateOperLotAggregates(SpaceE4A e4aEntry, IQueueMessage message)
        {
            var aggregation = new LotAggregation(e4aEntry, message);
            var checkOperLotDoc = _padsDao.FindExistingDoc(e4aEntry.DataFlatMetaData.SiteKey, "0", aggregation.AggregationId);
            SpacePads operLotPADS;
            if (checkOperLotDoc == null)
            {
                operLotPADS = aggregation.CreateNew();
                _padsDao.InsertDoc(operLotPADS);
            }
            else
            {
                operLotPADS = aggregation.UpdateExisting(checkOperLotDoc);
                _padsDao.UpdateDoc(operLotPADS.SearchPatterns.SiteKey, operLotPADS.SearchPatterns.TimeGroup, operLotPADS.Id, operLotPADS);
            }

            return operLotPADS;
        }
    }
}
