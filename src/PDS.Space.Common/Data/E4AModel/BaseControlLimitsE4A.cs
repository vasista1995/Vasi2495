using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PDS.Space.Common.Data.E4AModel
{
    public class BaseControlLimitsE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to controllimits subsection of DataFlatlimits section in e4a document.
        /// </summary>
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_mean_low")]
        public double? MeanCntrlLow { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlMeanLowEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_mean_target")]
        public double? MeanCntrlTarget { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlMeanTargetEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_mean_high")]
        public double? MeanCntrlHigh { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlMeanHighEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_low")]
        public double? CntrlLow { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlLowEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_sigma_center")]
        public double? CtrlSigmaCenter { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlSigmaCenterEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_range_center")]
        public double? CtrlRangeCenter { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlRangeCenterEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_center")]
        public double? CtrlCenter { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlCenterEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_target")]
        public double? CntrlTarget { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_high")]
        public double? CntrlHigh { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlHighEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_sigma_low")]
        public double? SigmaCntrlLow { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlSigmaLowEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_sigma_target")]
        public double? SigmaCntrlTarget { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_sigma_high")]
        public double? SigmaCntrlHigh { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlSigmaHighEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_range_low")]
        public double? RangeCntrlLow { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlRangeLowEnabled { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_range_target")]
        public double? RangeCntrlTarget { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty("ctrl_range_high")]
        public double? RangeCntrlHigh { get; set; }
        [BsonIgnoreIfNull]
        public string CtrlRangeHighEnabled { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtMaMV { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtEwmaMV { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtMsMV { get; set; }
        [BsonIgnoreIfNull]
        public double? EwmaS { get; set; }
        [BsonIgnoreIfNull]
        public double? EwmaR { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtEwmaMVLCL { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtEwmaMVCenter { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtEwmaMVUCL { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtMaMVLCL { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtMaMVCenter { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtMaMVUCL { get; set; }
        [BsonIgnoreIfNull]
        public double? EwmaSLCL { get; set; }
        [BsonIgnoreIfNull]
        public double? EwmaSCenter { get; set; }
        [BsonIgnoreIfNull]
        public double? EwmaSUCL { get; set; }
        [BsonIgnoreIfNull]
        public double? EwmaRLCL { get; set; }
        [BsonIgnoreIfNull]
        public double? EwmaRCenter { get; set; }
        [BsonIgnoreIfNull]
        public double? EwmaRUCL { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtMSMVLCL { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtMSMVCenter { get; set; }
        [BsonIgnoreIfNull]
        public double? ExtMSMVUCL { get; set; }
        [BsonIgnoreIfNull]
        public double? MVLal { get; set; }
        [BsonIgnoreIfNull]
        public double? MVUal { get; set; }
        [BsonIgnoreIfNull]
        public double? RawLal { get; set; }
        [BsonIgnoreIfNull]
        public double? RawUal { get; set; }
        [BsonIgnoreIfNull]
        public double? SigmaLal { get; set; }
        [BsonIgnoreIfNull]
        public double? SigmaUal { get; set; }
        [BsonIgnoreIfNull]
        public double? RangeLal { get; set; }
        [BsonIgnoreIfNull]
        public double? RangeUal { get; set; }
    }
}
