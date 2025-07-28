using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.WUX.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
    /// </summary>
    ///
    public class DataFlatMetaDataPads : BaseDataFlatMetaDataPads
    {
        [BsonIgnoreIfNull]
        public string Baunumber { get; set; }
        [BsonIgnoreIfNull]
        public string OperatorId { get; set; }
        [BsonIgnoreIfNull]
        public string GaugeID { get; set; }
        [BsonIgnoreIfNull]
        public string Line { get; set; }
        [BsonIgnoreIfNull]
        public string Shift { get; set; }
        [BsonIgnoreIfNull]
        public string Classification { get; set; }
        [BsonIgnoreIfNull]
        public string Process { get; set; }
    }
}
