using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.MKZ.PADS.Module.Data.PADSModel
{
    public class StrangeDataFlatMetaDataPads : BaseStrangeDataFlatMetaDataPads
    {

        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        ///
        [BsonIgnoreIfNull]
        public string Track { get; set; }
        [BsonIgnoreIfNull]
        public string ToolID { get; set; }
        [BsonIgnoreIfNull]
        public string PackageKeys { get; set; }
        [BsonIgnoreIfNull]
        public string Category { get; set; }
        [BsonIgnoreIfNull]
        public string LeadFrame { get; set; }
        [BsonIgnoreIfNull]
        public string WireSize { get; set; }
        [BsonIgnoreIfNull]
        public string DieSize { get; set; }
        [BsonIgnoreIfNull]
        public string SolderGlue { get; set; }
        [BsonIgnoreIfNull]
        public string Eintrag { get; set; }
        [BsonIgnoreIfNull]
        public string DeviceKeys { get; set; }
        [BsonIgnoreIfNull]
        public string Classification2 { get; set; }
        [BsonIgnoreIfNull]
        public string Line { get; set; }
        [BsonIgnoreIfNull]
        public string FamilyPackage { get; set; }
        [BsonIgnoreIfNull]
        public string WireType { get; set; }
        [BsonIgnoreIfNull]
        public string SubLine { get; set; }
        [BsonIgnoreIfNull]
        public string DieCategory { get; set; }
        [BsonIgnoreIfNull]
        public string ChipType { get; set; }
        [BsonIgnoreIfNull]
        public string DefectCategory { get; set; }
        [BsonIgnoreIfNull]
        public string Characteristics { get; set; }
        [BsonIgnoreIfNull]
        public string OriginOfRC { get; set; }
        [BsonIgnoreIfNull]
        public string Type { get; set; }
        [BsonIgnoreIfNull]
        public string GroupKeys { get; set; }
        [BsonIgnoreIfNull]
        public string ModuleKeys { get; set; }
        [BsonIgnoreIfNull]
        public string SawStreet { get; set; }
        [BsonIgnoreIfNull]
        public string WaferThickness { get; set; }
        [BsonIgnoreIfNull]
        public string BladeCategory { get; set; }
        [BsonIgnoreIfNull]
        public string WaferCharge { get; set; }
        [BsonIgnoreIfNull]
        public string SpcTool { get; set; }
        [BsonIgnoreIfNull]
        public string Variation { get; set; }
        [BsonIgnoreIfNull]
        public string Operator { get; set; }
        [BsonIgnoreIfNull]
        public string CartridgeID { get; set; }
        [BsonIgnoreIfNull]
        public string Grade { get; set; }
        [BsonIgnoreIfNull]
        public string SolderFeed { get; set; }
        [BsonIgnoreIfNull]
        public string Coding { get; set; }
        [BsonIgnoreIfNull]
        public string SizeKeys { get; set; }
        [BsonIgnoreIfNull]
        public string GaugeID2 { get; set; }
        [BsonIgnoreIfNull]
        public string NumberKeys { get; set; }
        [BsonIgnoreIfNull]
        public string SolderFeedLength { get; set; }
        [BsonIgnoreIfNull]
        public string RasterX { get; set; }
        [BsonIgnoreIfNull]
        public string RasterY { get; set; }
        [BsonIgnoreIfNull]
        public string Magnification { get; set; }
        [BsonIgnoreIfNull]
        public string Machine1 { get; set; }
            
    }
}
