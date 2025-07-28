using System;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Common.Source;

namespace PDS.Space.Common.Data.SpaceModel
{
    /// <summary>
    /// It takes all the values from the sql statements and converts them into the below properties.
    /// </summary>
    public class BaseSpaceEntry : ISourceRecord
    {
        /// <inheritdoc/>
        public string UniqueSourceKey => PKey;

        public string PKey { get; set; }
        public string ParameterName { get; set; }
        public string ParameterUnit { get; set; }
        public DateTime SampleDate { get; set; }
        public DateTime SampleDateUtc { get; set; }
        public int SampleSize { get; set; }
        public string Sigma { get; set; }
        public string Mean { get; set; }
        public string Median { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Q1 { get; set; }
        public string Q3 { get; set; }
        public string LdsID { get; set; }
        public long IsCalcParam { get; set; }
        public double? MeanCntrlLow { get; set; }
        public double? MeanCntrlTarget { get; set; }
        public double? MeanCntrlHigh { get; set; }
        public double? CntrlLow { get; set; }
        public double? CntrlTarget { get; set; }
        public double? CntrlHigh { get; set; }
        public double? RangeCntrlLow { get; set; }
        public double? RangeCntrlTarget { get; set; }
        public double? RangeCntrlHigh { get; set; }
        public double? SigmaCntrlLow { get; set; }
        public double? SigmaCntrlTarget { get; set; }
        public double? SigmaCntrlHigh { get; set; }
        public double? SpecLow { get; set; }
        public double? SpecTarget { get; set; }
        public double? SpecHigh { get; set; }
        public string CtrlMeanHighEnabled { get; set; }
        public string CtrlMeanLowEnabled { get; set; }
        public string CtrlMeanTargetEnabled { get; set; }
        public string CtrlLowEnabled { get; set; }
        public string CtrlHighEnabled { get; set; }
        public string CtrlRangeLowEnabled { get; set; }
        public string CtrlRangeHighEnabled { get; set; }
        public string CtrlSigmaLowEnabled { get; set; }
        public string CtrlSigmaCenterEnabled { get; set; }
        public string CtrlRangeCenterEnabled { get; set; }
        public string CtrlCenterEnabled { get; set; }
        public string CtrlSigmaHighEnabled { get; set; }
        public string SpecLowEnabled { get; set; }
        public string SpecHighEnabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime UpdatedDateUtc { get; set; }
        public string RvStoreFlag { get; set; }
        public string ProductType { get; set; }
        public string Product { get; set; }
        public string BasicType { get; set; }
        public string Route { get; set; }
        public string Operation { get; set; }
        public string Facility { get; set; }
        public string MeasurementEquipment { get; set; }
        public string Lot { get; set; }
        public string ChannelId { get; set; }
        public string CkcId { get; set; }
        public string ChannelName { get; set; }
        public string ChannelDescr { get; set; }
        public string SpaceInstanceName { get; set; }
        public string Sitekey { get; set; }
        public string CalcCreated { get; set; }
        public long ILdsID { get; set; }
        public string SpcComExtern { get; set; }
        public string SpcComIntern { get; set; }
        public string AcceptFlag { get; set; }
        public string UpdatedBy { get; set; }
        public string SpecTargetOrigin { get; set; }
        public int? ViolCount { get; set; }
        public double? ExtMaMV { get; set; }
        public double? ExtEwmaMV { get; set; }
        public double? ExtMsMV { get; set; }
        public string OutOfOrder { get; set; }
        public double? EwmaS { get; set; }
        public double? EwmaR { get; set; }
        public long Calc2ID { get; set; }
        public string LimitEnable { get; set; }
        public double? ExtEwmaMVLCL { get; set; }
        public double? ExtEwmaMVCenter { get; set; }
        public double? ExtEwmaMVUCL { get; set; }
        public double? ExtMaMVLCL { get; set; }
        public double? ExtMaMVCenter { get; set; }
        public double? ExtMaMVUCL { get; set; }
        public double? EwmaSLCL { get; set; }
        public double? EwmaSCenter { get; set; }
        public double? EwmaSUCL { get; set; }
        public double? EwmaRLCL { get; set; }
        public double? EwmaRCenter { get; set; }
        public double? EwmaRUCL { get; set; }
        public double? ExtMSMVLCL { get; set; }
        public double? ExtMSMVCenter { get; set; }
        public double? ExtMSMVUCL { get; set; }
        public long ClOrigin { get; set; }
        public double? MVLal { get; set; }
        public double? MVUal { get; set; }
        public double? RawLal { get; set; }
        public double? RawUal { get; set; }
        public double? SigmaLal { get; set; }
        public double? SigmaUal { get; set; }
        public double? RangeLal { get; set; }
        public double? RangeUal { get; set; }
        public double? ExtSpecLower { get; set; }
        public double? ExtSpecUpper { get; set; }
        public string ExtSpecLimEnable { get; set; }
        public string Owner { get; set; }
        public string CalculationStrategy { get; set; }
        public string ChannelType { get; set; }
        public string ChState { get; set; }


    }
}
