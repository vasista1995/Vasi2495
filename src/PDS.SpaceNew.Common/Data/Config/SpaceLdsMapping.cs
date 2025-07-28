using System.Collections.Generic;

namespace PDS.SpaceNew.PADS.Module.Data
{
    public class SpaceLdsMappingConfig
    {
        public int LdsId { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> SpaceAttributesMappings { get; set; }
        public Dictionary<string, string> SpaceRawValueAttributesMappings { get; set; }
    }
}
