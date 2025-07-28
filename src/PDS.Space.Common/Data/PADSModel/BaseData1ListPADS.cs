using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PDS.Space.Common.Data.PADSModel
{
    public class BaseData1ListPads
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to Data1list section in pads document.
        /// </summary>
        ///
        [JsonProperty(SpacePadsProperties.DataFlatLimits)]
        [BsonElement(SpacePadsProperties.DataFlatLimits)]
        public DataFlatLimitsPads DataFlatLimits { get; set; }
        [JsonProperty(SpacePadsProperties.MeasurementAggregates)]
        [BsonElement(SpacePadsProperties.MeasurementAggregates)]
        public MeasurementAggregatesPads MeasurementAggregates { get; set; }
        public string ParameterName { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterUnit { get; set; }
        [BsonIgnoreIfNull]
        public string ChannelId { get; set; }
        [BsonIgnoreIfNull]
        public string SourceDataLevel { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.ChannelName)]
        [BsonElement(SpacePadsProperties.ChannelName)]
        public string ChannelName { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.ChannelDescr)]
        [BsonElement(SpacePadsProperties.ChannelDescr)]
        public string ChannelDescr { get; set; }
        [BsonIgnoreIfNull]
        public string RvStoreFlag { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CreatedTimestampUtc)]
        [BsonElement(SpacePadsProperties.CreatedTimestampUtc)]
        public DateTime CreatedTimestampUtc { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CreatedTimestamp)]
        [BsonElement(SpacePadsProperties.CreatedTimestamp)]
        public DateTime CreatedTimestamp { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.UpdatedTimestampUtc)]
        [BsonElement(SpacePadsProperties.UpdatedTimestampUtc)]
        public DateTime UpdatedTimestampUtc { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.UpdatedTimestamp)]
        [BsonElement(SpacePadsProperties.UpdatedTimestamp)]
        public DateTime UpdatedTimestamp { get; set; }
        
    }
}
