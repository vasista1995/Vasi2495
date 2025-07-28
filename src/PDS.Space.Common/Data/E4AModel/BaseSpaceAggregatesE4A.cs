using MongoDB.Bson.Serialization.Attributes;

namespace PDS.Space.Common.Data.E4AModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to spaceaggregates subsection of DataFlatlimits section in e4a document.
    /// </summary>
    public class BaseSpaceAggregatesE4A
    {
        [BsonIgnoreIfNull]
        public string Mean { get; set; }
        [BsonIgnoreIfNull]
        public string Median { get; set; }
        [BsonIgnoreIfNull]
        public string Sigma { get; set; }
        [BsonIgnoreIfNull]
        public string Min { get; set; }
        [BsonIgnoreIfNull]
        public string Max { get; set; }
        [BsonIgnoreIfNull]
        public string Q1 { get; set; }
        [BsonIgnoreIfNull]
        public string Q3 { get; set; }
    }
}
