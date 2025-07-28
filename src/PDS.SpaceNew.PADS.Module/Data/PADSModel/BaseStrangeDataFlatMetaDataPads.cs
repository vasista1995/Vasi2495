using MongoDB.Bson.Serialization.Attributes;

namespace PDS.SpaceNew.PADS.Module.Data.PADSModel
{
    public class BaseStrangeDataFlatMetaDataPads
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// This happens by extracting all of the properties defined in this section and assigning them dynamically
        /// </summary>
        ///
        [BsonIgnoreIfNull]
        public string BasicType { get; set; }
        [BsonIgnoreIfNull]
        public string ProductType { get; set; }
        [BsonIgnoreIfNull]
        public string Product { get; set; }
    }
}
