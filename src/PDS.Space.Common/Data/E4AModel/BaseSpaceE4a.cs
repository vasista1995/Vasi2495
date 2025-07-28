using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using PDS.Common.E4AModel;

namespace PDS.Space.Common.Data.E4AModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to e4a document.
    /// each property has its own class where the subproperties of each property are defined.
    /// </summary>
    public class BaseSpaceE4A : E4aDocument
    {
        public BaseSpaceE4A(BaseSystemLogE4A systemLog) : base(systemLog)
        {
            SystemLog = systemLog;
        }

        public BaseProductionActionE4A ProductionAction { get; set; }

        public BaseItemE4A Item { get; set; }

        public new BaseSystemLogE4A SystemLog { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public string Id { get; set; }
    }
}
