using System;
using System.Collections.Generic;
using PDS.Common.ExtractionLog;
using PDS.Space.Common;
using PDS.Space.Common.Data.E4AModel;
using PDS.SpaceBE.TIJ.Common.Data.E4AModel;
using PDS.SpaceBE.TIJ.Source.Module.Data.SpaceModel;

namespace PDS.SpaceBE.TIJ.Source.Module
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
                data1ListRawVals.Add(dataRawValues);
            }
            data1ListE4A.Data1ListRawValues = data1ListRawVals;
            var productionAction = CreateProductionAction(sourceRecord);
            var item = CreateItem(sourceRecord);
            string sourceDataLevel = GetSourceDataLevel(sourceRecord.RvStoreFlag, idSource);
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
            e4a.Equipment = entry.MeasurementEquipment;
            e4a.Package = entry.Package;
            e4a.PackageFamily = entry.PackageFamily;
            e4a.Segment = entry.Segment;
            e4a.Module = entry.Module;

            // Consistent in all BE sites
            e4a.F56Parameter = entry.F56Parameter;
            e4a.FourDReport = entry.FourDReport;
            e4a.ProcessEquipment = entry.ProcessEquipment;
            e4a.SpecialCharacteristics = entry.SpecialCharacteristics;
            e4a.TargetCpk = entry.TargetCpk;
            e4a.PackageCF = entry.PackageCF;
            e4a.ParameterClass = entry.ParameterClass;

            // Consistent in multiple sites
            e4a.Category = entry.Category;
            e4a.Classification = entry.Classification;
            e4a.DieSize = entry.DieSize;
            e4a.Operator = entry.Operator;
            e4a.WireSize = entry.WireSize;
            e4a.WaferThickness = entry.WaferThickness;

            // Only for TIJ site
            e4a.HexSize = entry.HexSize;
            e4a.Layout = entry.Layout;
            e4a.LeadFormCategory = entry.LeadFormCategory;
            e4a.MoldCompound = entry.MoldCompound;
            e4a.PartNumber = entry.PartNumber;
            e4a.ProductionVersion = entry.ProductionVersion;
            e4a.SolderType = entry.SolderType;
            e4a.Track = entry.Track;
            e4a.Area = entry.Area;

            // Only Extractor side properties
            e4a.Grade = entry.Grade;

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
            string id = "SPACEACT1:" + entry.Sitekey + ":" + entry.SpaceInstanceName + ":" + entry.Area + ":" + entry.Module
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
