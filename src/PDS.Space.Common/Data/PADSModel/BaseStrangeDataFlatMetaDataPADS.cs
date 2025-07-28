using MongoDB.Bson.Serialization.Attributes;

namespace PDS.Space.Common.Data.PADSModel
{
    public class BaseStrangeDataFlatMetaDataPads
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
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
