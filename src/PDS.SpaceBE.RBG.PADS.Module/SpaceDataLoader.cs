using System.Diagnostics.CodeAnalysis;
using PDS.Core.Api.Config;
using PDS.Core.Api.Inject;
using PDS.Queue.Api;
using PDS.SpaceBE.RBG.PADS.Module.Aggregations;
using PDS.SpaceBE.RBG.Common.Data.E4AModel;
using PDS.SpaceBE.RBG.PADS.Module.Data.PADSModel;
using PDS.SpaceBE.RBG.Common.Config;
using PDS.Common.Target;
using PDS.SpaceBE.RBG.PADS.Module.Data;
using PDS.Core.Api.Utils;
using System.Collections.Generic;
using PDS.Queue.Api.Message;
using System.Linq;

namespace PDS.SpaceBE.RBG.PADS.Module
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
            SpacePads operLotPADS;
            //Calculating Lot aggregates
            using (var split = context.Stopwatch.Start("Lot oper-suboper Aggregates"))
            {
                operLotPADS = CreateOperLotAggregates(e4aDocument, context.QueueMessage);
            }
            //Calculating Wafer aggregates
            using (var split = context.Stopwatch.Start("Wafer oper-suboper Aggregates"))
            {
                CreateOperWaferAggregates(e4aDocument, operLotPADS, context.QueueMessage);
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

        public SpacePads CreateOperWaferAggregates(SpaceE4A e4aEntry, SpacePads operLotPADS, IQueueMessage message)
        {
            var padsOperDoc = new SpacePads();
            var wafers = new List<string>();
            foreach (var rawValues in e4aEntry.Data1List.Data1ListRawValues)
            {
                if (rawValues.WaferLot != null && rawValues.WaferLot != "-")
                {
                    if (wafers.Contains(rawValues.WaferLot))
                        continue;

                    wafers.Add(rawValues.WaferLot);
                }
            }
            foreach (string wafer in wafers.Distinct())
            {
                padsOperDoc = CreateOperWaferAggregate(e4aEntry, operLotPADS, wafer, message);
            }
            return padsOperDoc;
        }


        private SpacePads CreateOperWaferAggregate(SpaceE4A e4aEntry, SpacePads operLotPADS, string wafer, IQueueMessage message)
        {
            var wafPADS = new SpacePads();
            string waf = wafer.Length > 2 ? wafer.Split('.').Last() : wafer;
            waf = int.Parse(waf).ToString();
            string idOperWafAgg = GetOperWaferAggregationId(e4aEntry, waf);
            var aggregation = new WaferAggregation(e4aEntry, message);
            var checkValueList = _padsDao.FindExistingWafDoc(e4aEntry.DataFlatMetaData.Lot, wafer, operLotPADS.ProductionAction.Id);

            var checkWafDoc = _padsDao.FindExistingDoc(e4aEntry.DataFlatMetaData.SiteKey, "0", idOperWafAgg);

            if (!(checkValueList is null) && checkValueList.Count > 0)
            {
                if (checkWafDoc != null)
                {
                    wafPADS = aggregation.UpdateExisting(idOperWafAgg, waf, operLotPADS, checkWafDoc, checkValueList);
                    _padsDao.UpdateDoc(wafPADS.SearchPatterns.SiteKey, wafPADS.SearchPatterns.TimeGroup, wafPADS.Id, wafPADS);
                }
                else
                {
                    wafPADS = aggregation.CreateNew(idOperWafAgg, waf, operLotPADS, checkValueList);
                    _padsDao.InsertDoc(wafPADS);
                }
            }
            return wafPADS;
        }

        private static string GetOperWaferAggregationId(SpaceE4A e4aEntry, string waferName)
        {
            string facility = e4aEntry.DataFlatMetaData.ParameterFacility;
            string specName = e4aEntry.DataFlatMetaData.SpecName;
            string measLot = e4aEntry.DataFlatMetaData.Lot;
            string oper = e4aEntry.DataFlatMetaData.ParameterOper;
            return $"MotherlotWafer:{measLot}:{int.Parse(waferName)}:SPACEAGGED2:RBG:BE:{facility}:{oper}:{specName}:ProcessControl:1.0";
        }
    }
}
