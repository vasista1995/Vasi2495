using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.TIJ.PADS.Module.Data.PADSModel
{
    public class StrangeDataFlatMetaDataPads : BaseStrangeDataFlatMetaDataPads
    {

        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        ///
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string PackageFamily { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string Owner { get; set; }

        // Consistent for multiple BE sites
        [BsonIgnoreIfNull]
        public string Category { get; set; }
        [BsonIgnoreIfNull]
        public string DieSize { get; set; }
        [BsonIgnoreIfNull]
        public string Operator { get; set; }
        [BsonIgnoreIfNull]
        public string WireSize { get; set; }
        [BsonIgnoreIfNull]
        public string WaferThickness { get; set; }
    }
}
