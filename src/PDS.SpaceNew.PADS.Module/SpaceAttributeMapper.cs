using PDS.SpaceNew.Common;
using PDS.SpaceNew.Common.Data.Config;
using PDS.SpaceNew.PADS.Module.Data.PADSModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDS.SpaceNew.PADS.Module
{
    internal class SpaceAttributeMapper
    {
        /// <summary>
        /// Maps attributes which start with Exval_ or Daval_ to Flat-/StrangeMetaData
        /// </summary>
        /// <param name="attributePADSStructureMappingConfig"></param>
        /// <param name="renamedAttributesE4A"></param>
        /// <param name="flexibleAttributeMappingConfig"></param>
        /// <param name="customerFieldAttributeMappingConfig"></param>
        /// <returns></returns>
        internal SpacePads MapCertainAttributesToPADSStructure(SpacePads spacePads, PropertyPlacementConfig attributePADSStructureMappingConfig,
            SpaceE4A renamedAttributesE4A, IDictionary<string, string> flexibleAttributeMappingConfig, IDictionary<string, string> customerFieldAttributeMappingConfig)
        {
            var dataFlatMetaDataMapping = new Dictionary<string, object>();
            var dataFlatStrangeMetaDataMapping = new Dictionary<string, object>();
            
            // Add customer field space Attributes and exval & daval space attributes
            var flexibleAttributeNames = GetAttributeNames(flexibleAttributeMappingConfig);
            var customerFieldAttributeNames = GetAttributeNames(customerFieldAttributeMappingConfig);
            var attributeNames = flexibleAttributeNames.Union(customerFieldAttributeNames);

            var filteredSpaceAttributes = renamedAttributesE4A.SpaceAttributes.Where(i => attributeNames.Contains(i.Key, StringComparer.InvariantCultureIgnoreCase))
                                                                              .ToDictionary(i => i.Key, i => i.Value);

            AddAttributesToMetaDataMappings(dataFlatMetaDataMapping, dataFlatStrangeMetaDataMapping,
                attributePADSStructureMappingConfig.DataFlatMetaData, attributePADSStructureMappingConfig.Data1List, filteredSpaceAttributes);

            spacePads.DataFlatMetaData = dataFlatMetaDataMapping;
            spacePads.StrangeDataFlatMetaData = dataFlatStrangeMetaDataMapping;
            return spacePads;
        }

        private List<string> GetAttributeNames(IDictionary<string, string> attributeMappingConfig)
        {
            var attributeNames = attributeMappingConfig.Values.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
            return attributeNames;
        }

        private void AddAttributesToMetaDataMappings(Dictionary<string, object> dataFlatMetaDataMapping, Dictionary<string, object> dataFlatStrangeMetaDataMapping,
            HashSet<string> dataFlatMetaDataAttributes, HashSet<string> data1ListAttributes, IDictionary<string, object> spaceAttributes)
        {
            foreach (var attribute in spaceAttributes)
            {
                if (dataFlatMetaDataAttributes.Contains(attribute.Key))
                {
                    AddPropertyToObject(dataFlatMetaDataMapping, attribute.Key, attribute.Value);
                }
                else if (data1ListAttributes.Contains(attribute.Key))
                {

                }
                else
                {
                    AddPropertyToObject(dataFlatStrangeMetaDataMapping, attribute.Key, attribute.Value);
                }
            }
        }

        public void AddPropertyToObject(IDictionary<string, object> attributeValueMapping, string propertyName, object propertyValue)
        {
            if (attributeValueMapping.ContainsKey(propertyName))
                attributeValueMapping[propertyName] = propertyValue;
            else
                attributeValueMapping.Add(propertyName, propertyValue);
        }
    }
}
