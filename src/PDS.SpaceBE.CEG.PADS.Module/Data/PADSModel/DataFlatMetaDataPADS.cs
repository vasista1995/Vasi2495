using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.CEG.PADS.Module.Data.PADSModel
{
    public class DataFlatMetaDataPads : BaseDataFlatMetaDataPads
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        [BsonIgnoreIfNull]
        public string Plant { get; set; }
        [BsonIgnoreIfNull]
        public string Line { get; set; }
        [BsonIgnoreIfNull]
        public string ProductionOrder { get; set; }
        [BsonIgnoreIfNull]
        public string Batch { get; set; }
        [BsonIgnoreIfNull]
        public string MaterialNumber { get; set; }
        [BsonIgnoreIfNull]
        public string ProductGroup { get; set; }
        [BsonIgnoreIfNull]
        public string MeasurementRecipe { get; set; }
        [BsonIgnoreIfNull]
        public string OrderType { get; set; }
        [BsonIgnoreIfNull]
        public string QrkGroup { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessGroup { get; set; }
        [BsonIgnoreIfNull]
        public string SpcClass { get; set; }
        [BsonIgnoreIfNull]
        public string EquipmentName { get; set; }
        [BsonIgnoreIfNull]
        public string CustomComment { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string BondParameter { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterType { get; internal set; }
    }
}
