using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.BAT.Common.Data.E4AModel
{
    public class DataFlatMetaDataE4A : BaseDataFlatMetaDataE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in e4a document.
        /// </summary>
        [BsonIgnoreIfNull]
        public string PackageType { get; set; }
        [BsonIgnoreIfNull]
        public string Device { get; set; }
        [BsonIgnoreIfNull]
        public string DieCategory { get; set; }
        [BsonIgnoreIfNull]
        public string DieSet { get; set; }
        [BsonIgnoreIfNull]
        public string PackageFamily { get; set; }
        [BsonIgnoreIfNull]
        public string GaugeId { get; set; }
        [BsonIgnoreIfNull]
        public string LeadFrame { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string Machine { get; set; }
        [BsonIgnoreIfNull]
        public string Operator { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string PackageClass { get; set; }
        [BsonIgnoreIfNull]
        public string PadMetal { get; set; }
        [BsonIgnoreIfNull]
        public string Process { get; set; }
        [BsonIgnoreIfNull]
        public string SawStreet { get; set; }
        [BsonIgnoreIfNull]
        public string Shift { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessTool { get; set; }
        [BsonIgnoreIfNull]
        public string Track { get; set; }
        [BsonIgnoreIfNull]
        public string WaferThickness { get; set; }
        [BsonIgnoreIfNull]
        public string WireSize { get; set; }
        [BsonIgnoreIfNull]
        public string Classification { get; set; }
        [BsonIgnoreIfNull]
        public string Submission { get; set; }
        [BsonIgnoreIfNull]
        public string Reserve2 { get; set; }
        [BsonIgnoreIfNull]
        public string ChipType { get; set; }
        [BsonIgnoreIfNull]
        public string Area { get; set; }
        [BsonIgnoreIfNull]
        public string Capilary { get; set; }
        [BsonIgnoreIfNull]
        public string McPlatform { get; set; }
        [BsonIgnoreIfNull]
        public string OperatorId { get; set; }
        [BsonIgnoreIfNull]
        public string MeasurementEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string BladeCategory { get; set; }
        [BsonIgnoreIfNull]
        public string SubmissionType { get; set; }
        [BsonIgnoreIfNull]
        public string SolderGlue { get; set; }
        [BsonIgnoreIfNull]
        public string DieSize { get; set; }
        [BsonIgnoreIfNull]
        public string Grade { get; set; }
        [BsonIgnoreIfNull]
        public string CartridgeID { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }
        [BsonIgnoreIfNull]
        public string CFComment { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessOwner { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string Published { get; set; }
        [BsonIgnoreIfNull]
        public string GroupId { get; set; }
        [BsonIgnoreIfNull]
        public string FourDReport { get; set; }
        [BsonIgnoreIfNull]
        public string SpecialCharacteristics { get; set; }
    }
}
