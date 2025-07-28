using PDS.Space.Common.Aggregations;
using PDS.Space.Common.Data.PADSModel;


namespace PDS.Space.Common
{
    public static class BaseWaferAggregation
    {
        /// <summary>
        /// Create Base Fields of control limits in a new wafer aggregation document
        /// </summary>
        /// <param name="ctrlLimits"></param>
        /// <param name="lotCtrlLimits"></param>
        public static void InitCreateControlLimits(BaseControlLimitsPads ctrlLimits, BaseControlLimitsPads lotCtrlLimits)
        {
            ctrlLimits.CntrlHigh = lotCtrlLimits.CntrlHigh;
            ctrlLimits.CtrlHighEnabled = lotCtrlLimits.CtrlHighEnabled;
            ctrlLimits.CntrlLow = lotCtrlLimits.CntrlLow;
            ctrlLimits.CtrlLowEnabled = lotCtrlLimits.CtrlLowEnabled;
            ctrlLimits.CtrlSigmaCenterEnabled = lotCtrlLimits.CtrlSigmaCenterEnabled;
            ctrlLimits.CtrlRangeCenterEnabled = lotCtrlLimits.CtrlRangeCenterEnabled;
            ctrlLimits.CtrlCenterEnabled = lotCtrlLimits.CtrlCenterEnabled;
            ctrlLimits.CntrlTarget = lotCtrlLimits.CntrlTarget;
            ctrlLimits.MeanCntrlHigh = lotCtrlLimits.MeanCntrlHigh;
            ctrlLimits.CtrlMeanHighEnabled = lotCtrlLimits.CtrlMeanHighEnabled;
            ctrlLimits.MeanCntrlLow = lotCtrlLimits.MeanCntrlLow;
            ctrlLimits.CtrlMeanLowEnabled = lotCtrlLimits.CtrlMeanLowEnabled;
            ctrlLimits.MeanCntrlTarget = lotCtrlLimits.MeanCntrlTarget;
            ctrlLimits.CtrlMeanTargetEnabled = lotCtrlLimits.CtrlMeanTargetEnabled;
            ctrlLimits.RangeCntrlHigh = lotCtrlLimits.RangeCntrlHigh;
            ctrlLimits.CtrlRangeHighEnabled = lotCtrlLimits.CtrlRangeHighEnabled;
            ctrlLimits.RangeCntrlLow = lotCtrlLimits.RangeCntrlLow;
            ctrlLimits.CtrlRangeLowEnabled = lotCtrlLimits.CtrlRangeLowEnabled;
            ctrlLimits.RangeCntrlTarget = lotCtrlLimits.RangeCntrlTarget;
            ctrlLimits.SigmaCntrlHigh = lotCtrlLimits.SigmaCntrlHigh;
            ctrlLimits.CtrlSigmaHighEnabled = lotCtrlLimits.CtrlSigmaHighEnabled;
            ctrlLimits.SigmaCntrlLow = lotCtrlLimits.SigmaCntrlLow;
            ctrlLimits.CtrlSigmaLowEnabled = lotCtrlLimits.CtrlSigmaLowEnabled;
            ctrlLimits.SigmaCntrlTarget = lotCtrlLimits.SigmaCntrlTarget;
            ctrlLimits.RemovalDue2Ambiguity = lotCtrlLimits.RemovalDue2Ambiguity;
        }
        /// <summary>
        /// Create dataflatmetadata base fields of the dataflatmetadata subdocument in a new wafer document
        /// </summary>
        /// <param name="metaData"></param>
        /// <param name="lotMetaData"></param>
        public static void InitCreateDataFlatMetaData(BaseDataFlatMetaDataPads metaData, BaseDataFlatMetaDataPads lotMetaData)
        {
            metaData.ParameterFacility = lotMetaData.ParameterFacility;
            metaData.ParameterOper = lotMetaData.ParameterOper;
            metaData.MeasLot = lotMetaData.MeasLot;
            metaData.SiteKey = lotMetaData.SiteKey;
            metaData.SpaceInstanceName = lotMetaData.SpaceInstanceName;
            metaData.Equipment = lotMetaData.Equipment;
            metaData.Route = lotMetaData.Route;
        }
        /// <summary>
        /// Create Data1list base fields for the wafer aggregation document
        /// </summary>
        /// <param name="data1List"></param>
        /// <param name="lotDataList"></param>
        public static void InitCreateData1List(BaseData1ListPads data1List, BaseData1ListPads lotDataList)
        {
            data1List.ParameterName = lotDataList.ParameterName;
            data1List.ParameterUnit = lotDataList.ParameterUnit;
            data1List.ChannelId = lotDataList.ChannelId;
            data1List.ChannelName = lotDataList.ChannelName;
            data1List.ChannelDescr = lotDataList.ChannelDescr;
            data1List.SourceDataLevel = lotDataList.SourceDataLevel;
            data1List.RvStoreFlag = lotDataList.RvStoreFlag;
        }
        /// <summary>
        /// Create Strange data base fields for a new wafer document
        /// </summary>
        /// <param name="dataFlatStrangeMetaData"></param>
        /// <param name="lotMetaData"></param>
        public static void InitCreateStrangeData(BaseStrangeDataFlatMetaDataPads dataFlatStrangeMetaData, BaseStrangeDataFlatMetaDataPads lotMetaData)
        {
            dataFlatStrangeMetaData.BasicType = lotMetaData.BasicType;
            dataFlatStrangeMetaData.Product = lotMetaData.Product;
            dataFlatStrangeMetaData.ProductType = lotMetaData.ProductType;
        }
        /// <summary>
        /// create base fields in the spec limits for wafer document
        /// </summary>
        /// <param name="measurementSpecLimits"></param>
        /// <param name="lotSpecLimits"></param>
        public static void InitCreateSpecLimits(BaseMeasurementSpecLimitsPads measurementSpecLimits, BaseMeasurementSpecLimitsPads lotSpecLimits)
        {
            measurementSpecLimits.SpecHighEnabled = lotSpecLimits.SpecHighEnabled;
            measurementSpecLimits.SpecLowEnabled = lotSpecLimits.SpecLowEnabled;
            measurementSpecLimits.SpecHigh = lotSpecLimits.SpecHigh;
            measurementSpecLimits.SpecTarget = lotSpecLimits.SpecTarget;
            measurementSpecLimits.SpecLow = lotSpecLimits.SpecLow;
            measurementSpecLimits.RemovalDue2Ambiguity = lotSpecLimits.RemovalDue2Ambiguity;
        }
        /// <summary>
        /// Create base strange metadata for wafer aggreates document
        /// </summary>
        /// <param name="dataFlatStrangeMetaData"></param>
        /// <param name="lotMetaData"></param>
        /// <param name="checMetaData"></param>
        public static void InitUpdateStrangeMetaData(BaseStrangeDataFlatMetaDataPads dataFlatStrangeMetaData, BaseStrangeDataFlatMetaDataPads lotMetaData, BaseStrangeDataFlatMetaDataPads checMetaData)
        {
            dataFlatStrangeMetaData.BasicType = SpaceAggregationUtils.JoinStrings(checMetaData.BasicType, lotMetaData.BasicType);
            dataFlatStrangeMetaData.Product = SpaceAggregationUtils.JoinStrings(checMetaData.Product, lotMetaData.Product);
            dataFlatStrangeMetaData.ProductType = SpaceAggregationUtils.JoinStrings(checMetaData.ProductType, lotMetaData.ProductType);
        }
        /// <summary>
        /// Update the base fields of data flat meta data for the already existing wafer document
        /// </summary>
        /// <param name="dataFlatMetaData"></param>
        /// <param name="lotMetaData"></param>
        /// <param name="checMetaData"></param>
        public static void InitUpdateDataFlatMetaData(BaseDataFlatMetaDataPads dataFlatMetaData, BaseDataFlatMetaDataPads lotMetaData, BaseDataFlatMetaDataPads checMetaData)
        {
            dataFlatMetaData.ParameterFacility = lotMetaData.ParameterFacility;
            dataFlatMetaData.ParameterOper = lotMetaData.ParameterOper;
            dataFlatMetaData.MeasLot = SpaceAggregationUtils.JoinStrings(checMetaData.MeasLot, lotMetaData.MeasLot);
            dataFlatMetaData.SiteKey = lotMetaData.SiteKey;
            dataFlatMetaData.SpaceInstanceName = lotMetaData.SpaceInstanceName;
            dataFlatMetaData.Equipment = SpaceAggregationUtils.JoinStrings(checMetaData.Equipment, lotMetaData.Equipment);
            dataFlatMetaData.Route = SpaceAggregationUtils.JoinStrings(checMetaData.Route, lotMetaData.Route);
        }
    }
}
