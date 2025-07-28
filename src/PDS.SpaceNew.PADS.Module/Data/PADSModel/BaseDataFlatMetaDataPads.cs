using System;
using MongoDB.Bson.Serialization.Attributes;

namespace PDS.SpaceNew.PADS.Module.Data.PADSModel
{
    public class BaseDataFlatMetaDataPads
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        [BsonIgnoreIfNull]
        public string MeasLot { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterOper { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterFacility { get; set; }
        [BsonIgnoreIfNull]
        public string SiteKey { get; set; }
        [BsonIgnoreIfNull]
        public string SpaceInstanceName { get; set; }
        [BsonIgnoreIfNull]
        public DateTime BeginTimestampUtc { get; set; }
        [BsonIgnoreIfNull]
        public DateTime BeginTimestamp { get; set; }

        [BsonIgnoreIfNull]
        public DateTime EndTimestampUtc { get; set; }
        [BsonIgnoreIfNull]
        public DateTime EndTimestamp { get; set; }
        [BsonIgnoreIfNull]
        public string Route { get; set; }
        [BsonIgnoreIfNull]
        public string Equipment { get; set; }
    }
}
