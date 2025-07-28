using MongoDB.Bson.Serialization.Attributes;

namespace PDS.Space.Common.Data.E4AModel
{
    public class BaseMeasurementSpecLimitsE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to speclimts subsection of DataFlatlimits section in e4a document.
        /// </summary>

        [BsonIgnoreIfNull]
        public string SpecHighEnabled { get; set; }
        [BsonIgnoreIfNull]
        public string SpecLowEnabled { get; set; }
        [BsonIgnoreIfNull]
        public double? SpecHigh { get; set; }
        [BsonIgnoreIfNull]
        public double? SpecLow { get; set; }
        [BsonIgnoreIfNull]
        public double? SpecTarget { get; set; }
        [BsonIgnoreIfNull]
        public string SpecTargetOrigin { get; internal set; }
        [BsonIgnoreIfNull]
        public string ExtSpecLimEnable { get; internal set; }
        [BsonIgnoreIfNull]
        public double? ExtSpecUpper { get; internal set; }
        [BsonIgnoreIfNull]
        public double? ExtSpecLower { get; internal set; }
        [BsonIgnoreIfNull]
        public string LimitEnable { get; set; }
        public BaseMeasurementSpecLimitsE4A()
        { }
    }
}
