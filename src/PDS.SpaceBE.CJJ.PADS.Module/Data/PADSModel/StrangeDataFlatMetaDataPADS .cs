using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.CJJ.PADS.Module.Data.PADSModel
{
    public class StrangeDataFlatMetaDataPads : BaseStrangeDataFlatMetaDataPads
    {

        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        ///
        [BsonIgnoreIfNull]
        public string WireSize { get; set; }
        [BsonIgnoreIfNull]
        public string PackageFamily { get; set; }
        [BsonIgnoreIfNull]
        public string WireType { get; set; }
        [BsonIgnoreIfNull]
        public string ChipType { get; set; }
        [BsonIgnoreIfNull]
        public string WaferThickness { get; set; }
        [BsonIgnoreIfNull]
        public string Operator { get; set; }
        [BsonIgnoreIfNull]
        public string CartridgeID { get; set; }
        [BsonIgnoreIfNull]
        public string Grade { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string Device { get; set; }



        [BsonIgnoreIfNull]
        public string SpecName { get; set; }
        [BsonIgnoreIfNull]
        public string SalesName { get; set; }
        [BsonIgnoreIfNull]
        public string DeviceFamily { get; set; }
        [BsonIgnoreIfNull]
        public string PackageGroup { get; set; }
        [BsonIgnoreIfNull]
        public string PackageClass { get; set; }
        [BsonIgnoreIfNull]
        public string BusinessSegment { get; set; }
        [BsonIgnoreIfNull]
        public string MoveInQuantity { get; set; }
        [BsonIgnoreIfNull]
        public string MoveOutQuantity { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessOwner { get; set; }
        [BsonIgnoreIfNull]
        public string BeSegmentName { get; set; }
        [BsonIgnoreIfNull]
        public string ManufacturingWipLevel { get; set; }
        [BsonIgnoreIfNull]
        public string MaterialType { get; set; }
        [BsonIgnoreIfNull]
        public string Published { get; set; }
        [BsonIgnoreIfNull]
        public string GroupId { get; set; }
    }
}
