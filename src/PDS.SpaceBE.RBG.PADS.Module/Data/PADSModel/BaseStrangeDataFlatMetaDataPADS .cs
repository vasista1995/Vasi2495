using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.RBG.PADS.Module.Data.PADSModel
{
    public class StrangeDataFlatMetaDataPads : BaseStrangeDataFlatMetaDataPads
    {

        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        ///
        [BsonIgnoreIfNull]
        public string EquipmentType { get; set; }
        [BsonIgnoreIfNull]
        public string SalesName { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string PackageFamily { get; set; }
        [BsonIgnoreIfNull]
        public string Device { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string BeSort { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string GroupId { get; set; }
        [BsonIgnoreIfNull]
        public string Wire { get; set; }
        [BsonIgnoreIfNull]
        public string PackageGroup { get; set; }
        [BsonIgnoreIfNull]
        public string PackageClass { get; set; }
        [BsonIgnoreIfNull]
        public string DeviceFamily { get; set; }
        [BsonIgnoreIfNull]
        public string UserClass1 { get; set; }
        [BsonIgnoreIfNull]
        public string Owner { get; set; }
        [BsonIgnoreIfNull]
        public string UserClass2 { get; set; }
        [BsonIgnoreIfNull]
        public string Group1 { get; set; }
        [BsonIgnoreIfNull]
        public string UserClass3 { get; set; }
        [BsonIgnoreIfNull]
        public string Group2 { get; set; }
        [BsonIgnoreIfNull]
        public string Group3 { get; set; }
        [BsonIgnoreIfNull]
        public string SampleType { get; set; }
        [BsonIgnoreIfNull]
        public string ProductName { get; set; }
        [BsonIgnoreIfNull]
        public string OriginSampleSize { get; set; }
        [BsonIgnoreIfNull]
        public string BeSegmentName { get; set; }
        [BsonIgnoreIfNull]
        public string ManufacturingWipLevel { get; set; }
        [BsonIgnoreIfNull]
        public string ErrorCode { get; set; }
        [BsonIgnoreIfNull]
        public string Pin { get; set; }
        [BsonIgnoreIfNull]
        public string Data1 { get; set; }
        [BsonIgnoreIfNull]
        public string Data2 { get; set; }
        [BsonIgnoreIfNull]
        public string Data3 { get; set; }
    }
}
