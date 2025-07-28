using System.Diagnostics.CodeAnalysis;
using PDS.Core.Api.Config;
using PDS.Core.Api.Inject;
using PDS.Queue.Api;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.PADS.Module.Data.PADSModel;
using PDS.Common.Target;
using PDS.SpaceNew.PADS.Module.Data;
using PDS.Core.Api.Utils;
using PDS.SpaceNew.Common.Data.Config;
using System;
using PDS.SpaceNew.Common.Config;
using System.Collections.Generic;
using PDS.Queue.Api.Message;
using System.Linq;
using PDS.SpaceNew.PADS.Module.Aggregations;
using PDS.Space.Common.Data;
using PDS.Core.Api;
using System.Collections;
using System.Text;

namespace PDS.SpaceNew.PADS.Module
{
    /// <summary>
    /// This class creates a consumer queue from which each e4a document is taken.
    /// for each e4a document from the consumer queue the lot and wafer aggregates are created and loaded to MongoDB.
    /// </summary>
    public class SpaceDataLoader : BaseE4aMessageProcessor<SpaceE4A>
    {
        private readonly IPadsDao _padsDao;
        private readonly AttributeMappingConfig _attributeMappingConfig;
        private readonly PropertyPlacementConfig _attributePADSStructureMappingConfig;
        private readonly PropertyRawValueTransferConfig _propertyRawValueTransferConfig;
        private readonly IdCreationConfig _spaceIdCreationConfig;
        private readonly MethodInvokeConfig _methodInvokeConfig = ConfigFileController.GetMethodInvokeConfig();
        private readonly SpaceAttributeMappingController _attributeMappingController;

        private readonly SpaceLotAggregation _spaceLotAggregation;
        private readonly SpaceWaferAggregation _spaceWaferAggregation;
        private readonly SpacePropertyRawValueTransferer _spacePropertyRawValueTransferer = new SpacePropertyRawValueTransferer();
        private readonly SpacePADSUpdater _spacePADSUpdater;

        [Inject]
        public SpaceDataLoader([NotNull] IConfigManager configManager, [NotNull] IMessageQueueProvider queueProvider, [NotNull] IPadsDao padsDao)
            : base(System.Environment.GetEnvironmentVariable(SpaceConfigVariables.AppName), configManager, queueProvider)
        {
            string appName = ConfigHelper.GetSpaceAppName();
            string spaceInstanceName = ConfigHelper.GetSpaceInstanceName(appName);
            string siteKey = System.Environment.GetEnvironmentVariable(EnvironmentVariables.SiteKey);
            _attributePADSStructureMappingConfig = ConfigFileController.GetSpaceAttributePADSStructureMappingConfig(siteKey, spaceInstanceName);
            _attributeMappingConfig = ConfigFileController.GetSpaceAttributeMappingConfig(siteKey, spaceInstanceName);
            _propertyRawValueTransferConfig = ConfigFileController.GetSpacePropertyRawValueTransferConfig(siteKey, spaceInstanceName);
            _spaceIdCreationConfig = ConfigFileController.GetSpaceIdCreationConfig(siteKey, spaceInstanceName);

            _spaceLotAggregation = new SpaceLotAggregation(_spaceIdCreationConfig, _attributePADSStructureMappingConfig, siteKey);
            _spaceWaferAggregation = new SpaceWaferAggregation(_spaceIdCreationConfig, _attributePADSStructureMappingConfig, siteKey);

            _padsDao = Ensure.NotNull(padsDao, nameof(padsDao));
            _attributeMappingController = new SpaceAttributeMappingController(_attributeMappingConfig);
            _spacePADSUpdater = new SpacePADSUpdater(_spaceLotAggregation, _spaceWaferAggregation, _methodInvokeConfig, siteKey,
                _attributePADSStructureMappingConfig);
        }

        [ExcludeFromCodeCoverage]
        protected override void Process(SpaceE4A e4aDocument, E4aProcessContext context)
        {
            Ensure.NotNull(e4aDocument, nameof(e4aDocument));
            Ensure.NotNull(context, nameof(context));
            Ensure.NotNull(context.QueueMessage, nameof(context.QueueMessage));

            var flexibleAttributeMappingConfig = ConfigFileController.GetMappingConfig(e4aDocument, _attributeMappingController.LdsIdSpaceAttributesMappingConfig);
            var flexibleRawValuesAttributeMappingConfig = ConfigFileController.GetMappingConfig(e4aDocument, _attributeMappingController.LdsIdSpaceRawValuesAttributesMappingConfig);
            var rawValueAttributeMappingConfig = _attributeMappingController.RawValueAttributesMappingConfig.Concat(flexibleRawValuesAttributeMappingConfig).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var renamedAttributesE4A = _spacePropertyRawValueTransferer.TransferProperties(_propertyRawValueTransferConfig, e4aDocument);
            var newLotSpacePADS = _spaceLotAggregation.CreatePADSDocument(renamedAttributesE4A, flexibleAttributeMappingConfig, _attributeMappingController.CustomerFieldAttributesMappingConfig, context.QueueMessage);

            // Calculate Lot aggregates
            using (var split = context.Stopwatch.Start("Lot oper-suboper Aggregates"))
            {
                CreateLotAggregation(newLotSpacePADS, renamedAttributesE4A, context);
            }
            
            //Calculating Wafer aggregates
            using (var split = context.Stopwatch.Start("Wafer oper-suboper Aggregates"))
            {
                CreateOperWaferAggregates(renamedAttributesE4A, newLotSpacePADS, context.QueueMessage, flexibleAttributeMappingConfig,
                    _attributeMappingController.CustomerFieldAttributesMappingConfig);
            }

            using (var split = context.Stopwatch.Start("E4A loading to PADS"))
            {
                LoadE4A2PADS(e4aDocument);
            }
        }

        private void LoadE4A2PADS(SpaceE4A e4aDocument)
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

        private void CreateLotAggregation(SpacePads newLotSpacePADS, SpaceE4A renamedAttributesE4A, E4aProcessContext context)
        {
            string lotAggregationId = _spaceLotAggregation.GetLotAggregationId(renamedAttributesE4A, newLotSpacePADS.Item);

            SpacePads operLotPADS;
            string siteKey = (string) renamedAttributesE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SiteKey);
            var existingOperLotSpacePADS = _padsDao.FindExistingDoc(siteKey, "0", lotAggregationId);
            if (existingOperLotSpacePADS == null)
            {
                _padsDao.InsertDoc(newLotSpacePADS);
            }
            else
            {
                operLotPADS = _spacePADSUpdater.UpdateDocument(existingOperLotSpacePADS, newLotSpacePADS, renamedAttributesE4A, context.QueueMessage);
                if (operLotPADS != null)
                    _padsDao.UpdateDoc(operLotPADS.SearchPatterns.SiteKey, operLotPADS.SearchPatterns.TimeGroup, operLotPADS.Id, operLotPADS);
            }
        }

        public void CreateOperWaferAggregates(SpaceE4A renamedAttributesE4A, SpacePads lotSpacePADS, IQueueMessage message,
            IDictionary<string, string> flexibleAttributeMappingConfig, IDictionary<string, string> customerFieldAttributesMappingConfig)
        {
            var motherLotWafers = renamedAttributesE4A.SpaceRawValueAttributeCollection
                .Select(r => r.GetValueOrDefault(RequiredConvertedAttributes.MotherlotWafer))
                .Where(w => w != null &&
                            !string.IsNullOrWhiteSpace(w.ToString()) &&
                            !string.Equals(w.ToString(), "-", StringComparison.OrdinalIgnoreCase))
                .Distinct().ToList();

            foreach (string motherLotWafer in motherLotWafers)
            {
                CreateOperWaferAggregate(renamedAttributesE4A, lotSpacePADS, motherLotWafer, message,
                    flexibleAttributeMappingConfig, customerFieldAttributesMappingConfig);
            }
        }

        private void CreateOperWaferAggregate(SpaceE4A renamedAttributesE4A, SpacePads lotSpacePADS, string motherLotWafer, IQueueMessage message,
            IDictionary<string, string> flexibleAttributeMappingConfig, IDictionary<string, string> customerFieldAttributesMappingConfig)
        {
            string waferIdString = motherLotWafer.Length > 2 ? motherLotWafer.Split(':').Last() : motherLotWafer;
            int waferId = ConvertStringToIntOrThrow(waferIdString);
            string siteKey = (string) renamedAttributesE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.SiteKey);
            string waferAggregationId = _spaceWaferAggregation.GetOperWaferAggregationId(renamedAttributesE4A, motherLotWafer);

            string measLot = (string) renamedAttributesE4A.SpaceAttributes.GetValueOrThrow(RequiredConvertedAttributes.MeasLot);
            var waferValueList = _padsDao.FindExistingWafDoc(measLot, motherLotWafer, lotSpacePADS.ProductionAction.Id);
            if (waferValueList == null || !waferValueList.Any())
                return;

            var waferSpacePADS = _spaceWaferAggregation.CreateWaferSpacePADS(renamedAttributesE4A, waferAggregationId,
                   waferIdString, lotSpacePADS, waferValueList, flexibleAttributeMappingConfig, customerFieldAttributesMappingConfig, message);

            var existingWaferDocument = _padsDao.FindExistingDoc(siteKey, "0", waferAggregationId);
            if (existingWaferDocument == null)
            {
                //var waferSpacePADS = _spaceWaferAggregation.CreateWaferSpacePADS(renamedAttributesE4A, waferAggregationId,
                //    waferIdString, lotSpacePADS, waferValueList, flexibleAttributeMappingConfig, customerFieldAttributesMappingConfig, message);
                _padsDao.InsertDoc(waferSpacePADS); 
            }
            else
            {
                //    var waferSpacePADS = _spacePADSUpdater.UpdateWaferSpacePADS(waferAggregationId, waferIdString, lotSpacePADS,
                //        existingWaferDocument, renamedAttributesE4A, waferValueList, message);
                waferSpacePADS.Id = existingWaferDocument.Id;
                _padsDao.UpdateDoc(waferSpacePADS.SearchPatterns.SiteKey, waferSpacePADS.SearchPatterns.TimeGroup, waferSpacePADS.Id, waferSpacePADS);
            }
        }

        public int ConvertStringToIntOrThrow(string input)
        {
            if (int.TryParse(input, out int number))
            {
                return number;
            }
            else
            {
                throw new FormatException($"Unable to convert {input} to int");
            }
        }
    }
}
