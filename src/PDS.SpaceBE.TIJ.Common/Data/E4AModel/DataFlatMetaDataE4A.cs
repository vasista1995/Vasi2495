using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.TIJ.Common.Data.E4AModel
{
    public class DataFlatMetaDataE4A : BaseDataFlatMetaDataE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in e4a document.
        /// </summary>

        [BsonIgnoreIfNull]
        public string PackageFamily { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }

        //Consistent in all BE sites
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string FourDReport { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string SpecialCharacteristics { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string PackageCF { get; set; }


        // Consistent in multiple sites
        [BsonIgnoreIfNull]
        public string Category { get; set; }
        [BsonIgnoreIfNull]
        public string Classification { get; set; }
        [BsonIgnoreIfNull]
        public string DieSize { get; set; }
        [BsonIgnoreIfNull]
        public string Operator { get; set; }
        [BsonIgnoreIfNull]
        public string WireSize { get; set; }
        [BsonIgnoreIfNull]
        public string WaferThickness { get; set; }

        // Only for TIJ sites
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

        // Only Extractor side properties
        [BsonIgnoreIfNull]
        public string Grade { get; set; }
    }
}
