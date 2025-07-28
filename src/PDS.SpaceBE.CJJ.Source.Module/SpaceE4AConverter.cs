using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using PDS.Common.ExtractionLog;
using PDS.Space.Common;
using PDS.Space.Common.Data.E4AModel;
using PDS.SpaceBE.CJJ.Common.Data.E4AModel;
using PDS.SpaceBE.CJJ.Source.Module.Data.SpaceModel;

namespace PDS.SpaceBE.CJJ.Source.Module
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
                dataRawValues.Machine = rawvalues.Machine;
                data1ListRawVals.Add(dataRawValues);
            }
            data1ListE4A.Data1ListRawValues = data1ListRawVals;
            var productionAction = CreateProductionAction(sourceRecord);
            var item = CreateItem(sourceRecord);
            string sourceDataLevel = GetSourceDataLevel( sourceRecord.RvStoreFlag, idSource);
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

        public static string GetSourceDataLevel(string rvStoreFlag, string idSource)
        {
            string sourceDataLevel;
            if (rvStoreFlag == "N")
            {
                sourceDataLevel = "L";
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
            e4a.WireSize = entry.WireSize;
            e4a.Process = entry.Process;
            e4a.WireType = entry.WireType;
            e4a.ChipType = entry.ChipType;
            e4a.Classification = entry.Classification;
            e4a.WaferThickness = entry.WaferThickness;
            e4a.Shift = entry.Shift;
            e4a.CartridgeID = entry.CartridgeID;
            e4a.Grade = entry.Grade;

            e4a.Machine = entry.Machine;
            e4a.PackageFamily = entry.PackageFamily;
            e4a.Equipment = entry.MeasurementEquipment;
            e4a.Operator = entry.Operator;
            e4a.Package = entry.Package;
            e4a.Device = entry.Device;


            // StrangeMetaData assignments
            e4a.SpecName = entry.SpecName;
            e4a.SalesName = entry.SalesName;
            e4a.BasicType = entry.BasicType;
            e4a.DeviceFamily = entry.DeviceFamily;
            e4a.PackageGroup = entry.PackageGroup;
            e4a.PackageClass = entry.PackageClass;
            e4a.BusinessSegment = entry.BusinessSegment;
            e4a.MoveInQuantity = entry.MoveInQuantity;
            e4a.MoveOutQuantity = entry.MoveOutQuantity;
            e4a.BeSegmentName = entry.BeSegmentName;
            e4a.ManufacturingWipLevel = entry.ManufacturingWipLevel;
            e4a.MaterialType = entry.MaterialType;
            e4a.ProcessOwner = entry.ProcessOwner;
            e4a.Published = entry.Published;
            e4a.GroupId = entry.GroupId;

            // FlatMetaData assignments
            e4a.Baunumber = entry.Baunumber;
            e4a.OperatorId = entry.OperatorId;
            e4a.Material = entry.Material;

            e4a.ParameterClass = entry.ParameterClass;
            e4a.CFComment = entry.CFComment;
            e4a.TargetCpk = entry.TargetCpk;
            e4a.Segment = entry.Segment;
            e4a.Module = entry.Module;
            e4a.F56Parameter = entry.F56Parameter;
            e4a.SpecialCharacteristics = entry.SpecialCharacteristics;
            e4a.FourDReport = entry.FourDReport;
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
