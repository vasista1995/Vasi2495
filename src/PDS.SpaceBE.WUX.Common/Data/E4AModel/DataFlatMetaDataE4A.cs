using System.Diagnostics;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.WUX.Common.Data.E4AModel
{
    public class DataFlatMetaDataE4A : BaseDataFlatMetaDataE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in e4a document.
        /// </summary>
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string FourDReport { get; set; }
        [BsonIgnoreIfNull]
        public string SpecialCharacteristics { get; set; }
        [BsonIgnoreIfNull]
        public string ChipType { get; set; }

        // StrangeMetaData properties
        [BsonIgnoreIfNull]
        public string SalesName { get; set; }
        [BsonIgnoreIfNull]
        public string Wire { get; set; }
        [BsonIgnoreIfNull]
        public string BusinessSegment { get; set; }
        [BsonIgnoreIfNull]
        public string Coding { get; set; }
        [BsonIgnoreIfNull]
        public string DieCategory { get; set; }
        [BsonIgnoreIfNull]
        public string WireSize { get; set; }
        [BsonIgnoreIfNull]
        public string Track { get; set; }
        [BsonIgnoreIfNull]
        public string SolderGlue { get; set; }
        [BsonIgnoreIfNull]
        public string SawStreet { get; set; }
        [BsonIgnoreIfNull]
        public string WaferCharge { get; set; }
        [BsonIgnoreIfNull]
        public string BladeCategory { get; set; }
        [BsonIgnoreIfNull]
        public string DieSize { get; set; }
        [BsonIgnoreIfNull]
        public string SolderFeedLength { get; set; }
        [BsonIgnoreIfNull]
        public string WaferThickness { get; set; }
        [BsonIgnoreIfNull]
        public string Operator { get; set; }
        [BsonIgnoreIfNull]
        public string CartridgeID { get; set; }
        [BsonIgnoreIfNull]
        public string Grade { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessOwner { get; set; }
        [BsonIgnoreIfNull]
        public string Published { get; set; }
        [BsonIgnoreIfNull]
        public string GroupId { get; set; }
        [BsonIgnoreIfNull]
        public string CFComment { get; set; }

        // FlatMetaData properties
        [BsonIgnoreIfNull]
        public string Baunumber { get; set; }
        [BsonIgnoreIfNull]
        public string Material { get; set; }
        [BsonIgnoreIfNull]
        public string GaugeID { get; set; }
        [BsonIgnoreIfNull]
        public string Line { get; set; }
        [BsonIgnoreIfNull]
        public string Shift { get; set; }
        [BsonIgnoreIfNull]
        public string Classification { get; set; }
        [BsonIgnoreIfNull]
        public string Process { get; set; }

        // RawValues
        [BsonIgnoreIfNull]
        public string Tool { get; set; }
    }
}
