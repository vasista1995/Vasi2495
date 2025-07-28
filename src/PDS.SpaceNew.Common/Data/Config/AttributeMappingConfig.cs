using System.Collections.Generic;
using PDS.SpaceNew.PADS.Module.Data;

namespace PDS.SpaceNew.Common.Data.Config
{
    public class AttributeMappingConfig
    {
        public IEnumerable<SpaceLdsMappingConfig> LdsAttributeMappings { get; set; }
        public IDictionary<string, string> RawValueAttributeMappings { get; set; }
        public IDictionary<string, string> CustomerFieldAttributeMappings { get; set; }
    }
}
