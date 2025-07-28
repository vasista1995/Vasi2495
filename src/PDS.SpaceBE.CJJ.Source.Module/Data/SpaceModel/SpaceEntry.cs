using System.Collections.Generic;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.SpaceBE.CJJ.Source.Module.Data.SpaceModel
{

    /// <summary>
    /// It takes all the values from the sql statements and converts them into the below properties.
    /// </summary>
    public class SpaceEntry : BaseSpaceEntry
    {
        public string Machine { get; set; }
        public string WireSize { get; set; }
        public string Process { get; set; }
        public string PackageFamily { get; set; }
        public string WireType { get; set; }
        public string ChipType { get; set; }
        public string Classification { get; set; }
        public string WaferThickness { get; set; }
        public string MeasurementEquipment { get; set; }
        public string Operator { get; set; }
        public string Shift { get; set; }
        public string CartridgeID { get; set; }
        public string Grade { get; set; }
        public string ParameterClass { get; set; }
        public string CFComment { get; set; }
        public string TargetCpk { get; set; }
        public string Segment { get; set; }
        public string Module { get; set; }
        public string Package { get; set; }
        public string F56Parameter { get; set; }
        public string Device { get; set; }
        public string FourDReport { get; set; }
        public string SpecialCharacteristics { get; set; }
        public List<SpaceRawValuesEntry> SpaceRawValues { get; set; }

        // StrangeMetaData properties
        public string SpecName { get; set; }
        public string SalesName { get; set; }
        public string DeviceFamily { get; set; }
        public string PackageGroup { get; set; }
        public string PackageClass { get; set; }
        public string BusinessSegment { get; set; }
        public string MoveInQuantity { get; set; }
        public string MoveOutQuantity { get; set; }
        public string ProcessOwner { get; set; }
        public string BeSegmentName { get; set; }
        public string ManufacturingWipLevel { get; set; }
        public string MaterialType { get; set; }
        public string Published { get; set; }
        public string GroupId { get; set; }

        // FlatMetaData properties
        public string Baunumber { get; set; }
        public string OperatorId { get; set; }
        public string Material { get; set; }
    }
}
