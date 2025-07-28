using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.WAR.Common.Data.E4AModel
{
    public class DataFlatMetaDataE4A : BaseDataFlatMetaDataE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in e4a document.
        /// </summary>
        ///
        public string Plant { get; set; }
        public string Line { get; set; }
        public string ProductionOrder { get; set; }
        public string Batch { get; set; }
        public string MaterialNumber { get; set; }
        public string ProductGroup { get; set; }
        public string MeasurementRecipe { get; set; }
        public string ProcessEquipment { get; set; }
        public string OrderType { get; set; }
        public string QrkGroup { get; set; }
        public string ProcessGroup { get; set; }
        public string SpcClass { get; set; }
        public string ParameterType { get; set; }
        public string LongParameterName { get; set; }
        public string MaterialNumberText { get; set; }
        public string EquipmentName { get; set; }
        public string ProcessEquipmentName { get; set; }
        public string ProductName { get; set; }
        public string RawValPos { get; set; }
        public string LengthX { get; set; }
        public string Operator { get; set; }
        public string Status { get; set; }
        public string UnitLength { get; set; }
        public string ParameterClass { get; set; }
        public string TargetCpk { get; set; }
        public string ProcessOwner { get; set; }
        public string Segment { get; set; }
        public string Package { get; set; }
        public string F56Parameter { get; set; }
        public string GroupId { get; set; }
        public string Report { get; set; }
        public string Module { get; set; }
        public string ProcessStep { get; set; }
        public string CleaningType { get; set; }
        public string Robot { get; set; }
        public string CuringProcess { get; set; }
        public string BasePlatePosition { get; set; }
        public string CleaningChamber { get; set; }
        public string USPower { get; set; }
        public string ChipSolderingLine { get; set; }
        public string ShearSurface { get; set; }
        public string StencilThickness { get; set; }
        public string ProcessType { get; set; }
        public string CarrierID { get; set; }
        public string Surface { get; set; }
        public string SonotrodeNumber { get; set; }
        public string DataType { get; set; }
        public string FootPosition { get; set; }
        public string Zone { get; set; }
        public string CarrierFamily { get; set; }
        public string BondParameter { get; set; }
        public string PadSize { get; set; }
        public string WireDiameter { get; set; }
        public string TerminalConnector { get; set; }
        public string TerminalType { get; set; }
        public string WireMaterial { get; set; }
        public string Material { get; set; }
        public string Identifier { get; set; }
        public string CarrierAnvilPosition { get; set; }
        public string PastePrinter { get; set; }
        public string VaduFrameID { get; set; }
    }
}
