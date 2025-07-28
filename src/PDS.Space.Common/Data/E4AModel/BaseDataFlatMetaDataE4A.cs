using System;
using MongoDB.Bson.Serialization.Attributes;

namespace PDS.Space.Common.Data.E4AModel
{
    public class BaseDataFlatMetaDataE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in e4a document.
        /// </summary>
        [BsonIgnoreIfNull]
        public string Lot { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterName { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterUnit { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterOper { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterFacility { get; set; }
        [BsonIgnoreIfNull]
        public string SiteKey { get; set; }
        [BsonIgnoreIfNull]
        public string ChannelId { get; set; }
        [BsonIgnoreIfNull]
        public string SpaceInstanceName { get; set; }
        [BsonIgnoreIfNull]
        public DateTime CreatedTimestampUtc { get; set; }
        [BsonIgnoreIfNull]
        public DateTime CreatedTimestamp { get; set; }
        [BsonIgnoreIfNull]
        public DateTime UpdatedTimestamp { get; set; }
        [BsonIgnoreIfNull]
        public DateTime UpdatedTimestampUtc { get; set; }
        [BsonIgnoreIfNull]
        public DateTime ExportedTimestampUtc { get; set; }
        [BsonIgnoreIfNull]
        public DateTime ExportedTimestamp { get; set; }
        [BsonIgnoreIfNull]
        public DateTime SampleTimestampUtc { get; set; }
        [BsonIgnoreIfNull]
        public DateTime SampleTimestamp { get; set; }
        [BsonIgnoreIfNull]
        public string SourceDataLevel { get; set; }
        [BsonIgnoreIfNull]
        public string BasicType { get; set; }
        [BsonIgnoreIfNull]
        public string ProductType { get; set; }
        [BsonIgnoreIfNull]
        public string Product { get; set; }
        [BsonIgnoreIfNull]
        public string Route { get; set; }
        [BsonIgnoreIfNull]
        public string Equipment { get; set; }
        [BsonIgnoreIfNull]
        public string ChannelName { get; set; }
        [BsonIgnoreIfNull]
        public string ChannelDescr { get; set; }
        [BsonIgnoreIfNull]
        public string RvStoreFlag { get; set; }
        [BsonIgnoreIfNull]
        public string CalcCreated { get; set; }
        [BsonIgnoreIfNull]
        public long ILdsID { get; set; }
        [BsonIgnoreIfNull]
        public string SpcComExtern { get; set; }
        [BsonIgnoreIfNull]
        public string SpcComIntern { get; set; }
        [BsonIgnoreIfNull]
        public string AcceptFlag { get; set; }
        [BsonIgnoreIfNull]
        public string UpdatedBy { get; set; }
        [BsonIgnoreIfNull]
        public long ClOrigin { get; set; }
        [BsonIgnoreIfNull]
        public string OutOfOrder { get; set; }
        [BsonIgnoreIfNull]
        public long Calc2ID { get; set; }
        [BsonIgnoreIfNull]
        public int? ViolCount { get; set; }
        [BsonIgnoreIfNull]
        public string Owner { get; set; }
        [BsonIgnoreIfNull]
        public string CalculationStrategy { get; set; }
        [BsonIgnoreIfNull]
        public string LdsID { get; set; }
        [BsonIgnoreIfNull]
        public long IsCalcParam { get; set; }
        [BsonIgnoreIfNull]
        public string ChannelType { get; set; }
        [BsonIgnoreIfNull]
        public string ChState { get; set; }

    }
}
