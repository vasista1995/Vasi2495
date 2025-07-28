using MongoDB.Bson.Serialization.Attributes;

namespace PDS.Space.Common.Data.SpaceModel
{
    public class BaseSpaceRawValuesEntry
    {
        [BsonIgnoreIfNull]
        public string BreakPoint { get; set; }
        [BsonIgnoreIfNull]
        public long SampleId { get; set; }
        [BsonIgnoreIfNull]
        public long Seqnr { get; set; }
        [BsonIgnoreIfNull]
        public double Value { get; set; }
        [BsonIgnoreIfNull]
        public string ExternalFlagged { get; set; }
        [BsonIgnoreIfNull]
        public string InternalFlagged { get; set; }
        [BsonIgnoreIfNull]
        public string InternalComment { get; set; }
        [BsonIgnoreIfNull]
        public int NumViolations { get; set; }
        [BsonIgnoreIfNull]
        public string ViolationComments { get; set; }
        [BsonIgnoreIfNull]
        public string ViolationList { get; set; }
        [BsonIgnoreIfNull]
        public string PrimaryViolation { get; set; }
        [BsonIgnoreIfNull]
        public string PrimaryViolationComments { get; set; }
    }
}
