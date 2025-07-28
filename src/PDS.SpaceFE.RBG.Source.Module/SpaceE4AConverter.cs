using System;
using System.Collections.Generic;
using PDS.Common.ExtractionLog;
using PDS.Space.Common.Data.E4AModel;
using PDS.SpaceFE.RBG.Common.Data.E4AModel;
using PDS.SpaceFE.RBG.Source.Module.Data.SpaceModel;
using PDS.Space.Common;

namespace PDS.SpaceFE.RBG.Source.Module
{
    /// <summary>
    /// Converts Space source records into E4A documents.
    /// </summary>
    public static class SpaceE4AConverter
    {
        public static SpaceE4A Convert(SpaceEntry sourceRecord, IExtractionJobRun jobRun)
        {
            string idSource = $"SPACEACT1:{sourceRecord.PKey}";
            var data1ListE4A = new Data1ListE4A
            {
                SampleSize = sourceRecord.SampleSize
            };
            var data1ListRawVals = new List<Data1ListRawValuesE4A>();
            foreach (var rawvalues in sourceRecord.SpaceRawValues)
            {
                string wafer = ConvertWaferName(rawvalues.WaferName);
                var dataRawValues = new Data1ListRawValuesE4A();
                BaseSpaceE4AConverter.InitData1ListRawValues(dataRawValues, rawvalues);
                dataRawValues.WaferSequence = rawvalues.WaferSequence;
                dataRawValues.ProcessEquipment = rawvalues.ProcessEquipment;
                dataRawValues.MotherLotWafer = wafer;
                dataRawValues.X = rawvalues.X;
                dataRawValues.Y = rawvalues.Y;
                dataRawValues.GOF = rawvalues.GOF;
                dataRawValues.Slot = rawvalues.Slot;
                dataRawValues.TestPosition = rawvalues.TestPosition;
                data1ListRawVals.Add(dataRawValues);
            }
            data1ListE4A.Data1ListRawValues = data1ListRawVals;
            var productionAction = CreateProductionAction(sourceRecord);
            var item = CreateItem(sourceRecord);
            string sourceDataLevel = GetSourceDataLevel(sourceRecord.MotherlotWafer, sourceRecord.RvStoreFlag, idSource);
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
                ControlLimits = controlLimits,
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

        internal static string ConvertWaferName(string waferName)
        {
            if (waferName == null)
            {
                return waferName;
            }
            else if (waferName.Length < 8)
            {
                return waferName;
            }
            else
            {
                string motherLot = waferName[..8];
                int waferId = int.Parse(waferName.Substring(waferName.Length - 2));
                return motherLot + ":" + waferId.ToString();
            }
        }

        internal static string GetSourceDataLevel(string motherlotWafer, string rvStoreFlag, string idSource)
        {
            string sourceDataLevel = "";
            if (rvStoreFlag == "Y")
            {
                sourceDataLevel = "C";
            }
            else if (rvStoreFlag == "N")
            {
                if (motherlotWafer == null || motherlotWafer.Length <= 8)
                {
                    sourceDataLevel = "L";
                }
                else if (motherlotWafer.Length > 8)
                {
                    sourceDataLevel = "W";
                }
            }
            else
            {
                throw new InvalidOperationException($"Not Valid RVStoreFlag: {rvStoreFlag}, corresponding IdSource is {idSource}");
            }
            return sourceDataLevel;
        }
        private static DataFlatMetaDataE4A CreateFlatMetaData(SpaceEntry entry, string sourcedataLevel)
        {
            var e4a = new DataFlatMetaDataE4A();
            BaseSpaceE4AConverter.InitFlatMetaData(e4a, entry, sourcedataLevel);
            e4a.SPSNumber = entry.SPSNumber;
            e4a.ProcessGroup = entry.ProcessGroup;
            e4a.ProductType = entry.ProductType;
            e4a.Product = entry.Product;
            e4a.ProcessBatch = entry.ProcessBatch;
            e4a.ProcessLine = entry.ProcessLine;
            e4a.ProcessEquipment = entry.ProcessEquipment;
            e4a.Equipment = entry.MeasurementEquipment;
            e4a.EquipmentType = entry.MeasurementEquipmentType;
            e4a.BasicType = entry.BasicType;
            e4a.Layer = entry.Layer;
            e4a.Route = entry.Route;
            e4a.Recipe = entry.Recipe;
            e4a.MeasurementBatch = entry.MeasurementBatch;
            e4a.Design = entry.Design;
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
