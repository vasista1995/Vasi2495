using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using PDS.SpaceNew.Common.Data.Config;

namespace PDS.SpaceNew.PADS.Module
{
    public class SpaceAttributeMappingController
    {
        public IDictionary<int, Dictionary<string, string>> LdsIdSpaceAttributesMappingConfig = new Dictionary<int, Dictionary<string, string>>();
        public IDictionary<int, Dictionary<string, string>> LdsIdSpaceRawValuesAttributesMappingConfig = new Dictionary<int, Dictionary<string, string>>();

        public IDictionary<string, string> RawValueAttributesMappingConfig { get; set; }

        public IEnumerable<string> RawValueAttributeNames { get; set; }
        public Dictionary<string, string> CustomerFieldAttributesMappingConfig { get; set; }

        public SpaceAttributeMappingController(AttributeMappingConfig attributeMappingConfig)
        {
            RawValueAttributeNames = attributeMappingConfig.RawValueAttributeMappings.Keys.ToList();
            attributeMappingConfig.LdsAttributeMappings.ToList().ForEach(i => LdsIdSpaceAttributesMappingConfig.Add(i.LdsId, ConvertToIgnoreCaseDictionary(i.SpaceAttributesMappings)));
            attributeMappingConfig.LdsAttributeMappings.ToList().ForEach(i => LdsIdSpaceRawValuesAttributesMappingConfig.Add(i.LdsId, ConvertToIgnoreCaseDictionary(i.SpaceRawValueAttributesMappings)));
            RawValueAttributesMappingConfig = ConvertToIgnoreCaseDictionary(attributeMappingConfig.RawValueAttributeMappings);
            CustomerFieldAttributesMappingConfig = ConvertToIgnoreCaseDictionary(attributeMappingConfig.CustomerFieldAttributeMappings);
        }

        private Dictionary<string, T> ConvertToIgnoreCaseDictionary<T>(IDictionary<string, T> dictionary)
        {
            return new Dictionary<string, T>(dictionary, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
