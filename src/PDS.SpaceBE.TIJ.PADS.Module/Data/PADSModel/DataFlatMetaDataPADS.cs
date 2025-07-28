using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.TIJ.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
    /// </summary>
    ///
    public class DataFlatMetaDataPads : BaseDataFlatMetaDataPads
    {
        [BsonIgnoreIfNull]
        public string Motherlot { get; set; }
        [BsonIgnoreIfNull]
        public string Wafer { get; set; }
        [BsonIgnoreIfNull]
        public string Classification { get; set; }

        // Only for TIJ site
        [BsonIgnoreIfNull]
        public string HexSize { get; set; }
        [BsonIgnoreIfNull]
        public string Layout { get; set; }
        [BsonIgnoreIfNull]
        public string LeadFormCategory { get; set; }
        [BsonIgnoreIfNull]
        public string MoldCompound { get; set; }
        [BsonIgnoreIfNull]
        public string PartNumber { get; set; }
        [BsonIgnoreIfNull]
        public string ProductionVersion { get; set; }
        [BsonIgnoreIfNull]
        public string SolderType { get; set; }
        [BsonIgnoreIfNull]
        public string Track { get; set; }
        [BsonIgnoreIfNull]
        public string Area { get; set; }
        [BsonIgnoreIfNull]
        public string Tool { get; set; }

    }
}
