using System.Diagnostics.CodeAnalysis;
using PDS.Core.Api.Inject;
using PDS.Core.Api.Utils;
using PDS.SpaceNew.Source.Module.Data;
using PDS.SpaceNew.Source.Module.Data.SpaceModel;
using System.Linq;
using System.Collections.Generic;
using PDS.SpaceNew.Common;
using PDS.Common.ExtractionLog;
using PDS.Common.Source;
using PDS.Common.ExtractionInbox;
using System;
using PDS.SpaceNew.Common.Data.Config;
using PDS.SpaceNew.Common.Config;
using PDS.Core.Api;
using PDS.Space.Common.Data;
using System.Collections;
using PDS.Common.E4AModel;
using PDS.SpaceNew.PADS.Module;
using Newtonsoft.Json;
using System.IO;

namespace PDS.SpaceNew.Source.Module
{
    /// <summary>
    /// This class creates a kafka producer, a connection to space rbg fe offline database.
    /// Extracts data from space offline db, creates e4a messages.
    /// pushes the e4a document messages into the producer queue topic.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SpaceDataExtractor : BaseDateRangeExtractor<SpaceEntry, SpaceE4A>
    {
        private readonly SpaceDao _spaceDao;
        private readonly SpaceEntryConverter _spaceEntryConverter = new SpaceEntryConverter();
        private readonly SpaceE4AConverter _spaceE4AConverter = new SpaceE4AConverter();
        private readonly AttributeChanger _attributeChanger = new AttributeChanger();
        private readonly SpaceAttributeNameChanger _spaceAttributeNameChanger = new SpaceAttributeNameChanger();
        private readonly SpaceAttributeMappingController _attributeMappingController;
        private readonly AttributeMappingConfig _spaceAttributeMappingConfig;
        private readonly AttributeCalculationConfig _spaceAttributeCalculationConfig;
        private readonly AttributeMappingConfig _attributeMappingConfig;
        private readonly List<int> _ldsIds;
        private readonly string _siteKey;
        private readonly string _spaceInstanceName;

        [Inject]
        public SpaceDataExtractor([NotNull] SourceExtractorServices manager, [NotNull] SpaceDao spaceDao)
            : base(System.Environment.GetEnvironmentVariable(SpaceConfigVariables.AppName), "Default", manager)
        {
            _spaceDao = Ensure.NotNull(spaceDao, nameof(spaceDao));

            string appName = ConfigHelper.GetSpaceAppName();
            _spaceInstanceName = ConfigHelper.GetSpaceInstanceName(appName);
            _siteKey = System.Environment.GetEnvironmentVariable(EnvironmentVariables.SiteKey);

            _attributeMappingConfig = ConfigFileController.GetSpaceAttributeMappingConfig(_siteKey, _spaceInstanceName);
            _spaceAttributeMappingConfig = ConfigFileController.GetSpaceAttributeMappingConfig(_siteKey, _spaceInstanceName);
            _spaceAttributeCalculationConfig = ConfigFileController.GetSpaceAttributeCalculationConfig(_siteKey, _spaceInstanceName);
            _attributeMappingController = new SpaceAttributeMappingController(_attributeMappingConfig);

            SpaceEntry.RawValueAttributeNames = _spaceAttributeMappingConfig.RawValueAttributeMappings.Keys.ToHashSet();
            _ldsIds = _spaceAttributeMappingConfig.LdsAttributeMappings.Select(i => i.LdsId).Distinct().ToList();
        }

        protected override IEnumerable<SpaceEntry> GetSourceRecords(DateRangeExtractionJobRun runLog, SourceExtractContext context)
        {
            var allSpaceEntries = new List<SpaceEntry>();
            foreach (int ldsID in _ldsIds)
            {
                var spaceEntries = GetSourceRecordsForLdsId(runLog, ldsID);
                allSpaceEntries.AddRange(spaceEntries);
            }

            return allSpaceEntries;
        }

        private IEnumerable<SpaceEntry> GetSourceRecordsForLdsId(DateRangeExtractionJobRun runLog, int ldsId)
        {
            var spaceDatabaseEntries = _spaceDao.GetSpaceData(runLog.StartValue, runLog.EndValue, ldsId);

            // Get lotAttributeKey for ldsId
            var lotAttributeKeys = _spaceAttributeMappingConfig.LdsAttributeMappings.First(m => m.LdsId == ldsId).SpaceAttributesMappings
                                                                                    .Where(x => string.Equals(x.Value, RequiredConvertedAttributes.MeasLot) ||
                                                                                                string.Equals(x.Value, RequiredConvertedAttributes.Lot) || string.Equals(x.Value, RequiredConvertedAttributes.ProductionOrder));
            if (!lotAttributeKeys.Any())
                throw new KeyNotFoundException($"No Lot attribute with name \"{RequiredConvertedAttributes.MeasLot}\" or \"{RequiredConvertedAttributes.Lot}\" or \"{RequiredConvertedAttributes.ProductionOrder}\" is defined in mapping for lds id {ldsId}!");

            if (lotAttributeKeys.Count() > 1)
                throw new InvalidOperationException($"Lot attribute with name \"{RequiredConvertedAttributes.MeasLot}\" or \"{RequiredConvertedAttributes.Lot}\" is defined multiple times in mapping for lds id {ldsId}!");

            // Retrieve ParameterFacility and ParameterOper attribute keys for crating the Unique Sourcekey at E4A
            var serializer = new JsonSerializer();
            using var streamReader = new StreamReader(Path.Combine("Resources", _spaceInstanceName, runLog.SiteKey, "PKeyConfig.json"));
            using var jsonReader = new JsonTextReader(streamReader);
            var configData = serializer.Deserialize<PKeyConfigRoot>(jsonReader);
            var pKeyConfigs = configData.LdsPKeyMappings;
            var pKeyConfig = pKeyConfigs.FirstOrDefault(c => c.LdsId == ldsId);
            var parameterFacility = pKeyConfig.PKeyAttributesMappings.FirstOrDefault(x => x.Value == "ParameterFacility").Key;
            var parameterOper = pKeyConfig.PKeyAttributesMappings.FirstOrDefault(x => x.Value == "ParameterOper").Key;
            string lotAttribute = lotAttributeKeys.First().Key;
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("parameterFacility", parameterFacility);
            keyValuePairs.Add("parameterOper", parameterOper);
            keyValuePairs.Add("lotAttribute", lotAttribute);
            var spaceEntries = _spaceEntryConverter.Convert(spaceDatabaseEntries, runLog.SiteKey, keyValuePairs, _spaceInstanceName);
            return spaceEntries;
        }

        protected override SpaceE4A ConvertToE4aDocument(SpaceEntry sourceRecord, DateRangeExtractionJobRun runLog, SourceExtractContext context)
        {
            var spaceE4A = _spaceE4AConverter.Convert(sourceRecord, runLog, AppName);

            var flexibleAttributeMappingConfig = ConfigFileController.GetMappingConfig(spaceE4A, _attributeMappingController.LdsIdSpaceAttributesMappingConfig);
            var flexibleRawValuesAttributeMappingConfig = ConfigFileController.GetMappingConfig(spaceE4A, _attributeMappingController.LdsIdSpaceRawValuesAttributesMappingConfig);
            var rawValueAttributeMappingConfig = _attributeMappingController.RawValueAttributesMappingConfig.Concat(flexibleRawValuesAttributeMappingConfig).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var renamedAttributesE4A = _spaceAttributeNameChanger.MapSpaceAttributes(flexibleAttributeMappingConfig, _attributeMappingController.CustomerFieldAttributesMappingConfig, rawValueAttributeMappingConfig, spaceE4A);
            //if (!renamedAttributesE4A.SpaceAttributes.ContainsKey("MeasLot"))
            //{
            //    renamedAttributesE4A.SpaceAttributes.Add("MeasLot", renamedAttributesE4A.SpaceAttributes["ProductionOrder"]);
            //}

            AddIdSystemName(renamedAttributesE4A);
            AddMotherLotWaferAttribute(renamedAttributesE4A);
            AddSourceDataLevel(renamedAttributesE4A);
            _attributeChanger.AddCalculatedAttributes(renamedAttributesE4A, _spaceAttributeCalculationConfig);
            _attributeChanger.RemoveAttributes(renamedAttributesE4A, _spaceAttributeCalculationConfig);
            // TODO: Check why WaferName has to be converted to MotherLotWafer in SpaceFE and not in SpaceBE
            return renamedAttributesE4A;
        }

        private void AddSourceDataLevel(SpaceE4A spaceE4A)
        {
            string motherLotWafer = (string) (spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.MotherlotWafer) ?? null);
            string rvStoreFlag = (string) (spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.RvStoreFlag) ?? null);
            spaceE4A.SpaceAttributes[RequiredConvertedAttributes.SourceDataLevel] = GetSourceDataLevel(motherLotWafer, rvStoreFlag, spaceE4A.IdSource);
        }

        /// <summary>
        /// This method is a workaround for a site specific case function:
        ///
        /// CASE WHEN t.module LIKE '1%' THEN 'HPSModFAUF'
        /// WHEN t.module LIKE '0%' THEN 'HPSSubFAUF'
        /// ELSE 'ERROR'
        /// END IdSystemName
        /// 
        /// In case you got some new case statements then you can consider adding a new site specific
        /// config file type and exchange this method by the new config file integration
        /// </summary>
        /// <param name="spaceE4A"></param>
        private void AddIdSystemName(SpaceE4A spaceE4A)
        {
            if ((_siteKey == "CEG" || _siteKey == "WAR") && _spaceInstanceName == "BE")
            {
                string module = (string) spaceE4A.SpaceAttributes.GetValueOrDefault("Module");
                spaceE4A.SpaceAttributes.Add("IdSystemName", GetIdSystemName(module));
                string CustomComment = (string) spaceE4A.SpaceAttributes.GetValueOrDefault("CustomComment");
                if (CustomComment == null)
                {
                    spaceE4A.SpaceAttributes.Add("CustomComment", "null");
                }
            }
        }
        private string GetIdSystemName(string module)
        {
            if (module == null)
                return "ERROR";
            if (module.StartsWith('1'))
                return "HPSModFAUF";
            if (module.StartsWith('0'))
                return "HPSSubFAUF";
            return "ERROR";
        }
        private string GetSourceDataLevel(string motherLotWafer, string rvStoreFlag, string idSource)
        {
            switch (rvStoreFlag)
            {
                case "N":
                    return motherLotWafer == null ? "L" : "W";
                case "Y":
                    return "C";
                default:
                    throw new InvalidOperationException($"Not Valid RVStoreFlag: {rvStoreFlag}, corresponding IdSource is {idSource}");
            }
        }

        /// <summary>
        /// This method is necessary if we only got the WaferName and not the MotherLotWafer attribute as data
        /// Then we have to create the MotherLotWafer attribute because we need this attribute in the WaferAggregation
        /// at the loader part
        /// </summary>
        /// <param name="spaceE4A"></param>
        private void AddMotherLotWaferAttribute(SpaceE4A spaceE4A)
        {
            string Exval14 = (string) spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.MotherlotWafer);
            if (!string.IsNullOrEmpty(Exval14) && Exval14.StartsWith("WAFER_"))
            {
                string wafer = Exval14.Substring(Exval14.Length - 1);
                if (int.TryParse(wafer, out int waferNumber))
                {
                    string waferId = $"0{waferNumber}";
                    spaceE4A.SpaceAttributes[RequiredConvertedAttributes.Wafer] = waferId;
                    string measLot = (string) spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.MeasLot);
                    spaceE4A.SpaceAttributes[RequiredConvertedAttributes.MotherlotWafer] = $"{measLot}:{waferId}";
                    return;
                }
                else
                {
                    // Handle the case where wafer is not a valid integer
                }
            }
            if (Exval14 == "Halloren1"|| Exval14=="-")
            {
                spaceE4A.SpaceAttributes[RequiredConvertedAttributes.MotherlotWafer] = null;
                spaceE4A.SpaceAttributes[RequiredConvertedAttributes.MotherLot] = (string) spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.MeasLot);
            }
            if (!string.IsNullOrEmpty(Exval14) && Exval14 == "NoID")
            {
                spaceE4A.SpaceAttributes[RequiredConvertedAttributes.MotherlotWafer] = null;
                spaceE4A.SpaceAttributes[RequiredConvertedAttributes.MotherLot] = (string) spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.MeasLot);
            }
            if (!string.IsNullOrEmpty(Exval14) && Exval14.Length == 2)
            {
                string measLot = (string) spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.MeasLot);
                string lot = (string) spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Lot);
                lot = lot ?? measLot;
                spaceE4A.SpaceAttributes[RequiredConvertedAttributes.Wafer] = Exval14;
                spaceE4A.SpaceAttributes[RequiredConvertedAttributes.MotherlotWafer] = $"{lot}:{Exval14}";
            }
            else if (!string.IsNullOrEmpty(Exval14) && Exval14 != "NoID")
            {
                spaceE4A.SpaceAttributes[RequiredConvertedAttributes.Wafer] = int.Parse(Exval14.Substring(Exval14.Length - 2));
                spaceE4A.SpaceAttributes[RequiredConvertedAttributes.MotherLot] = Exval14[..8];
                string motherlotWafer = ConvertMotherlotWaferformat(Exval14);
                spaceE4A.SpaceAttributes[RequiredConvertedAttributes.MotherlotWafer] = motherlotWafer;
            }

            // WaferName sometimes also contains the motherLot
            string waferName = (string) spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.WaferName);
            if (waferName != null)
            {
                string motherLotWafer = ConvertWaferName(waferName);
                spaceE4A.SpaceAttributes.Add(RequiredConvertedAttributes.MotherlotWafer, motherLotWafer);
                return;
            }

            // MotherLot sometimes also contains the WaferName
            //string motherLot = (string) spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.MotherLot);
            //if (motherLot != null)
            //{
            //    waferName = motherLot.Length > 2 ? motherLot.Split('.').Last() : motherLot;
            //    string measLot = (string) spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.MeasLot);
            //    string lot = (string) spaceE4A.SpaceAttributes.GetValueOrDefault(RequiredConvertedAttributes.Lot);
            //    lot = lot ?? measLot;
            //    string motherLotWafer = $"{lot}:{waferName}";
            //    spaceE4A.SpaceAttributes.Add(RequiredConvertedAttributes.MotherlotWafer, motherLotWafer);
            //}
        }
        private string ConvertMotherlotWaferformat(string motherlotwafer)
        {
            string motherLot = motherlotwafer[..8];
            int waferId = int.Parse(motherlotwafer.Substring(motherlotwafer.Length - 2));
            return motherLot + ":" + waferId.ToString();
        }

        private string ConvertWaferName(string waferName)
        {
            if (waferName == null || waferName.Length < 8)
            {
                return waferName;
            }

            string motherLot = waferName[..8];
            int waferId = int.Parse(waferName.Substring(waferName.Length - 2));
            return motherLot + ":" + waferId.ToString();
        }

        protected override void UpdateRunLog(DateRangeExtractionJobRun runLog, IEnumerable<SpaceEntry> sourceRecords,
            IEnumerable<SpaceE4A> e4aDocuments, SourceExtractContext context)
        {
            runLog.ExtractedRecords = e4aDocuments.Count();
            if (runLog.JobType.Equals(JobType.Continuous) && runLog.ExtractedRecords != 0)
            {
                runLog.EndValue = e4aDocuments.Max(doc => (DateTime) doc.SpaceAttributes.GetValueOrThrow(RequiredRawAttributes.UpdatedTimestamp));
                if(runLog.StartValue.Equals(runLog.EndValue))
                {
                    runLog.EndValue = runLog.EndValue.Add(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}
