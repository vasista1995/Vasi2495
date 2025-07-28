using System;
using System.Net.NetworkInformation;
using PDS.Space.Common.Data.E4AModel;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.Space.Common
{
    /// <summary>
    /// Base E4A converter for all the Base fields additions
    /// </summary>
    public static class BaseSpaceE4AConverter
    {
        public static void InitControlLimits(BaseControlLimitsE4A e4a, BaseSpaceEntry entry)
        {
            e4a.MeanCntrlHigh = entry.MeanCntrlHigh;
            e4a.CtrlMeanHighEnabled = entry.CtrlMeanHighEnabled;
            e4a.MeanCntrlLow = entry.MeanCntrlLow;
            e4a.CtrlMeanLowEnabled = entry.CtrlMeanLowEnabled;
            e4a.MeanCntrlTarget = entry.MeanCntrlTarget;
            e4a.CtrlMeanTargetEnabled = entry.CtrlMeanTargetEnabled;
            e4a.CntrlHigh = entry.CntrlHigh;
            e4a.CtrlHighEnabled = entry.CtrlHighEnabled;
            e4a.CntrlLow = entry.CntrlLow;
            e4a.CtrlLowEnabled = entry.CtrlLowEnabled;
            e4a.CtrlSigmaCenterEnabled = entry.CtrlSigmaCenterEnabled;
            e4a.CtrlRangeCenterEnabled = entry.CtrlRangeCenterEnabled;
            e4a.CtrlCenterEnabled = entry.CtrlCenterEnabled;
            e4a.CntrlTarget = entry.CntrlTarget;
            e4a.RangeCntrlHigh = entry.RangeCntrlHigh;
            e4a.CtrlRangeHighEnabled = entry.CtrlRangeHighEnabled;
            e4a.RangeCntrlLow = entry.RangeCntrlLow;
            e4a.CtrlRangeLowEnabled = entry.CtrlRangeLowEnabled;
            e4a.RangeCntrlTarget = entry.RangeCntrlTarget;
            e4a.SigmaCntrlHigh = entry.SigmaCntrlHigh;
            e4a.CtrlSigmaHighEnabled = entry.CtrlSigmaHighEnabled;
            e4a.SigmaCntrlLow = entry.SigmaCntrlLow;
            e4a.CtrlSigmaLowEnabled = entry.CtrlSigmaLowEnabled;
            e4a.SigmaCntrlTarget = entry.SigmaCntrlTarget;
            e4a.ExtMaMV = entry.ExtMaMV;
            e4a.ExtEwmaMV = entry.ExtEwmaMV;
            e4a.ExtMsMV = entry.ExtMsMV;
            e4a.EwmaS = entry.EwmaS;
            e4a.EwmaR = entry.EwmaR;
            e4a.ExtEwmaMVLCL = entry.ExtEwmaMVLCL;
            e4a.ExtEwmaMVCenter = entry.ExtEwmaMVCenter;
            e4a.ExtEwmaMVUCL = entry.ExtEwmaMVUCL;
            e4a.ExtMaMVLCL = entry.ExtMaMVLCL;
            e4a.ExtMaMVCenter = entry.ExtMaMVCenter;
            e4a.ExtMaMVUCL = entry.ExtMaMVUCL;
            e4a.EwmaSLCL = entry.EwmaSLCL;
            e4a.EwmaSCenter = entry.EwmaSCenter;
            e4a.EwmaSUCL = entry.EwmaSUCL;
            e4a.EwmaRLCL = entry.EwmaRLCL;
            e4a.EwmaRCenter = entry.EwmaRCenter;
            e4a.EwmaRUCL = entry.EwmaRUCL;
            e4a.ExtMSMVLCL = entry.ExtMSMVLCL;
            e4a.ExtMSMVCenter = entry.ExtMSMVCenter;
            e4a.ExtMSMVUCL = entry.ExtMSMVUCL;
            e4a.MVLal = entry.MVLal;
            e4a.MVUal = entry.MVUal;
            e4a.RawLal = entry.RawLal;
            e4a.RawUal = entry.RawUal;
            e4a.SigmaLal = entry.SigmaLal;
            e4a.SigmaUal = entry.SigmaUal;
            e4a.RangeLal = entry.RangeLal;
            e4a.RangeUal = entry.RangeUal;
    }

        public static void InitData1ListRawValues(BaseData1ListRawValuesE4A e4a, BaseSpaceRawValuesEntry entry)
        {
            e4a.ExternalFlagged = entry.ExternalFlagged;
            e4a.InternalComment = entry.InternalComment;
            e4a.InternalFlagged = entry.InternalFlagged;
            e4a.NumViolations = entry.NumViolations;
            e4a.PrimaryViolation = entry.PrimaryViolation;
            e4a.PrimaryViolationComments = entry.PrimaryViolationComments;
            e4a.SampleId = entry.SampleId;
            e4a.Seqnr = entry.Seqnr;
            e4a.Value = entry.Value;
            e4a.ViolationComments = entry.ViolationComments;
            e4a.ViolationList = entry.ViolationList;
        }

        public static void InitFlatMetaData(BaseDataFlatMetaDataE4A e4a, BaseSpaceEntry entry, string sourcedataLevel)
        {
            e4a.Lot = entry.Lot;
            e4a.ParameterFacility = entry.Facility;
            e4a.ParameterOper = entry.Operation;
            e4a.ParameterName = entry.ParameterName;
            e4a.ParameterUnit = entry.ParameterUnit;
            e4a.SiteKey = entry.Sitekey;
            e4a.BasicType = entry.BasicType;
            e4a.SpaceInstanceName = entry.SpaceInstanceName;
            e4a.ChannelId = entry.ChannelId;
            e4a.ChannelDescr = entry.ChannelDescr;
            e4a.ChannelName = entry.ChannelName;
            e4a.CreatedTimestampUtc = entry.CreatedDateUtc;
            e4a.CreatedTimestamp = entry.CreatedDate;
            e4a.UpdatedTimestamp = entry.UpdatedDate;
            e4a.UpdatedTimestampUtc = entry.UpdatedDateUtc;
            e4a.ExportedTimestamp = DateTime.UtcNow;
            e4a.ExportedTimestampUtc = e4a.ExportedTimestamp;
            e4a.SampleTimestampUtc = entry.SampleDateUtc;
            e4a.SampleTimestamp = entry.SampleDate;
            e4a.SourceDataLevel = sourcedataLevel;
            e4a.RvStoreFlag = entry.RvStoreFlag;
            e4a.CalcCreated = entry.CalcCreated;
            e4a.ILdsID = entry.ILdsID;
            e4a.LdsID = entry.LdsID;
            e4a.IsCalcParam = entry.IsCalcParam;
            e4a.SpcComExtern = entry.SpcComExtern;
            e4a.SpcComIntern = entry.SpcComIntern;
            e4a.AcceptFlag = entry.AcceptFlag;
            e4a.UpdatedBy = entry.UpdatedBy;
            e4a.ClOrigin = entry.ClOrigin;
            e4a.OutOfOrder = entry.OutOfOrder;
            e4a.Calc2ID = entry.Calc2ID;
            e4a.ViolCount = entry.ViolCount;
            e4a.Owner = entry.Owner;
            e4a.ChState = entry.ChState;
            e4a.CalculationStrategy = entry.CalculationStrategy;
            e4a.ChannelType = entry.ChannelType;
        }

        public static void InitSpaceAggregates(BaseSpaceAggregatesE4A e4A, BaseSpaceEntry entry)
        {
            e4A.Mean = entry.Mean;
            e4A.Median = entry.Median;
            e4A.Sigma = entry.Sigma;
            e4A.Max = entry.Max;
            e4A.Min = entry.Min;
            e4A.Q1 = entry.Q1;
            e4A.Q3 = entry.Q3;
        }

        public static void InitMeasurementSpecLimits(BaseMeasurementSpecLimitsE4A e4A, BaseSpaceEntry entry)
        {
            e4A.SpecHighEnabled = entry.SpecHighEnabled;
            e4A.SpecLowEnabled = entry.SpecLowEnabled;
            e4A.SpecHigh = entry.SpecHigh;
            e4A.SpecLow = entry.SpecLow;
            e4A.SpecTarget = entry.SpecTarget;
            e4A.SpecTargetOrigin = entry.SpecTargetOrigin;
            e4A.ExtSpecLower = entry.ExtSpecLower;
            e4A.ExtSpecUpper = entry.ExtSpecUpper;
            e4A.ExtSpecLimEnable = entry.ExtSpecLimEnable;
            e4A.LimitEnable = entry.LimitEnable;
        }
    }
}
