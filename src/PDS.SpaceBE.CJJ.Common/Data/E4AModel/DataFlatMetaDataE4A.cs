using System.Diagnostics;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.CJJ.Common.Data.E4AModel
{
    public class DataFlatMetaDataE4A : BaseDataFlatMetaDataE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in e4a document.
        /// </summary>
        [BsonIgnoreIfNull]
        public string Machine { get; set; }
        [BsonIgnoreIfNull]
        public string Familypackage { get; set; }
        [BsonIgnoreIfNull]
        public string PackageFamily { get; set; }
        [BsonIgnoreIfNull]
        public string Classification { get; set; }
        [BsonIgnoreIfNull]
        public string WaferThickness { get; set; }
        [BsonIgnoreIfNull]
        public string Operator {get; set;}
        [BsonIgnoreIfNull]
        public string CartridgeID { get; set; }
        [BsonIgnoreIfNull]
        public string Grade { get; set; }
        [BsonIgnoreIfNull]
        public string Wafer { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string WireSize { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }
        [BsonIgnoreIfNull]
        public string CFComment { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string Device { get; set; }
        [BsonIgnoreIfNull]
        public string FourDReport { get; set; }
        [BsonIgnoreIfNull]
        public string SpecialCharacteristics { get; set; }
        [BsonIgnoreIfNull]
        public string Shift { get; set; }
        [BsonIgnoreIfNull]
        public string ChipType { get; set; }
        [BsonIgnoreIfNull]
        public string WireType { get; set; }
        [BsonIgnoreIfNull]
        public string Process { get; set; }

        // StrangeMetaData properties
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

        // FlatMetaData properties
        [BsonIgnoreIfNull]
        public string Baunumber { get; set; }
        [BsonIgnoreIfNull]
        public string OperatorId { get; set; }
        [BsonIgnoreIfNull]
        public string Material { get; set; }
    }
}
