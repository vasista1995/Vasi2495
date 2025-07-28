using System;
using System.Collections.Generic;
using PDS.Common.ExtractionLog;
using PDS.Space.Common;
using PDS.Space.Common.Data.E4AModel;
using PDS.SpaceBE.RBG.Common.Data.E4AModel;
using PDS.SpaceBE.RBG.Source.Module.Data.SpaceModel;

namespace PDS.SpaceBE.RBG.Source.Module
{
    /// <summary>
    /// Converts Space source records into E4A documents.
    /// </summary>
    public static class SpaceE4AConverter
    {
        /// <summary>
        /// This method is to Convert the source record into e4a file to be published in the kafka producer.
        /// </summary>
        /// <param name="sourceRecord"></param>
        /// <param name="jobRun"></param>
        public static SpaceE4A Convert(SpaceEntry sourceRecord, IExtractionJobRun jobRun)
        {
            string idSource = $"SPACEACT2:{sourceRecord.PKey}";
            var data1ListE4A = new Data1ListE4A
            {
                SampleSize = sourceRecord.SampleSize
            };
            var data1ListRawVals = new List<Data1ListRawValuesE4A>();
            foreach (var rawvalues in sourceRecord.SpaceRawValues)
            {
                var dataRawValues = new Data1ListRawValuesE4A();
                BaseSpaceE4AConverter.InitData1ListRawValues(dataRawValues, rawvalues);
                dataRawValues.WaferLot = rawvalues.WaferLot;
                data1ListRawVals.Add(dataRawValues);
            }
            data1ListE4A.Data1ListRawValues = data1ListRawVals;
            var productionAction = CreateProductionAction(sourceRecord);
            var item = CreateItem(sourceRecord);
            string sourceDataLevel = GetSourceDataLevel(sourceRecord.WaferLot, sourceRecord.RvStoreFlag, idSource);
            var dataFlatMetaData = CreateFlatMetaData(sourceRecord, sourceDataLevel);
            var measurementSpecLimits = new MeasurementSpecLimitsE4A();
            BaseSpaceE4AConverter.InitMeasurementSpecLimits(measurementSpecLimits, sourceRecord);
            var spaceAggregates = new SpaceAggregatesE4A();
            BaseSpaceE4AConverter.InitSpaceAggregates(spaceAggregates, sourceRecord);
            var controlLimits = new ControlLimitsE4A();
            BaseSpaceE4AConverter.InitControlLimits(controlLimits, sourceRecord);
            var systemLog = CreateSystemLog(sourceRecord, jobRun);
            data1ListE4A.DataFlatLimits = new DataFlatLimitsE4A()
            {
                CkcId = sourceRecord.CkcId,
                MeasurementSpecLimits = measurementSpecLimits,
                SpaceAggregates = spaceAggregates,
                ControlLimits = controlLimits
            };
            return new SpaceE4A(systemLog)
            {
                IdSource = idSource,
                ProductionAction = productionAction,
                Item = item,
                DataFlatMetaData = dataFlatMetaData,
                Data1List = data1ListE4A
            };
        }

        public static string GetSourceDataLevel(string waferLot, string rvStoreFlag, string idSource)
        {
            string sourceDataLevel;
            if (rvStoreFlag == "N")
            {
                if (waferLot == null)
                {
                    sourceDataLevel = "L";
                }
                else
                {
                    sourceDataLevel = "W";
                }
            }
            else if (rvStoreFlag == "Y")
            {
                sourceDataLevel = "C";
            }
            else
            {
                throw new InvalidOperationException($"Not Valid RVStoreFlag: {rvStoreFlag}, corresponding IdSource is {idSource}");
            }
            return sourceDataLevel;
        }

        private static DataFlatMetaDataE4A CreateFlatMetaData(SpaceEntry entry, string sourceDataLevel)
        {
            var e4a = new DataFlatMetaDataE4A();
            BaseSpaceE4AConverter.InitFlatMetaData(e4a, entry, sourceDataLevel);
            e4a.ProductType = entry.ProductType;
            e4a.Product = entry.Product;
            e4a.ProductName = entry.ProductName;
            e4a.Equipment = entry.MeasurementEquipment;
            e4a.BasicType = entry.BasicType;
            e4a.Route = entry.Route;
            e4a.Package = entry.Package;
            e4a.PackageClass = entry.PackageClass;
            e4a.PackageFamily = entry.PackageFamily;
            e4a.PackageGroup = entry.PackageGroup;
            e4a.BeSegmentName = entry.BeSegmentName;
            e4a.BeSort = entry.BeSort;
            e4a.Data1 = entry.Data1;
            e4a.Data2 = entry.Data2;
            e4a.Data3 = entry.Data3;
            e4a.SalesName = entry.SalesName;
            e4a.SampleType = entry.SampleType;
            e4a.Segment = entry.Segment;
            e4a.Module = entry.Module;
            e4a.SpecName = entry.SpecName;
            e4a.OperatorId = entry.OperatorId;
            e4a.Owner = entry.Owner;
            e4a.MoveOutQuantity = entry.MoveOutQuantity;
            e4a.MoveInQuantity = entry.MoveInQuantity;
            e4a.OriginSampleSize = entry.OriginSampleSize;
            e4a.ManufacturingWipLevel = entry.ManufacturingWipLevel;
            e4a.Material = entry.Material;
            e4a.Device = entry.Device;
            e4a.DeviceFamily = entry.DeviceFamily;
            e4a.ErrorCode = entry.ErrorCode;
            e4a.Group1 = entry.Group1;
            e4a.Group2 = entry.Group2;
            e4a.Group3 = entry.Group3;
            e4a.GroupId = entry.GroupId;
            e4a.EquipmentType = entry.EquipmentType;
            e4a.Pin = entry.Pin;
            e4a.UserClass1 = entry.UserClass1;
            e4a.UserClass2 = entry.UserClass2;
            e4a.UserClass3 = entry.UserClass3;
            e4a.Wire = entry.Wire;
            return e4a;
        }

        private static ItemE4A CreateItem(SpaceEntry entry)
        {
            return new ItemE4A()
            {
                Id = "Measlot:" + entry.Lot,
                IdSystemName = "Measlot",
                Type = "Lot"
            };
        }

        private static ProductionActionE4A CreateProductionAction(SpaceEntry entry)
        {
            string id = "SPACEACT1:" + entry.Sitekey + ":" + entry.SpaceInstanceName + ":" + entry.Facility + ":" + entry.Operation
                            + ":" + entry.ParameterName + ":" + entry.ChannelId;
            return new ProductionActionE4A()
            {
                Id = id,
                Type = "SpaceAction"
            };
        }

        private static BaseSystemLogE4A CreateSystemLog(SpaceEntry sourceRecord, IExtractionJobRun jobRun)
        {
            return new BaseSystemLogE4A(sourceRecord, jobRun);
        }
    }
}
