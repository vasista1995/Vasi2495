using System.Diagnostics;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.MKZ.Common.Data.E4AModel
{
    public class DataFlatMetaDataE4A : BaseDataFlatMetaDataE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in e4a document.
        /// </summary>
        [BsonIgnoreIfNull]
        public string Machine { get; set; }
        [BsonIgnoreIfNull]
        public string Track { get; set; }
        [BsonIgnoreIfNull]
        public string PackageKeys { get; set; }
        [BsonIgnoreIfNull]
        public string Category { get; set; }
        [BsonIgnoreIfNull]
        public string LeadFrame { get; set; }
        [BsonIgnoreIfNull]
        public string Eintrag { get; set; }
        [BsonIgnoreIfNull]
        public string Classification2 { get; set; }
        [BsonIgnoreIfNull]
        public string Familypackage { get; set; }
        [BsonIgnoreIfNull]
        public string FamilyPackage { get; set; }
        [BsonIgnoreIfNull]
        public string DieCategory { get; set; }
        [BsonIgnoreIfNull]
        public string Classification { get; set; }
        [BsonIgnoreIfNull]
        public string DefectCategory { get; set; }
        [BsonIgnoreIfNull]
        public string Characteristics { get; set; }
        [BsonIgnoreIfNull]
        public string SawStreet { get; set; }
        [BsonIgnoreIfNull]
        public string WaferThickness { get; set; }
        [BsonIgnoreIfNull]
        public string BladeCategory { get; set; }
        [BsonIgnoreIfNull]
        public string WaferCharge { get; set; }
        [BsonIgnoreIfNull]
        public string PadSize { get; set; }
        [BsonIgnoreIfNull]
        public string Variation { get; set; }
        [BsonIgnoreIfNull]
        public string GaugeID { get; set; }
        [BsonIgnoreIfNull]
        public string Operator {get; set;}
        [BsonIgnoreIfNull]
        public string CartridgeID { get; set; }
        [BsonIgnoreIfNull]
        public string Grade { get; set; }
        [BsonIgnoreIfNull]
        public string GaugeID2 { get; set; }
        [BsonIgnoreIfNull]
        public string Remark { get; set; }
        [BsonIgnoreIfNull]
        public string Wafer { get; set; }
        [BsonIgnoreIfNull]
        public string RasterX { get; set; }
        [BsonIgnoreIfNull]
        public string RasterY { get; set; }
        [BsonIgnoreIfNull]
        public string Magnification { get; set; }
        [BsonIgnoreIfNull]
        public string Machine1 { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string Tool { get; set; }
        [BsonIgnoreIfNull]
        public string WireSize { get; set; }
        [BsonIgnoreIfNull]
        public string SolderFeedLength { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }
        [BsonIgnoreIfNull]
        public string CFComment { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessOwner { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string Published { get; set; }
        [BsonIgnoreIfNull]
        public string GroupID { get; set; }
        [BsonIgnoreIfNull]
        public string Device { get; set; }
        [BsonIgnoreIfNull]
        public string FourDReport { get; set; }
        [BsonIgnoreIfNull]
        public string SpecialCharacteristics { get; set; }
        [BsonIgnoreIfNull]
        public string SpcTool { get; set; }
        [BsonIgnoreIfNull]
        public string ToolId { get; set; }
        [BsonIgnoreIfNull]
        public string Shift { get; set; }
        [BsonIgnoreIfNull]
        public string SolderFeed { get; set; }
        [BsonIgnoreIfNull]
        public string Coding { get; set; }
        [BsonIgnoreIfNull]
        public string SizeKeys { get; set; }
        [BsonIgnoreIfNull]
        public string NumberKeys { get; set; }
        [BsonIgnoreIfNull]
        public string ModuleKeys { get; set; }
        [BsonIgnoreIfNull]
        public string GroupKeys { get; set; }
        [BsonIgnoreIfNull]
        public string Wire { get; set; }
        [BsonIgnoreIfNull]
        public string Type { get; set; }
        [BsonIgnoreIfNull]
        public string OriginOfRC { get; set; }
        [BsonIgnoreIfNull]
        public string ChipType { get; set; }
        [BsonIgnoreIfNull]
        public string SubLine { get; set; }
        [BsonIgnoreIfNull]
        public string WireType { get; set; }
        [BsonIgnoreIfNull]
        public string Process { get; set; }
        [BsonIgnoreIfNull]
        public string Line { get; set; }
        [BsonIgnoreIfNull]
        public string DeviceKeys { get; set; }
        [BsonIgnoreIfNull]
        public string SolderGlue { get; set; }
        [BsonIgnoreIfNull]
        public string DieSize { get; set; }
    }
}
