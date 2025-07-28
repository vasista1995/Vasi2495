using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.SpaceBE.CEG.Source.Module.Data.SpaceModel
{
    /// <summary>
    /// It takes all the values from the sql statements and converts them into the below properties.
    /// </summary>
    public class SpaceEntry : BaseSpaceEntry
    {
        [BsonIgnoreIfNull]
        public string Plant { get; set; }
        [BsonIgnoreIfNull]
        public string IdSystemName { get; set; }
        [BsonIgnoreIfNull]
        public string Line { get; set; }
        [BsonIgnoreIfNull]
        public string ProductionOrder { get; set; }
        [BsonIgnoreIfNull]
        public string Batch { get; set; }
        [BsonIgnoreIfNull]
        public string MaterialNumber { get; set; }
        [BsonIgnoreIfNull]
        public string ProductGroup { get; set; }
        [BsonIgnoreIfNull]
        public string MeasurementRecipe { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string OrderType { get; set; }
        [BsonIgnoreIfNull]
        public string QrkGroup { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessGroup { get; set; }
        [BsonIgnoreIfNull]
        public string SpcClass { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterType { get; set; }
        [BsonIgnoreIfNull]
        public string LongParameterName { get; set; }
        [BsonIgnoreIfNull]
        public string MaterialNumberText { get; set; }
        [BsonIgnoreIfNull]
        public string MeasurementEquipmentName { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessEquipmentName { get; set; }
        [BsonIgnoreIfNull]
        public string ProductName { get; set; }
        [BsonIgnoreIfNull]
        public string Length { get; set; }
        [BsonIgnoreIfNull]
        public string Operator { get; set; }
        [BsonIgnoreIfNull]
        public string UnitLength { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessOwner { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string Package { get; set; }
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string GroupId { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string PosName { get; set; }
        [BsonIgnoreIfNull]
        public string OCAPEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string Published { get; set; }
        [BsonIgnoreIfNull]
        public string FourDReport { get; set; }
        [BsonIgnoreIfNull]
        public string SpecialCharacteristics { get; set; }
        [BsonIgnoreIfNull]
        public string VariableExtr1 { get; set; }
        [BsonIgnoreIfNull]
        public string VariableExtr2 { get; set; }
        [BsonIgnoreIfNull]
        public string VariableExtr3 { get; set; }
        [BsonIgnoreIfNull]
        public string VariableExtr4 { get; set; }
        [BsonIgnoreIfNull]
        public string VariableExtr5 { get; set; }
        [BsonIgnoreIfNull]
        public string VariableExtr6 { get; set; }
        [BsonIgnoreIfNull]
        public string Label4Extr1 { get; set; }
        [BsonIgnoreIfNull]
        public string Label4Extr2 { get; set; }
        [BsonIgnoreIfNull]
        public string Label4Extr3 { get; set; }
        [BsonIgnoreIfNull]
        public string Label4Extr4 { get; set; }
        [BsonIgnoreIfNull]
        public string Label4Extr5 { get; set; }
        [BsonIgnoreIfNull]
        public string Label4Extr6 { get; set; }

        public List<SpaceRawValuesEntry> SpaceRawValues { get; set; }
    }
}
