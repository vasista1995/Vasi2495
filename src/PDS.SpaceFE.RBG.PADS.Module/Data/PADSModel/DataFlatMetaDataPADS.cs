using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel
{
    public class DataFlatMetaDataPads : BaseDataFlatMetaDataPads
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        ///
        [BsonIgnoreIfNull]
        public string Motherlot { get; set; }
        [BsonIgnoreIfNull]
        public string Wafer { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.SPSNumber)]
        [BsonElement(SpacePadsProperties.SPSNumber)]
        public string SPSNumber { get; set; }
        [BsonIgnoreIfNull]
        public string SubOperation { get; set; }
        [BsonIgnoreIfNull]
        public string Recipe { get; set; }
        [BsonIgnoreIfNull]
        public string EquipmentType { get; set; }
        [BsonIgnoreIfNull]
        public string Slot { get; set; }
        [BsonIgnoreIfNull]
        public string WaferSequence { get; set; }
        [BsonIgnoreIfNull]
        public string TestPosition { get; set; }
    }
}
