using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace PDS.Space.Common.Data.PADSModel
{
    /// <summary>
    /// This class provides the main properties that must be present in a space mongo document.
    /// The properties for each subclass will be followed in different classes.
    /// </summary>
    public class BaseSpacePads
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }
        public DocumentPads Document { get; set; }
        public ProductionActionPads ProductionAction { get; set; }
        public ItemPads Item { get; set; }
        public SystemLogPads SystemLog { get; set; }
        public SearchPatternsPads SearchPatterns { get; set; }
    }
}
