using MongoDB.Bson.Serialization.Attributes;

namespace PDS.SpaceNew.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to Rawvalues array subsection of data1list section in pads document.
    /// </summary>
    public class Data1ListRawValuesPads4Wafer : BaseData1ListRawValuesPads, IData1ListRawValuesPadsGof
    {
        [BsonIgnoreIfNull]
        public string ProcessEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string GOF { get; set; }
        [BsonIgnoreIfNull]
        public string ItemIdMotherlotWafer { get; set; }
        [BsonIgnoreIfNull]
        public string MotherlotWafer { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterName { get; set; }
        [BsonIgnoreIfNull]
        public string ChannelId { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterUnit { get; set; }
        [BsonIgnoreIfNull]
        public string Slot { get; set; }
        [BsonIgnoreIfNull]
        public string WaferSequence { get; set; }
        [BsonIgnoreIfNull]
        public string TestPosition { get; set; }
    }
}
