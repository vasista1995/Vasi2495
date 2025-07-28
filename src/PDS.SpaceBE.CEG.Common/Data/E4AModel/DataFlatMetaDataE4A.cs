using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.CEG.Common.Data.E4AModel
{
    public class DataFlatMetaDataE4A : BaseDataFlatMetaDataE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in e4a document.
        /// </summary>
        ///
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
        public string ProcessEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string OrderType { get; set; }
        [BsonIgnoreIfNull]
        public string QrkGroup { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessGroup { get; set; }
        [BsonIgnoreIfNull]
        public string SpcClass { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterType { get; set; }
        [BsonIgnoreIfNull]
        public string LongParameterName { get; set; }
        [BsonIgnoreIfNull]
        public string MaterialNumberText { get; set; }
        [BsonIgnoreIfNull]
        public string EquipmentName { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessEquipmentName { get; set; }
        [BsonIgnoreIfNull]
        public string ProductName { get; set; }
        [BsonIgnoreIfNull]
        public string Length { get; set; }
        [BsonIgnoreIfNull]
        public string Operator { get; set; }
        [BsonIgnoreIfNull]
        public string UnitLength { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessOwner { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string GroupId { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string PosName { get; set; }
        [BsonIgnoreIfNull]
        public string OCAPEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string Published { get; set; }
        [BsonIgnoreIfNull]
        public string SpecialCharacteristics { get; set; }
        [BsonIgnoreIfNull]
        public string FourDReport { get; set; }
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
        public string BondParameter { get; set; }
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
