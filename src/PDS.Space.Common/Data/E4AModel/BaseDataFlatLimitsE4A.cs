using MongoDB.Bson.Serialization.Attributes;

namespace PDS.Space.Common.Data.E4AModel
{
    public class BaseDataFlatLimitsE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to Dataflatlimits section in e4a document.
        /// </summary>
        [BsonIgnoreIfNull]
        public string CkcId { get; set; }
        [BsonIgnoreIfNull]
        public BaseMeasurementSpecLimitsE4A MeasurementSpecLimits { get; set; }
        [BsonIgnoreIfNull]
        public BaseSpaceAggregatesE4A SpaceAggregates { get; set; }
        [BsonIgnoreIfNull]
        public BaseControlLimitsE4A ControlLimits { get; set; }
    }
}
