using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.WAR.PADS.Module.Data.PADSModel
{
    public class StrangeDataFlatMetaDataPads : BaseStrangeDataFlatMetaDataPads
    {

        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        ///
        [BsonIgnoreIfNull]
        public string MaterialNumberText { get; set; }
        [BsonIgnoreIfNull]
        public string ProductName { get; set; }
        [BsonIgnoreIfNull]
        public string RawValPos { get; set; }
        [BsonIgnoreIfNull]
        public string LengthX { get; set; }
        [BsonIgnoreIfNull]
        public string Operator { get; set; }
        [BsonIgnoreIfNull]
        public string Status { get; set; }
        [BsonIgnoreIfNull]
        public string UnitLength { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessOwner { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string GroupId { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string Report { get; set; }
    }
}
