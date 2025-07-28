using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.SpaceBE.TIJ.Source.Module.Data.SpaceModel
{

    /// <summary>
    /// It takes all the values from the sql statements and converts them into the below properties.
    /// </summary>
    public class SpaceEntry : BaseSpaceEntry
    {
        [BsonIgnoreIfNull]
        public string PackageFamily { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }

        // Consistent in all BE sites
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string FourDReport { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string SpecialCharacteristics { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string PackageCF { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }

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

        // Only Extractor side properties
        [BsonIgnoreIfNull]
        public string Grade { get; set; }

        public List<SpaceRawValuesEntry> SpaceRawValues { get; set; }

    }
}
