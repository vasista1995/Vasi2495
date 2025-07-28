using System.Collections.Generic;
using Newtonsoft.Json;
using PDS.Common.E4AModel;

namespace PDS.SpaceNew.Common.Data.Config
{
    public class PropertyPlacementConfig
    {
        [JsonProperty(E4aProperties.DataFlatMetaData)]
        public HashSet<string> DataFlatMetaData { get; set; }
        public HashSet<string> DataFlatStrangeMetaData { get; set; }
        public HashSet<string> Data1List { get; set; }
        public HashSet<string> Data1ListRawValues { get; set; }
    }
}
