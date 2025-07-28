using PDS.Common.Utils;
using PDS.Space.Common.Aggregations;
using PDS.Space.Common.Data.E4AModel;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.Space.Common
{
    public static class BaseLotAggregation
    {
        public static void InitCreateControlLimits(BaseControlLimitsPads ctrlLimits, BaseControlLimitsE4A e4aCtrlLimits)
        {
            ctrlLimits.CntrlHigh = e4aCtrlLimits.CntrlHigh;
            ctrlLimits.CtrlHighEnabled = e4aCtrlLimits.CtrlHighEnabled;
            ctrlLimits.CntrlLow = e4aCtrlLimits.CntrlLow;
            ctrlLimits.CtrlLowEnabled = e4aCtrlLimits.CtrlLowEnabled;
            ctrlLimits.CntrlTarget = e4aCtrlLimits.CntrlTarget;
            ctrlLimits.MeanCntrlHigh = e4aCtrlLimits.MeanCntrlHigh;
            ctrlLimits.CtrlMeanHighEnabled = e4aCtrlLimits.CtrlMeanHighEnabled;
            ctrlLimits.MeanCntrlLow = e4aCtrlLimits.MeanCntrlLow;
            ctrlLimits.CtrlMeanLowEnabled = e4aCtrlLimits.CtrlMeanLowEnabled;
            ctrlLimits.MeanCntrlTarget = e4aCtrlLimits.MeanCntrlTarget;
            ctrlLimits.CtrlMeanTargetEnabled = e4aCtrlLimits.CtrlMeanTargetEnabled;
            ctrlLimits.RangeCntrlHigh = e4aCtrlLimits.RangeCntrlHigh;
            ctrlLimits.CtrlRangeHighEnabled = e4aCtrlLimits.CtrlRangeHighEnabled;
            ctrlLimits.RangeCntrlLow = e4aCtrlLimits.RangeCntrlLow;
            ctrlLimits.CtrlRangeLowEnabled = e4aCtrlLimits.CtrlRangeLowEnabled;
            ctrlLimits.RangeCntrlTarget = e4aCtrlLimits.RangeCntrlTarget;
            ctrlLimits.SigmaCntrlHigh = e4aCtrlLimits.SigmaCntrlHigh;
            ctrlLimits.CtrlSigmaHighEnabled = e4aCtrlLimits.CtrlSigmaHighEnabled;
            ctrlLimits.SigmaCntrlLow = e4aCtrlLimits.SigmaCntrlLow;
            ctrlLimits.CtrlSigmaLowEnabled = e4aCtrlLimits.CtrlSigmaLowEnabled;
            ctrlLimits.SigmaCntrlTarget = e4aCtrlLimits.SigmaCntrlTarget;
            ctrlLimits.CtrlSigmaCenterEnabled = e4aCtrlLimits.CtrlSigmaCenterEnabled;
            ctrlLimits.CtrlRangeCenterEnabled = e4aCtrlLimits.CtrlRangeCenterEnabled;
            ctrlLimits.CtrlCenterEnabled = e4aCtrlLimits.CtrlCenterEnabled;
        }

        public static void InitCreateDataFlatMetaData(BaseDataFlatMetaDataPads metaData, BaseDataFlatMetaDataE4A e4aMetaData)
        {
            metaData.ParameterFacility = e4aMetaData.ParameterFacility;
            metaData.ParameterOper = e4aMetaData.ParameterOper;
            metaData.MeasLot = e4aMetaData.Lot;
            metaData.SiteKey = e4aMetaData.SiteKey;
            metaData.SpaceInstanceName = e4aMetaData.SpaceInstanceName;
            metaData.Equipment = e4aMetaData.Equipment;
            metaData.Route = e4aMetaData.Route;
            metaData.BeginTimestamp = e4aMetaData.SampleTimestamp;
            metaData.BeginTimestampUtc = e4aMetaData.SampleTimestampUtc;
            metaData.EndTimestamp = e4aMetaData.SampleTimestamp;
            metaData.EndTimestampUtc = e4aMetaData.SampleTimestampUtc;

        }

        public static void InitCreateStrangeData(BaseStrangeDataFlatMetaDataPads dataFlatStrangeMetaData, BaseDataFlatMetaDataE4A e4aMetaData)
        {
            dataFlatStrangeMetaData.BasicType = e4aMetaData.BasicType;
            dataFlatStrangeMetaData.Product = e4aMetaData.Product;
            dataFlatStrangeMetaData.ProductType = e4aMetaData.ProductType;
        }

        public static void InitCreateSpecLimits(BaseMeasurementSpecLimitsPads measurementSpecLimits, BaseMeasurementSpecLimitsE4A e4aLimits)
        {
            measurementSpecLimits.SpecHighEnabled = e4aLimits.SpecHighEnabled;
            measurementSpecLimits.SpecLowEnabled = e4aLimits.SpecLowEnabled;
            measurementSpecLimits.SpecHigh = e4aLimits.SpecHigh;
            measurementSpecLimits.SpecTarget = e4aLimits.SpecTarget;
            measurementSpecLimits.SpecLow = e4aLimits.SpecLow;
        }

        public static void InitCreateDataList(BaseData1ListPads data1List, BaseDataFlatMetaDataE4A e4aMetaData)
        {
            data1List.ParameterName = e4aMetaData.ParameterName;
            data1List.ParameterUnit = e4aMetaData.ParameterUnit;
            data1List.ChannelId = e4aMetaData.ChannelId;
            data1List.ChannelName = StringUtils.Normalize(e4aMetaData.ChannelName, e4aMetaData.SiteKey);
            data1List.ChannelDescr = StringUtils.Normalize(e4aMetaData.ChannelDescr, e4aMetaData.SiteKey);
            data1List.SourceDataLevel = e4aMetaData.SourceDataLevel;
            data1List.RvStoreFlag = e4aMetaData.RvStoreFlag;
            data1List.CreatedTimestampUtc = e4aMetaData.CreatedTimestampUtc;
            data1List.CreatedTimestamp = e4aMetaData.CreatedTimestamp;
            data1List.UpdatedTimestamp = e4aMetaData.UpdatedTimestamp;
            data1List.UpdatedTimestampUtc = e4aMetaData.UpdatedTimestampUtc;

        }

        public static void InitCreateRawValues(BaseData1ListRawValuesPads rawValues, BaseData1ListRawValuesE4A e4aRawValues, BaseDataFlatMetaDataE4A e4aMetadata, BaseSpaceAggregatesE4A spaceAggregates)
        {
            rawValues.IsFlagged = (e4aRawValues.InternalFlagged.Equals("Y") || e4aRawValues.ExternalFlagged.Equals("Y")) ? "Y" : "N";
            rawValues.Value = e4aMetadata.RvStoreFlag == "N" ? double.NaN : e4aRawValues.Value;
            rawValues.Seqnr = e4aRawValues.Seqnr;
            rawValues.SampleId = e4aRawValues.SampleId; 
            rawValues.ViolationList = e4aRawValues.ViolationList;
            rawValues.ViolationComments = StringUtils.Normalize(e4aRawValues.ViolationComments, e4aMetadata.SiteKey);
            rawValues.NumViolations = e4aRawValues.NumViolations;
            rawValues.PrimaryViolation = e4aRawValues.PrimaryViolation;
            rawValues.PrimaryViolationComments = StringUtils.Normalize(e4aRawValues.PrimaryViolationComments, e4aMetadata.SiteKey);
            rawValues.InternalComment = StringUtils.Normalize(e4aRawValues.InternalComment, e4aMetadata.SiteKey);
            rawValues.SampleTimestamp = e4aMetadata.SampleTimestamp;
            rawValues.SampleTimestampUtc = e4aMetadata.SampleTimestampUtc;
            rawValues.CreatedTimestamp = e4aMetadata.CreatedTimestamp;
            rawValues.CreatedTimestampUtc = e4aMetadata.CreatedTimestampUtc;
            rawValues.UpdatedTimestamp = e4aMetadata.UpdatedTimestamp;
            rawValues.UpdatedTimestampUtc = e4aMetadata.UpdatedTimestampUtc;
            rawValues.SampleMax = spaceAggregates.Max is null ? double.NaN : SpaceAggregationUtils.DoubleParse(spaceAggregates.Max);
            rawValues.SampleMin = spaceAggregates.Min is null ? double.NaN : SpaceAggregationUtils.DoubleParse(spaceAggregates.Min);
            rawValues.SampleMean = spaceAggregates.Mean is null ? double.NaN : SpaceAggregationUtils.DoubleParse(spaceAggregates.Mean);
            rawValues.SampleStdev = spaceAggregates.Sigma is null ? double.NaN : SpaceAggregationUtils.DoubleParse(spaceAggregates.Sigma);
            rawValues.SampleMedian = spaceAggregates.Median is null ? double.NaN : SpaceAggregationUtils.DoubleParse(spaceAggregates.Median);
            rawValues.SampleQ1 = spaceAggregates.Q1 is null ? double.NaN : SpaceAggregationUtils.DoubleParse(spaceAggregates.Q1) ;
            rawValues.SampleQ3 = spaceAggregates.Q3 is null ? double.NaN : SpaceAggregationUtils.DoubleParse(spaceAggregates.Q3);
        }

        public static void InitUpdateStrangeMetaData(BaseStrangeDataFlatMetaDataPads dataFlatStrangeMetaData, BaseDataFlatMetaDataE4A e4aMetaData, BaseStrangeDataFlatMetaDataPads checkMetaData)
        {
            dataFlatStrangeMetaData.BasicType = SpaceAggregationUtils.JoinStrings(checkMetaData.BasicType, e4aMetaData.BasicType);
            dataFlatStrangeMetaData.Product = SpaceAggregationUtils.JoinStrings(checkMetaData.Product, e4aMetaData.Product);
            dataFlatStrangeMetaData.ProductType = SpaceAggregationUtils.JoinStrings(checkMetaData.ProductType, e4aMetaData.ProductType);
        }

        public static void InitUpdateDataFlatMetaData(BaseDataFlatMetaDataPads dataFlatMetaData, BaseDataFlatMetaDataE4A e4aMetaData, BaseDataFlatMetaDataPads checkMetaData)
        {
            dataFlatMetaData.ParameterFacility = e4aMetaData.ParameterFacility;
            dataFlatMetaData.ParameterOper = e4aMetaData.ParameterOper;
            dataFlatMetaData.MeasLot = e4aMetaData.Lot;
            dataFlatMetaData.SiteKey = e4aMetaData.SiteKey;
            dataFlatMetaData.SpaceInstanceName = e4aMetaData.SpaceInstanceName;
            dataFlatMetaData.Equipment = SpaceAggregationUtils.JoinStrings(checkMetaData.Equipment, e4aMetaData.Equipment);
            dataFlatMetaData.Route = SpaceAggregationUtils.JoinStrings(checkMetaData.Route, e4aMetaData.Route);
            dataFlatMetaData.BeginTimestamp = SpaceAggregationUtils.MinDate(checkMetaData.BeginTimestamp, e4aMetaData.SampleTimestamp);
            dataFlatMetaData.BeginTimestampUtc = SpaceAggregationUtils.MinDate(checkMetaData.BeginTimestampUtc, e4aMetaData.SampleTimestampUtc);
            dataFlatMetaData.EndTimestamp = SpaceAggregationUtils.MaxDate(checkMetaData.EndTimestamp, e4aMetaData.SampleTimestamp);
            dataFlatMetaData.EndTimestampUtc = SpaceAggregationUtils.MaxDate(checkMetaData.EndTimestampUtc, e4aMetaData.SampleTimestampUtc);
        }

        public static void InitUpdateDataList(BaseData1ListPads data1List, BaseDataFlatMetaDataE4A e4aMetaData, BaseData1ListPads checkDataList)
        {
            data1List.ParameterName = e4aMetaData.ParameterName;
            data1List.ParameterUnit = e4aMetaData.ParameterUnit;
            data1List.ChannelId = SpaceAggregationUtils.JoinStrings(checkDataList.ChannelId, e4aMetaData.ChannelId);
            data1List.SourceDataLevel = checkDataList.SourceDataLevel;
            data1List.ChannelName = StringUtils.Normalize(checkDataList.ChannelName, e4aMetaData.SiteKey);
            data1List.ChannelDescr = checkDataList.ChannelDescr;
            data1List.RvStoreFlag = SpaceAggregationUtils.JoinStrings(checkDataList.RvStoreFlag, e4aMetaData.RvStoreFlag);
            data1List.CreatedTimestamp = SpaceAggregationUtils.MinDate(checkDataList.CreatedTimestamp, e4aMetaData.CreatedTimestamp);
            data1List.CreatedTimestampUtc = SpaceAggregationUtils.MinDate(checkDataList.CreatedTimestampUtc, e4aMetaData.CreatedTimestampUtc);
            data1List.UpdatedTimestamp = SpaceAggregationUtils.MaxDate(checkDataList.UpdatedTimestamp, e4aMetaData.UpdatedTimestamp);
            data1List.UpdatedTimestampUtc = SpaceAggregationUtils.MaxDate(checkDataList.UpdatedTimestampUtc, e4aMetaData.UpdatedTimestampUtc);

        }

        public static void InitUpdateControlLimits(BaseControlLimitsPads controlLimits, BaseControlLimitsE4A e4aLimits, BaseControlLimitsPads checkLimits)
        {
            var ambFlag = false;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.CntrlHigh, e4aLimits.CntrlHigh))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.CntrlLow, e4aLimits.CntrlLow))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.CntrlTarget, e4aLimits.CntrlTarget))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.RangeCntrlHigh, e4aLimits.RangeCntrlHigh))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.RangeCntrlLow, e4aLimits.RangeCntrlLow))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.RangeCntrlTarget, e4aLimits.RangeCntrlTarget))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.MeanCntrlHigh, e4aLimits.MeanCntrlHigh))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.SigmaCntrlLow, e4aLimits.SigmaCntrlLow))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.MeanCntrlLow, e4aLimits.MeanCntrlLow))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.SigmaCntrlTarget, e4aLimits.SigmaCntrlTarget))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.MeanCntrlTarget, e4aLimits.MeanCntrlTarget))
                ambFlag = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.SigmaCntrlHigh, e4aLimits.SigmaCntrlHigh))
                ambFlag = true;
            controlLimits.CntrlHigh = SpaceAggregationUtils.UpdateLimit(checkLimits.CntrlHigh, e4aLimits.CntrlHigh);
            controlLimits.CtrlHighEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlHighEnabled, e4aLimits.CtrlHighEnabled);
            controlLimits.CntrlLow = SpaceAggregationUtils.UpdateLimit(checkLimits.CntrlLow, e4aLimits.CntrlLow);
            controlLimits.CtrlLowEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlLowEnabled, e4aLimits.CtrlLowEnabled);
            controlLimits.CtrlSigmaCenterEnabled =  SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlSigmaCenterEnabled, e4aLimits.CtrlSigmaCenterEnabled);
            controlLimits.CtrlRangeCenterEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlRangeCenterEnabled, e4aLimits.CtrlRangeCenterEnabled);
            controlLimits.CtrlCenterEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlCenterEnabled, e4aLimits.CtrlCenterEnabled);
            controlLimits.CntrlTarget = SpaceAggregationUtils.UpdateLimit(checkLimits.CntrlTarget, e4aLimits.CntrlTarget);
            controlLimits.RangeCntrlHigh = SpaceAggregationUtils.UpdateLimit(checkLimits.RangeCntrlHigh, e4aLimits.RangeCntrlHigh);
            controlLimits.CtrlRangeHighEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlRangeHighEnabled, e4aLimits.CtrlRangeHighEnabled);
            controlLimits.RangeCntrlLow = SpaceAggregationUtils.UpdateLimit(checkLimits.RangeCntrlLow, e4aLimits.RangeCntrlLow);
            controlLimits.CtrlRangeLowEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlRangeLowEnabled, e4aLimits.CtrlRangeLowEnabled);
            controlLimits.RangeCntrlTarget = SpaceAggregationUtils.UpdateLimit(checkLimits.RangeCntrlTarget, e4aLimits.RangeCntrlTarget);
            controlLimits.MeanCntrlHigh = SpaceAggregationUtils.UpdateLimit(checkLimits.MeanCntrlHigh, e4aLimits.MeanCntrlHigh);
            controlLimits.CtrlMeanHighEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlMeanHighEnabled, e4aLimits.CtrlMeanHighEnabled);
            controlLimits.MeanCntrlLow = SpaceAggregationUtils.UpdateLimit(checkLimits.MeanCntrlLow, e4aLimits.MeanCntrlLow);
            controlLimits.CtrlMeanLowEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlMeanLowEnabled, e4aLimits.CtrlMeanLowEnabled);
            controlLimits.MeanCntrlTarget = SpaceAggregationUtils.UpdateLimit(checkLimits.MeanCntrlTarget, e4aLimits.MeanCntrlTarget);
            controlLimits.CtrlMeanTargetEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlMeanTargetEnabled, e4aLimits.CtrlMeanTargetEnabled);
            controlLimits.SigmaCntrlHigh = SpaceAggregationUtils.UpdateLimit(checkLimits.SigmaCntrlHigh, e4aLimits.SigmaCntrlHigh);
            controlLimits.CtrlSigmaHighEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlSigmaHighEnabled, e4aLimits.CtrlSigmaHighEnabled);
            controlLimits.SigmaCntrlLow = SpaceAggregationUtils.UpdateLimit(checkLimits.SigmaCntrlLow, e4aLimits.SigmaCntrlLow);
            controlLimits.CtrlSigmaLowEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.CtrlSigmaLowEnabled, e4aLimits.CtrlSigmaLowEnabled);
            controlLimits.SigmaCntrlTarget = SpaceAggregationUtils.UpdateLimit(checkLimits.SigmaCntrlTarget, e4aLimits.SigmaCntrlTarget);
            controlLimits.RemovalDue2Ambiguity = ambFlag ? "Y" : null;
        }
        public static void InitUpdateSpecLimits(BaseMeasurementSpecLimitsPads measurementSpecLimits, BaseMeasurementSpecLimitsE4A e4aLimits, BaseMeasurementSpecLimitsPads checkLimits)
        {
            measurementSpecLimits.SpecHigh = SpaceAggregationUtils.UpdateLimit(checkLimits.SpecHigh, e4aLimits.SpecHigh);
            measurementSpecLimits.SpecLow = SpaceAggregationUtils.UpdateLimit(checkLimits.SpecLow, e4aLimits.SpecLow);
            measurementSpecLimits.SpecTarget = SpaceAggregationUtils.UpdateLimit(checkLimits.SpecTarget, e4aLimits.SpecTarget);
            measurementSpecLimits.SpecHighEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.SpecHighEnabled, e4aLimits.SpecHighEnabled);
            measurementSpecLimits.SpecLowEnabled = SpaceAggregationUtils.UpdateLimitEnabled(checkLimits.SpecLowEnabled, e4aLimits.SpecLowEnabled);
            var isAmbigous = false;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.SpecHigh, e4aLimits.SpecHigh))
                isAmbigous = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.SpecLow, e4aLimits.SpecLow))
                isAmbigous = true;
            if (SpaceAggregationUtils.IsLimitAmbigous(checkLimits.SpecTarget, e4aLimits.SpecTarget))
                isAmbigous = true;
            measurementSpecLimits.RemovalDue2Ambiguity = isAmbigous ? "Y" : null;
        }

        public static void InitUpdateRawValues(BaseData1ListRawValuesPads rawValues, BaseData1ListRawValuesPads checkRawValues, BaseDataFlatMetaDataE4A metaData, long e4aRawSampleId)
        {
            rawValues.IsFlagged = checkRawValues.IsFlagged;
            rawValues.Value = checkRawValues.Value;
            rawValues.Seqnr = checkRawValues.Seqnr;
            rawValues.SampleId = checkRawValues.SampleId;
            rawValues.ViolationList = checkRawValues.ViolationList;
            rawValues.ViolationComments = StringUtils.Normalize(checkRawValues.ViolationComments, metaData.SiteKey);
            rawValues.NumViolations = checkRawValues.NumViolations;
            rawValues.PrimaryViolation = checkRawValues.PrimaryViolation;
            rawValues.PrimaryViolationComments = StringUtils.Normalize(checkRawValues.PrimaryViolationComments, metaData.SiteKey);
            rawValues.InternalComment = StringUtils.Normalize(checkRawValues.InternalComment, metaData.SiteKey);
            rawValues.SampleMax = checkRawValues.SampleMax;
            rawValues.SampleMin = checkRawValues.SampleMin;
            rawValues.SampleMean = checkRawValues.SampleMean;
            rawValues.SampleQ3 = checkRawValues.SampleQ3;
            rawValues.SampleQ1 = checkRawValues.SampleQ1;
            rawValues.SampleStdev = checkRawValues.SampleStdev;
            rawValues.SampleMedian = checkRawValues.SampleMedian;
            rawValues.SampleTimestamp = (checkRawValues.SampleTimestamp != metaData.SampleTimestamp && checkRawValues.SampleId != e4aRawSampleId) ? checkRawValues.SampleTimestamp : metaData.SampleTimestamp;
            rawValues.SampleTimestampUtc = (checkRawValues.SampleTimestampUtc != metaData.SampleTimestampUtc && checkRawValues.SampleId != e4aRawSampleId) ? checkRawValues.SampleTimestampUtc : metaData.SampleTimestampUtc;
            rawValues.CreatedTimestamp = (checkRawValues.CreatedTimestamp != metaData.CreatedTimestamp && checkRawValues.SampleId != e4aRawSampleId) ? checkRawValues.CreatedTimestamp : metaData.CreatedTimestamp;
            rawValues.CreatedTimestampUtc = (checkRawValues.CreatedTimestampUtc != metaData.CreatedTimestampUtc && checkRawValues.SampleId != e4aRawSampleId) ? checkRawValues.CreatedTimestampUtc : metaData.CreatedTimestampUtc;
            rawValues.UpdatedTimestamp = (checkRawValues.UpdatedTimestamp != metaData.UpdatedTimestamp && checkRawValues.SampleId != e4aRawSampleId) ? checkRawValues.UpdatedTimestamp : metaData.UpdatedTimestamp;
            rawValues.UpdatedTimestampUtc = (checkRawValues.UpdatedTimestampUtc != metaData.UpdatedTimestampUtc && checkRawValues.SampleId != e4aRawSampleId) ? checkRawValues.UpdatedTimestampUtc : metaData.UpdatedTimestampUtc;

        }
    }
}
