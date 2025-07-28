using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to Rawvalues array subsection of data1list section in pads document.
    /// </summary>
    public class Data1ListRawValuesPads : BaseData1ListRawValuesPads , IData1ListRawValuesPadsGof
    {
        [JsonProperty(SpacePadsProperties.ProcessEquipment)]
        [BsonElement(SpacePadsProperties.ProcessEquipment)]
        [BsonIgnoreIfNull]
        public string ProcessEquipment { get; set; }
        [BsonIgnoreIfNull]
        [BsonElement(SpacePadsProperties.ItemIdMotherlotWafer)]
        [JsonProperty(SpacePadsProperties.ItemIdMotherlotWafer)]
        public string ItemIdMotherlotWafer { get; set; }
        [BsonIgnoreIfNull]
        [BsonElement(SpacePadsProperties.ItemIDFEProberChipID)]
        [JsonProperty(SpacePadsProperties.ItemIDFEProberChipID)]
        public string ItemIDFEProberChipID { get; set; }
        [BsonIgnoreIfNull]
        public string Wafer { get; set; }
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
        
    }
}
