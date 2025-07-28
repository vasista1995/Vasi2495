using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.SpaceBE.WUX.Source.Module.Data.SpaceModel
{

    /// <summary>
    /// It takes all the values from the sql statements and converts them into the below properties.
    /// </summary>
    public class SpaceEntry : BaseSpaceEntry
    {
        public string Equipment { get; set; }
        public string ParameterClass { get; set; }
        public string TargetCpk { get; set; }
        public string Segment { get; set; }
        public string Module { get; set; }
        public string Package { get; set; }
        public string F56Parameter { get; set; }
        public string FourDReport { get; set; }
        public string SpecialCharacteristics { get; set; }
        public List<SpaceRawValuesEntry> SpaceRawValues { get; set; }

        // StrangeMetaData properties
        public string SalesName { get; set; }
        public string BusinessSegment { get; set; }
        public string Wire { get; set; }
        public string Coding { get; set; }
        public string DieCategory { get; set; }
        public string ChipType { get; set; }
        public string WireSize { get; set; }
        public string Track { get; set; }
        public string SolderGlue { get; set; }
        public string SawStreet { get; set; }
        public string WaferCharge { get; set; }
        public string BladeCategory { get; set; }
        public string DieSize { get; set; }
        public string SolderFeedLength { get; set; }
        public string WaferThickness { get; set; }
        public string Operator { get; set; }
        public string CartridgeID { get; set; }
        public string Grade { get; set; }
        public string ProcessOwner { get; set; }
        public string Published { get; set; }
        public string GroupId { get; set; }
        public string CFComment { get; set; }

        // FlatMetaData properties
        public string Baunumber { get; set; }
        public string GaugeID { get; set; }
        public string Line { get; set; }
        public string Shift { get; set; }
        public string Classification { get; set; }
        public string Process { get; set; }

        // RawValues
        public string Tool { get; set; }
    }
}
