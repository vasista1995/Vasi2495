using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PDS.Space.Common.Data.PADSModel
{
    public class DataFlatLimitsPads
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatlimits section in pads document.
        /// </summary>
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CKCId)]
        [BsonElement(SpacePadsProperties.CKCId)]
        public string CKCId { get; set; }
        [BsonIgnoreIfNull]
        public MeasurementSpecLimitsPads MeasurementSpecLimits { get; set; }
        [BsonIgnoreIfNull]
        public ControlLimitsPads ControlLimits { get; set; }
    }
}
