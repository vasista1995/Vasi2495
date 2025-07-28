namespace PDS.Space.Common.Data.PADSModel
{
    /// <summary>
    /// Space specific PADS property names that do not match to C# property naming.
    /// </summary>
    public static class SpacePadsProperties
    {
        public const string SystemLogE4A = "SystemLogE4A";
        public const string Data1ListRawValues = "Data_1list_RawValues";
        public const string DataFlatStrangeMetadata = "Data_flat_StrangeMetadata";
        public const string Data1ListParameters = "Data_1list_Parameters";
        public const string Id = "_id";
        public const string SpecHigh = "spec_high";
        public const string SpecLow = "spec_low";
        public const string Target = "target";
        public const string ExecCount = "exec_count";
        public const string BaseCount = "base_count";
        public const string FlaggedCount = "flagged_count";
        public const string Mean = "mean";
        public const string GofMean = "gof_mean";
        public const string Median = "median";
        public const string Sigma = "stdev";
        public const string Range = "range";
        public const string Min = "min";
        public const string Max = "max";
        public const string GofMin = "gof_min";
        public const string GofMax = "gof_max";
        public const string Q2 = "q02";
        public const string Q5 = "q05";
        public const string Q25 = "q25";
        public const string Q75 = "q75";
        public const string Q95 = "q95";
        public const string Q98 = "q98";
        public const string MeanCntrlLow = "ctrl_mean_low";
        public const string MeanCntrlTarget = "ctrl_mean_target";
        public const string MeanCntrlHigh = "ctrl_mean_high";
        public const string CntrlLow = "ctrl_low";
        public const string CntrlTarget = "ctrl_target";
        public const string CntrlHigh = "ctrl_high";
        public const string SigmaCntrlLow = "ctrl_stdev_low";
        public const string CtrlSigmaLowEnabled = "CtrlStdevLowEnabled";
        public const string CtrlSigmaCenterEnabled = "CtrlSigmaCenterEnabled";
        public const string CtrlRangeCenterEnabled = "CtrlRangeCenterEnabled";
        public const string CtrlCenterEnabled = "CtrlCenterEnabled";
        public const string SigmaCntrlTarget = "ctrl_stdev_target";
        public const string SigmaCntrlHigh = "ctrl_stdev_high";
        public const string CtrlSigmaHighEnabled = "CtrlStdevHighEnabled";
        public const string RangeCntrlLow = "ctrl_range_low";
        public const string RangeCntrlTarget = "ctrl_range_target";
        public const string RangeCntrlHigh = "ctrl_range_high";
        public const string CreatedTimestampUtc = "created_timestamp_utc";
        public const string CreatedTimestamp = "created_timestamp";
        public const string UpdatedTimestampUtc = "updated_timestamp_utc";
        public const string UpdatedTimestamp = "updated_timestamp";
        public const string IsFlagged = "is_flagged";
        public const string ChannelName = "channel_name";
        public const string ChannelDescr = "channel_description";
        public const string DataFlatLimits = "Data_flat_Limits";
        public const string MeasurementAggregates = "Data_flat_MeasurementAggregates";
        public const string ProcessBatch = "process_batch";
        public const string ProcessEquipment = "process_equipment";
        public const string ProcessTool = "process_tool";
        public const string ItemIdMotherlotWafer = "ItemID:MotherlotWafer";
        public const string ItemIDFEProberChipID = "ItemID:FEProberChipID";
        public const string SPSNumber = "SPS_Number";
        public const string CKCId = "CKC_ID";
    }
}
