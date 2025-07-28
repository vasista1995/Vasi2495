using System.Collections.Generic;
using PDS.SpaceNew.Common;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Linq;

namespace PDS.SpaceNew.PADS.Module
{
    /// <summary>
    /// This Lot Class generates lot aggregates for each e4a document that is pulled from the file system.
    /// Each incoming e4a document will result in either creating a new pads document or updating already existing pads document.
    /// </summary>
    public class SpaceAttributeNameChanger
    {
        /// <summary>
        /// create the e4a document with renamed attributes
        /// </summary>
        /// <returns name="SpaceE4A">The SpaceE4A document with renamed properties</returns>
        public SpaceE4A MapSpaceAttributes(Dictionary<string, string> flexibleAttributeMappingConfig,
            Dictionary<string, string> customerFieldAttributesMappingConfig,
            IDictionary<string, string> rawValueAttributeMappingConfig, [NotNull] SpaceE4A e4aDocument)
        {
            var spaceMetaDataAttributeMappingConfig = flexibleAttributeMappingConfig.Concat(customerFieldAttributesMappingConfig)
                .GroupBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Value, StringComparer.OrdinalIgnoreCase);

            var spaceValueAttributeMapping = MapSpaceValueAttributes(spaceMetaDataAttributeMappingConfig, e4aDocument);
            var spaceRawValueAttributeMapping = MapRawValueAttributes(rawValueAttributeMappingConfig, e4aDocument);

            var mappedSpaceE4A = new SpaceE4A(e4aDocument.SystemLog, spaceValueAttributeMapping, e4aDocument.SpaceDataLakeAttributes,
                spaceRawValueAttributeMapping, e4aDocument.PKey);
            return mappedSpaceE4A;
        }

        private Dictionary<string, object> MapSpaceValueAttributes(Dictionary<string, string> flexibleAttributeMappingConfig, SpaceE4A spaceE4A)
        {
            var targetSpaceValuesMapping = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            MapSpaceAttributes(flexibleAttributeMappingConfig, spaceE4A.SpaceAttributes, targetSpaceValuesMapping);
            return targetSpaceValuesMapping;
        }

        private IEnumerable<Dictionary<string, object>> MapRawValueAttributes(IDictionary<string, string> rawValueAttributeMappingConfig, SpaceE4A spaceE4A)
        {
            var newSpaceRawValuesCollection = new List<Dictionary<string, object>>();
            foreach (var sourceSpaceRawValuesMapping in spaceE4A.SpaceRawValueAttributeCollection)
            {
                var targetSpaceRawValuesMapping = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
                MapSpaceAttributes(rawValueAttributeMappingConfig, sourceSpaceRawValuesMapping, targetSpaceRawValuesMapping);
                newSpaceRawValuesCollection.Add(targetSpaceRawValuesMapping);
            }

            return newSpaceRawValuesCollection;
        }

        private void MapSpaceAttributes(IDictionary<string, string> mappingConfig, IDictionary<string, object> sourceValueMapping,
            IDictionary<string, object> targetValueMapping)
        {
            foreach (var sourceValueKeyPair in sourceValueMapping)
            {
                string sourceAttributeName = sourceValueKeyPair.Key;
                var sourceAttributeValue = sourceValueKeyPair.Value;

                string newAttributeName = mappingConfig.TryGetValue(sourceAttributeName, out string configAttributeName)
                    ? configAttributeName
                    : sourceAttributeName;

                if (string.IsNullOrWhiteSpace(newAttributeName))
                    continue;

                if (newAttributeName.Contains(","))
                {
                    // if the attribute contains a "," then it's a list of attributes
                    foreach (string newAttributeListName in newAttributeName.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        targetValueMapping[newAttributeListName] = sourceAttributeValue;
                    }
                }
                else
                {
                    targetValueMapping[newAttributeName] = sourceAttributeValue;
                }
            }
        }
    }
}
