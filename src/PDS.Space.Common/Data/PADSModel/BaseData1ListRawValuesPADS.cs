using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PDS.Space.Common.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to Rawvalues array subsection of data1list section in pads document.
    /// </summary>
    public class BaseData1ListRawValuesPads
    {
        [BsonIgnoreIfNull]
        public long Seqnr { get; set; }
        [BsonIgnoreIfNull]
        public long SampleId { get; set; }
        [BsonIgnoreIfNull]
        public double Value { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.IsFlagged)]
        [BsonElement(SpacePadsProperties.IsFlagged)]
        public string IsFlagged { get; set; }
        [BsonIgnoreIfNull]
        public DateTime SampleTimestamp { get; set; }
        [BsonIgnoreIfNull]
        public DateTime SampleTimestampUtc { get; set; }
        [BsonIgnoreIfNull]
        public DateTime CreatedTimestamp { get; set; }
        [BsonIgnoreIfNull]
        public DateTime CreatedTimestampUtc { get; set; }
        [BsonIgnoreIfNull]
        public DateTime UpdatedTimestamp { get; set; }
        [BsonIgnoreIfNull]
        public DateTime UpdatedTimestampUtc { get; set; }
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
        [BsonIgnoreIfNull]
        public double SampleMean { get; set; }
        [BsonIgnoreIfNull]
        public double SampleMin { get; set; }
        [BsonIgnoreIfNull]
        public double SampleMax { get; set; }
        [BsonIgnoreIfNull]
        public double SampleMedian { get; set; }
        [BsonIgnoreIfNull]
        public double SampleStdev { get; set; }
        [BsonIgnoreIfNull]
        public double SampleQ1 { get; set; }
        [BsonIgnoreIfNull]
        public double SampleQ3 { get; set; }
        [BsonIgnoreIfNull]
        public int SampleSize { get; set; }
        [BsonIgnoreIfNull]
        public string InternalComment { get; set; }
    }
}
