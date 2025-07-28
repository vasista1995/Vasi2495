using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PDS.Space.Common.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to measurementaggregates subsection of DataFlatlimits section in pads document.
    /// </summary>
    public class MeasurementAggregatesPads : BaseMeasurementAggregatesPads
    {
        [JsonProperty(SpacePadsProperties.GofMean)]
        [BsonElement(SpacePadsProperties.GofMean)]
        [BsonIgnoreIfNull]
        public double? GofMean { get; set; }
        [JsonProperty(SpacePadsProperties.GofMin)]
        [BsonElement(SpacePadsProperties.GofMin)]
        [BsonIgnoreIfNull]
        public double? GofMin { get; set; }
        [JsonProperty(SpacePadsProperties.GofMax)]
        [BsonElement(SpacePadsProperties.GofMax)]
        [BsonIgnoreIfNull]
        public double? GofMax { get; set; }
    }
}
