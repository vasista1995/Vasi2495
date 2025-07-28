using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PDS.Space.Common.Data.PADSModel
{
    public class BaseControlLimitsPads
    {
        [JsonProperty(SpacePadsProperties.MeanCntrlLow)]
        [BsonElement(SpacePadsProperties.MeanCntrlLow)]
        [BsonIgnoreIfNull]
        public double? MeanCntrlLow { get; set; }

        [BsonIgnoreIfNull]
        public string CtrlMeanLowEnabled { get; set; }

        [JsonProperty(SpacePadsProperties.MeanCntrlTarget)]
        [BsonElement(SpacePadsProperties.MeanCntrlTarget)]
        [BsonIgnoreIfNull]
        public double? MeanCntrlTarget { get; set; }

        [BsonIgnoreIfNull]
        public string CtrlMeanTargetEnabled { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.MeanCntrlHigh)]
        [BsonElement(SpacePadsProperties.MeanCntrlHigh)]
        public double? MeanCntrlHigh { get; set; }

        [BsonIgnoreIfNull]
        public string CtrlMeanHighEnabled { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CntrlLow)]
        [BsonElement(SpacePadsProperties.CntrlLow)]
        public double? CntrlLow { get; set; }

        [BsonIgnoreIfNull]
        public string CtrlLowEnabled { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CntrlTarget)]
        [BsonElement(SpacePadsProperties.CntrlTarget)]
        public double? CntrlTarget { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CntrlHigh)]
        [BsonElement(SpacePadsProperties.CntrlHigh)]
        public double? CntrlHigh { get; set; }

        [BsonIgnoreIfNull]
        public string CtrlHighEnabled { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.SigmaCntrlLow)]
        [BsonElement(SpacePadsProperties.SigmaCntrlLow)]
        public double? SigmaCntrlLow { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CtrlSigmaLowEnabled)]
        [BsonElement(SpacePadsProperties.CtrlSigmaLowEnabled)]
        public string CtrlSigmaLowEnabled { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CtrlSigmaCenterEnabled)]
        [BsonElement(SpacePadsProperties.CtrlSigmaCenterEnabled)]
        public string CtrlSigmaCenterEnabled { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CtrlRangeCenterEnabled)]
        [BsonElement(SpacePadsProperties.CtrlRangeCenterEnabled)]
        public string CtrlRangeCenterEnabled { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CtrlCenterEnabled)]
        [BsonElement(SpacePadsProperties.CtrlCenterEnabled)]
        public string CtrlCenterEnabled { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.SigmaCntrlTarget)]
        [BsonElement(SpacePadsProperties.SigmaCntrlTarget)]
        public double? SigmaCntrlTarget { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.SigmaCntrlHigh)]
        [BsonElement(SpacePadsProperties.SigmaCntrlHigh)]
        public double? SigmaCntrlHigh { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.CtrlSigmaHighEnabled)]
        [BsonElement(SpacePadsProperties.CtrlSigmaHighEnabled)]
        public string CtrlSigmaHighEnabled { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.RangeCntrlLow)]
        [BsonElement(SpacePadsProperties.RangeCntrlLow)]
        public double? RangeCntrlLow { get; set; }

        [BsonIgnoreIfNull]
        public string CtrlRangeLowEnabled { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.RangeCntrlTarget)]
        [BsonElement(SpacePadsProperties.RangeCntrlTarget)]
        public double? RangeCntrlTarget { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.RangeCntrlHigh)]
        [BsonElement(SpacePadsProperties.RangeCntrlHigh)]
        public double? RangeCntrlHigh { get; set; }

        [BsonIgnoreIfNull]
        public string CtrlRangeHighEnabled { get; set; }

        [BsonIgnoreIfNull]
        public string RemovalDue2Ambiguity { get; set; }
    }
}
