using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PDS.Common.E4AModel;
using PDS.Space.Common.Data.E4AModel;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceNew.Common
{
    /// <summary>
    /// This class possess all the properties that must be assigned to e4a document.
    /// each property has its own class where the subproperties of each property are defined.
    /// </summary>
    public class SpaceE4A : E4aDocument
    {
        public IDictionary<string, object> SpaceAttributes { get; set; }
        public IDictionary<string, object> SpaceDataLakeAttributes { get; set; }
        public IEnumerable<IDictionary<string, object>> SpaceRawValueAttributeCollection { get; set; }
        public new BaseSystemLogE4A SystemLog { get; set; }

        public string PKey { get; set; }

        public SpaceE4A(BaseSystemLogE4A systemLog, IDictionary<string, object> spaceAttributes,
            IDictionary<string, object> spaceDataLakeAttributes,
            IEnumerable<IDictionary<string, object>> spaceRawValueAttributes, string pKey) : base(systemLog)
        {
            SystemLog = systemLog;
            IdSource = $"SPACEACT2:{pKey}";
            SpaceAttributes = spaceAttributes;
            SpaceDataLakeAttributes = spaceDataLakeAttributes;
            SpaceRawValueAttributeCollection = spaceRawValueAttributes;
            PKey = pKey;
        }
    }
}
