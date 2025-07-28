using System.Collections.Generic;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.SpaceBE.WAR.Source.Module.Data.SpaceModel
{
    /// <summary>
    /// It takes all the values from the sql statements and converts them into the below properties.
    /// </summary>
    public class SpaceEntry : BaseSpaceEntry
    {
        public string Plant { get; set; }
        public string IdSystemName { get; set; }
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
        public string MeasurementEquipmentName { get; set; }
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
        public string VariableExtr1 { get; set; }
        public string VariableExtr2 { get; set; }
        public string VariableExtr3 { get; set; }
        public string VariableExtr4 { get; set; }
        public string VariableExtr5 { get; set; }
        public string VariableExtr6 { get; set; }
        public string Label4Extr1 { get; set; }
        public string Label4Extr2 { get; set; }
        public string Label4Extr3 { get; set; }
        public string Label4Extr4 { get; set; }
        public string Label4Extr5 { get; set; }
        public string Label4Extr6 { get; set; }
        public List<SpaceRawValuesEntry> SpaceRawValues { get; set; }
    }
}
