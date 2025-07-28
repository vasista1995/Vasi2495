using System;
using System.Collections.Generic;
using System.Linq;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.Common.Data.Config;

namespace PDS.SpaceNew.PADS.Module
{
    internal class SpacePropertyRawValueTransferer
    {
        internal SpaceE4A TransferProperties(PropertyRawValueTransferConfig propertyRawValueTransferConfig, SpaceE4A renamedAttributesE4A)
        {
            foreach (string propertyToTransfer in propertyRawValueTransferConfig.RawValueTransferProperties)
            {
                if (renamedAttributesE4A.SpaceAttributes.ContainsKey(propertyToTransfer))
                {
                    TransferProperty(propertyToTransfer, renamedAttributesE4A.SpaceAttributes, renamedAttributesE4A.SpaceRawValueAttributeCollection);
                }
            }

            // Remove metadata properties which shall not be kept after copying
            var metadataPropertiesToRemove = propertyRawValueTransferConfig.RawValueTransferProperties.Where(p => !propertyRawValueTransferConfig.MetaDataPropertiesToKeep.Contains(p));
            foreach (string metadataPropertyToRemove in metadataPropertiesToRemove)
            {
                renamedAttributesE4A.SpaceAttributes.Remove(metadataPropertyToRemove);
            }

            return renamedAttributesE4A;
        }

        private void TransferProperty(string propertyToTransfer, IDictionary<string, object> spaceAttributes,
            IEnumerable<IDictionary<string, object>> spaceRawValueAttributeCollection)
        {
            string propertyName = spaceAttributes.Keys.FirstOrDefault(key => string.Equals(key, propertyToTransfer, StringComparison.OrdinalIgnoreCase));
            if (propertyName != null)
            {
                object propertyValue = spaceAttributes[propertyName];
                foreach (var spaceRawValueProperties in spaceRawValueAttributeCollection)
                {
                    if (!spaceRawValueProperties.ContainsKey(propertyName))
                    {
                        spaceRawValueProperties.Add(propertyName, propertyValue);
                    }
                    else
                    {
                        spaceRawValueProperties[propertyName] = propertyValue;
                    }
                }
            }
        }
    }
}
