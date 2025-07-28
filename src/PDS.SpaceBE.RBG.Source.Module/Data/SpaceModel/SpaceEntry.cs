using System.Collections.Generic;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.SpaceBE.RBG.Source.Module.Data.SpaceModel
{

    /// <summary>
    /// It takes all the values from the sql statements and converts them into the below properties.
    /// </summary>
    public class SpaceEntry : BaseSpaceEntry
    {
        public string SalesName { get; set; }
        public string PackageFamily { get; set; }
        public string Device { get; set; }
        public string Module { get; set; }
        public string Package { get; set; }
        public string BeSort { get; set; }
        public string Material { get; set; }
        public string Segment { get; set; }
        public string EquipmentType { get; set; }
        public string GroupId { get; set; }
        public string Wire { get; set; }
        public string PackageGroup { get; set; }
        public string PackageClass { get; set; }
        public string DeviceFamily { get; set; }
        public string UserClass1 { get; set; }
        public string UserClass2 { get; set; }
        public string Group1 { get; set; }
        public string UserClass3 { get; set; }
        public string Group2 { get; set; }
        public string SpecName { get; set; }
        public string Group3 { get; set; }
        public string OriginSampleSize { get; set; }
        public string SampleType { get; set; }
        public string MoveInQuantity { get; set; }
        public string MoveOutQuantity { get; set; }
        public string OperatorId { get; set; }
        public string ProductName { get; set; }
        public string BeSegmentName { get; set; }
        public string ManufacturingWipLevel { get; set; }
        public string ErrorCode { get; set; }
        public string Pin { get; set; }
        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public string Data3 { get; set; }
        public string WaferLot { get; set; }
        public List<SpaceRawValuesEntry> SpaceRawValues { get; set; }

    }
}
