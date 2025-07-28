using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.RBG.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
    /// </summary>
    ///
    public class DataFlatMetaDataPads : BaseDataFlatMetaDataPads
    {
        [BsonIgnoreIfNull]
        public string Material { get; set; }
        [BsonIgnoreIfNull]
        public string OperatorId { get; set; }
        [BsonIgnoreIfNull]
        public string Motherlot { get; set; }
        [BsonIgnoreIfNull]
        public string Wafer { get; set; }
        [BsonIgnoreIfNull]
        public string SpecName { get; set; }

    }
}
