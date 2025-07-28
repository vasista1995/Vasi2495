using System.Collections.Generic;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.SpaceBE.MKZ.Source.Module.Data.SpaceModel
{

    /// <summary>
    /// It takes all the values from the sql statements and converts them into the below properties.
    /// </summary>
    public class SpaceEntry : BaseSpaceEntry
    {
        public string Machine { get; set; }
        public string Track { get; set; }
        public string Tool { get; set; }
        public string PackageKeys { get; set; }
        public string Category { get; set; }
        public string LeadFrame { get; set; }
        public string WireSize { get; set; }
        public string DieSize { get; set; }
        public string SolderGlue { get; set; }
        public string Eintrag { get; set; }
        public string DeviceKeys { get; set; }
        public string Classification2 { get; set; }
        public string Line { get; set; }
        public string Process { get; set; }
        public string FamilyPackage { get; set; }
        public string WireType { get; set; }
        public string SubLine { get; set; }
        public string DieCategory { get; set; }
        public string ChipType { get; set; }
        public string Classification { get; set; }
        public string DefectCategory { get; set; }
        public string Characteristics { get; set; }
        public string OriginOfRC { get; set; }
        public string Type { get; set; }
        public string Wire { get; set; }
        public string GroupKeys { get; set; }
        public string ModuleKeys { get; set; }
        public string SawStreet { get; set; }
        public string WaferThickness { get; set; }
        public string BladeCategory { get; set; }
        public string Equipment { get; set; }
        public string WaferCharge { get; set; }
        public string PadSize { get; set; }
        public string SpcTool { get; set; }
        public string ToolId { get; set; }
        public string Variation { get; set; }
        public string GaugeID { get; set; }
        public string Operator { get; set; }
        public string Shift { get; set; }
        public string CartridgeID { get; set; }
        public string Grade { get; set; }
        public string SolderFeed { get; set; }
        public string Coding { get; set; }
        public string SizeKeys { get; set; }
        public string GaugeID2 { get; set; }
        public string NumberKeys { get; set; }
        public string Remark { get; set; }
        public string SolderFeedLength { get; set; }
        public string RasterX { get; set; }
        public string RasterY { get; set; }
        public string WaferNumber { get; set; }
        public string Magnification { get; set; }
        public string Machine1 { get; set; }
        public string ParameterClass { get; set; }
        public string CFComment { get; set; }
        public string TargetCpk { get; set; }
        public string ProcessOwner { get; set; }
        public string Segment { get; set; }
        public string Module { get; set; }
        public string Package { get; set; }
        public string F56Parameter { get; set; }
        public string Published { get; set; }
        public string GroupID { get; set; }
        public string Device { get; set; }
        public string FourDReport { get; set; }
        public string SpecialCharacteristics { get; set; }

        public List<SpaceRawValuesEntry> SpaceRawValues { get; set; }

    }
}
