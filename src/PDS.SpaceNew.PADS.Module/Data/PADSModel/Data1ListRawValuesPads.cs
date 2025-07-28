using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PDS.Space.Common.Data.PADSModel;
using System;

namespace PDS.SpaceNew.PADS.Module.Data.PADSModel
{
    public class Data1ListRawValuesPads : BaseData1ListRawValuesPads, IData1ListRawValuesPadsGof
    {
        // Attributes which are filled site specific
        [BsonIgnoreIfNull]
        public string ProcessTool { get; set; }
        [JsonProperty(SpacePadsProperties.ProcessEquipment)]
        [BsonElement(SpacePadsProperties.ProcessEquipment)]
        [BsonIgnoreIfNull]
        public string ProcessEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string MeasurementEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string MeasurementEquipmentName { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessEquipmentName { get; set; }
        [BsonIgnoreIfNull]
        public string MotherlotWafer { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterName { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterUnit { get; set; }
        [BsonIgnoreIfNull]
        public string Wafer { get; internal set; }
        [BsonIgnoreIfNull]
        [BsonElement(SpacePadsProperties.ItemIdMotherlotWafer)]
        [JsonProperty(SpacePadsProperties.ItemIdMotherlotWafer)]
        public string ItemIdMotherlotWafer { get; set; }
        [BsonIgnoreIfNull]
        [BsonElement(SpacePadsProperties.ItemIDFEProberChipID)]
        [JsonProperty(SpacePadsProperties.ItemIDFEProberChipID)]
        public string ItemIDFEProberChipID { get; set; }
        [BsonIgnoreIfNull]
        public string X { get; set; }
        [BsonIgnoreIfNull]
        public string Y { get; set; }
        [BsonIgnoreIfNull]
        public string GOF { get; set; }
        [BsonIgnoreIfNull]
        public string Slot { get; set; }
        [BsonIgnoreIfNull]
        public string WaferSequence { get; set; }
        [BsonIgnoreIfNull]
        public string TestPosition { get; set; }
        [BsonIgnoreIfNull]
        public string ChannelId { get; set; }
    }
}
