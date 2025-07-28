using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.WUX.PADS.Module.Data.PADSModel
{
    public class StrangeDataFlatMetaDataPads : BaseStrangeDataFlatMetaDataPads
    {

        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        ///
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string SalesName { get; set; }
        [BsonIgnoreIfNull]
        public string BusinessSegment { get; set; }
        [BsonIgnoreIfNull]
        public string Wire { get; set; }

        // New properties
        [BsonIgnoreIfNull]
        public string Owner { get; set; }
        [BsonIgnoreIfNull]
        public string Coding { get; set; }
        [BsonIgnoreIfNull]
        public string DieCategory { get; set; }
        [BsonIgnoreIfNull]
        public string Wiresize { get; set; }
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
        public string ChipType { get; set; }
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
    }
}
