using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PDS.Space.Common.Data.PADSModel
{
    public class BaseMeasurementAggregatesPads
    {
        [JsonProperty(SpacePadsProperties.ExecCount)]
        [BsonElement(SpacePadsProperties.ExecCount)]
        [BsonIgnoreIfNull]
        public int ExecCount { get; set; }
        [JsonProperty(SpacePadsProperties.BaseCount)]
        [BsonElement(SpacePadsProperties.BaseCount)]
        [BsonIgnoreIfNull]
        public int BaseCount { get; set; }
        [JsonProperty(SpacePadsProperties.FlaggedCount)]
        [BsonElement(SpacePadsProperties.FlaggedCount)]
        [BsonIgnoreIfNull]
        public int FlaggedCount { get; set; }
        [JsonProperty(SpacePadsProperties.Mean)]
        [BsonElement(SpacePadsProperties.Mean)]
        [BsonIgnoreIfNull]
        public double? Mean { get; set; }
        [JsonProperty(SpacePadsProperties.Median)]
        [BsonElement(SpacePadsProperties.Median)]
        [BsonIgnoreIfNull]
        public double? Median { get; set; }
        [JsonProperty(SpacePadsProperties.Sigma)]
        [BsonElement(SpacePadsProperties.Sigma)]
        [BsonIgnoreIfNull]
        public double? Sigma { get; set; }
        [JsonProperty(SpacePadsProperties.Range)]
        [BsonElement(SpacePadsProperties.Range)]
        [BsonIgnoreIfNull]
        public double? Range { get; set; }
        [JsonProperty(SpacePadsProperties.Min)]
        [BsonElement(SpacePadsProperties.Min)]
        [BsonIgnoreIfNull]
        public double? Min { get; set; }
        [JsonProperty(SpacePadsProperties.Max)]
        [BsonElement(SpacePadsProperties.Max)]
        [BsonIgnoreIfNull]
        public double? Max { get; set; }        
        [JsonProperty(SpacePadsProperties.Q2)]
        [BsonElement(SpacePadsProperties.Q2)]
        [BsonIgnoreIfNull]
        public double? Q2 { get; set; }
        [JsonProperty(SpacePadsProperties.Q5)]
        [BsonElement(SpacePadsProperties.Q5)]
        [BsonIgnoreIfNull]
        public double? Q5 { get; set; }
        [JsonProperty(SpacePadsProperties.Q25)]
        [BsonElement(SpacePadsProperties.Q25)]
        [BsonIgnoreIfNull]
        public double? Q25 { get; set; }
        [JsonProperty(SpacePadsProperties.Q75)]
        [BsonElement(SpacePadsProperties.Q75)]
        [BsonIgnoreIfNull]
        public double? Q75 { get; set; }
        [JsonProperty(SpacePadsProperties.Q95)]
        [BsonElement(SpacePadsProperties.Q95)]
        [BsonIgnoreIfNull]
        public double? Q95 { get; set; }
        [JsonProperty(SpacePadsProperties.Q98)]
        [BsonElement(SpacePadsProperties.Q98)]
        [BsonIgnoreIfNull]
        public double? Q98 { get; set; }
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
        public string Samples { get; set; }
    }
}
