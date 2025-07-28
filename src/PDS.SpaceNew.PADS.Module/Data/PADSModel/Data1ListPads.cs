using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PDS.Space.Common.Data.PADSModel;
using System;
using System.Collections.Generic;

namespace PDS.SpaceNew.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to Data1list section in pads document.
    /// </summary>
    public class Data1ListPads
    {
        // RawValues
        [JsonProperty(SpacePadsProperties.Data1ListRawValues)]
        [BsonElement(SpacePadsProperties.Data1ListRawValues)]
        [BsonIgnoreIfNull]
        public List<Data1ListRawValuesPads> Data1ListRawValues { get; set; }

        // FlatLimits
        [JsonProperty(SpacePadsProperties.DataFlatLimits)]
        [BsonElement(SpacePadsProperties.DataFlatLimits)]
        public DataFlatLimitsPads DataFlatLimits { get; set; }

        // MeasurementAggregates
        [JsonProperty(SpacePadsProperties.MeasurementAggregates)]
        [BsonElement(SpacePadsProperties.MeasurementAggregates)]
        public MeasurementAggregatesPads MeasurementAggregates { get; set; }

        // Attributes which should be filled for each site
        [BsonIgnoreIfNull]
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

        // Attributes which are filled dependent to the site configuration
        [JsonProperty(SpacePadsProperties.ProcessEquipment)]
        [BsonElement(SpacePadsProperties.ProcessEquipment)]
        [BsonIgnoreIfNull]
        public string ProcessEquipment { get; set; }
        [JsonProperty(SpacePadsProperties.ProcessTool)]
        [BsonElement(SpacePadsProperties.ProcessTool)]
        [BsonIgnoreIfNull]
        public string ProcessTool { get; set; }
        [JsonProperty(SpacePadsProperties.ProcessBatch)]
        [BsonElement(SpacePadsProperties.ProcessBatch)]
        [BsonIgnoreIfNull]
        public string ProcessBatch { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessEquipmentName { get; set; }
        [BsonIgnoreIfNull]
        public string LongParameterName { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string FourDReport { get; set; }
        [BsonIgnoreIfNull]
        public string SpecialCharacteristics { get; set; }
        [BsonIgnoreIfNull]
        public string CFComment { get; set; }
        [BsonIgnoreIfNull]
        public string Machine { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterType { get; internal set; }
        [BsonIgnoreIfNull]
        public string ProcessOwner { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string Published { get; set; }
        [BsonIgnoreIfNull]
        public string GroupID { get; set; }
        [BsonIgnoreIfNull]
        public string Device { get; set; }
        [BsonIgnoreIfNull]
        public string PackageCF { get; set; }
    }
}
