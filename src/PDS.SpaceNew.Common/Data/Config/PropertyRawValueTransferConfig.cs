using System.Collections.Generic;

namespace PDS.SpaceNew.Common.Data.Config
{
    public class PropertyRawValueTransferConfig
    {
        public HashSet<string> RawValueTransferProperties { get; set; }
        public HashSet<string> MetaDataPropertiesToKeep { get; set; }
    }
}
