using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.WAR.PADS.Module.Data.PADSModel
{
    public class DataFlatMetaDataPads : BaseDataFlatMetaDataPads
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
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
        public string ProcessStep { get; set; }
        [BsonIgnoreIfNull]
        public string CleaningType { get; set; }
        [BsonIgnoreIfNull]
        public string Robot { get; set; }
        [BsonIgnoreIfNull]
        public string CuringProcess { get; set; }
        [BsonIgnoreIfNull]
        public string BasePlatePosition { get; set; }
        [BsonIgnoreIfNull]
        public string CleaningChamber { get; set; }
        [BsonIgnoreIfNull]
        public string USPower { get; set; }
        [BsonIgnoreIfNull]
        public string ChipSolderingLine { get; set; }
        [BsonIgnoreIfNull]
        public string ShearSurface { get; set; }
        [BsonIgnoreIfNull]
        public string StencilThickness { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessType { get; set; }
        [BsonIgnoreIfNull]
        public string CarrierID { get; set; }
        [BsonIgnoreIfNull]
        public string BondParameter { get; set; }
        [BsonIgnoreIfNull]
        public string Surface { get; set; }
        [BsonIgnoreIfNull]
        public string SonotrodeNumber { get; set; }
        [BsonIgnoreIfNull]
        public string DataType { get; set; }
        [BsonIgnoreIfNull]
        public string FootPosition { get; set; }
        [BsonIgnoreIfNull]
        public string Zone { get; set; }
        [BsonIgnoreIfNull]
        public string CarrierFamily { get; set; }
        [BsonIgnoreIfNull]
        public string PadSize { get; set; }
        [BsonIgnoreIfNull]
        public string WireDiameter { get; set; }
        [BsonIgnoreIfNull]
        public string TerminalConnector { get; set; }
        [BsonIgnoreIfNull]
        public string TerminalType { get; set; }
        [BsonIgnoreIfNull]
        public string WireMaterial { get; set; }
        [BsonIgnoreIfNull]
        public string Material { get; set; }
        [BsonIgnoreIfNull]
        public string Identifier { get; set; }
        [BsonIgnoreIfNull]
        public string CarrierAnvilPosition { get; set; }
        [BsonIgnoreIfNull]
        public string PastePrinter { get; set; }
        [BsonIgnoreIfNull]
        public string VaduFrameID { get; set; }
    }
}
