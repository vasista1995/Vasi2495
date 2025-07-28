using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PDS.Space.Common.Data.PADSModel
{
    public class BaseMeasurementSpecLimitsPads
    {
        [BsonIgnoreIfNull]
        public string SpecHighEnabled { get; set; }
        [BsonIgnoreIfNull]
        public string SpecLowEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.SpecHigh)]
        [BsonElement(SpacePadsProperties.SpecHigh)]
        public double? SpecHigh { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.SpecLow)]
        [BsonElement(SpacePadsProperties.SpecLow)]
        public double? SpecLow { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.Target)]
        [BsonElement(SpacePadsProperties.Target)]
        public double? SpecTarget { get; set; }
        [BsonIgnoreIfNull]
        public string RemovalDue2Ambiguity { get; set; }
    }
}
