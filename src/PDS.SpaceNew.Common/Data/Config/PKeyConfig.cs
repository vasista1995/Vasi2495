using System.Collections.Generic;

namespace PDS.SpaceNew.Common.Data.Config
{
    public class PKeyConfigRoot
    {
        public List<PKeyConfig> LdsPKeyMappings { get; set; }
    }

    public class PKeyConfig
    {
        public int LdsId { get; set; }
        public Dictionary<string, string> PKeyAttributesMappings { get; set; }
    }
}
