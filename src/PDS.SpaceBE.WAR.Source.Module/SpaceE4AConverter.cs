using System;
using System.Collections.Generic;
using PDS.Common.ExtractionLog;
using PDS.Space.Common;
using PDS.Space.Common.Aggregations;
using PDS.Space.Common.Data.E4AModel;
using PDS.SpaceBE.WAR.Common.Data.E4AModel;
using PDS.SpaceBE.WAR.Source.Module.Data.SpaceModel;

namespace PDS.SpaceBE.WAR.Source.Module
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
            string idSource = $"SPACEACT3:{sourceRecord.PKey}";
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
            string sourceDataLevel = CreateSourceDataLevel(sourceRecord, idSource);
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

        public static string CreateSourceDataLevel(SpaceEntry sourceRecord, string idSource)
        {
            if (sourceRecord.RvStoreFlag == "N")
            {
                return "L";
            }
            else if (sourceRecord.RvStoreFlag == "Y")
            {
                return "C";
            }
            else
            {
                throw new InvalidOperationException($"Not Valid RVStoreFlag: {sourceRecord.RvStoreFlag}, corresponding IdSource is {idSource}");
            }
        }

        private static DataFlatMetaDataE4A CreateFlatMetaData(SpaceEntry entry, string sourceDataLevel)
        {
            var e4a = new DataFlatMetaDataE4A();
            BaseSpaceE4AConverter.InitFlatMetaData(e4a, entry, sourceDataLevel);
            e4a.Plant = entry.Plant;
            e4a.Line = entry.Line;
            e4a.ProductionOrder = entry.ProductionOrder;
            e4a.Batch = entry.Batch;
            e4a.MaterialNumber = entry.Label4Extr1 == "MaterialNumber" ? SpaceAggregationUtils.JoinStrings(entry.VariableExtr1, entry.MaterialNumber) : entry.MaterialNumber;
            e4a.ProductGroup = entry.ProductGroup;
            e4a.MeasurementRecipe = entry.MeasurementRecipe;
            e4a.ProcessEquipment = entry.ProcessEquipment;
            e4a.OrderType = entry.OrderType;
            e4a.QrkGroup = entry.QrkGroup;
            e4a.ProcessGroup = entry.ProcessGroup;
            e4a.SpcClass = entry.SpcClass;
            e4a.ParameterType = entry.ParameterType;
            e4a.LongParameterName = entry.LongParameterName;
            e4a.MaterialNumberText = entry.MaterialNumberText;
            e4a.Equipment = entry.MeasurementEquipment;
            e4a.EquipmentName = entry.MeasurementEquipmentName;
            e4a.ProcessEquipmentName = entry.ProcessEquipmentName;
            e4a.ProductName = entry.ProductName;
            e4a.RawValPos = entry.RawValPos;
            e4a.LengthX = entry.LengthX;
            e4a.Operator = entry.Operator;
            e4a.Status = entry.Status;
            e4a.UnitLength = entry.UnitLength;
            e4a.ParameterClass = entry.ParameterClass;
            e4a.TargetCpk = entry.TargetCpk;
            e4a.ProcessOwner = entry.ProcessOwner;
            e4a.Segment = entry.Segment;
            e4a.Package = entry.Package;
            e4a.F56Parameter = entry.F56Parameter;
            e4a.GroupId = entry.GroupId;
            e4a.Report = entry.Report;
            e4a.Module = entry.Module;
            if (entry.Label4Extr1 == "ProcessStep")
                e4a.ProcessStep = entry.VariableExtr1;
            else if (entry.Label4Extr3 == "ProcessStep")
                e4a.ProcessStep = entry.VariableExtr3;
            e4a.CleaningType = entry.Label4Extr1 == "CleaningType" ? entry.VariableExtr1 : null;
            e4a.Robot = entry.Label4Extr1 == "Robot" ? entry.VariableExtr1 : null;
            e4a.CuringProcess = entry.Label4Extr1 == "CuringProcess" ? entry.VariableExtr1 : null;
            e4a.BasePlatePosition = entry.Label4Extr1 == "BasePlatePosition" ? entry.VariableExtr1 : null;
            e4a.CleaningChamber = entry.Label4Extr1 == "CleaningChamber" ? entry.VariableExtr1 : null;
            e4a.USPower = entry.Label4Extr1 == "USPower" ? entry.VariableExtr1 : null;
            e4a.ChipSolderingLine = entry.Label4Extr1 == "ChipSolderingLine" ? entry.VariableExtr1 : null;
            e4a.ShearSurface = entry.Label4Extr2 == "ShearSurface" ? entry.VariableExtr2 : null;
            e4a.StencilThickness = entry.Label4Extr2 == "StencilThickness" ? entry.VariableExtr2 : null;
            e4a.ProcessType = entry.Label4Extr2 == "ProcessType" ? entry.VariableExtr2 : null;
            e4a.CarrierID = entry.Label4Extr2 == "CarrierID" ? entry.VariableExtr2 : null;
            e4a.Surface = entry.Label4Extr2 == "Surface" ? entry.VariableExtr2 : null;
            e4a.SonotrodeNumber = entry.Label4Extr2 == "SonotrodeNumber" ? entry.VariableExtr2 : null;
            e4a.DataType = entry.Label4Extr2 == "DataType" ? entry.VariableExtr2 : null;
            e4a.FootPosition = entry.Label4Extr3 == "FootPosition" ? entry.VariableExtr3 : null;
            e4a.Zone = entry.Label4Extr3 == "Zone" ? entry.VariableExtr3 : null;
            e4a.CarrierFamily = entry.Label4Extr3 == "CarrierFamily" ? entry.VariableExtr3 : null;
            e4a.BondParameter = entry.Label4Extr3 == "BondParameter" ? entry.VariableExtr3 : null;
            e4a.PadSize = entry.Label4Extr4 == "PadSize" ? entry.VariableExtr4 : null;
            e4a.WireDiameter = entry.Label4Extr4 == "WireDiameter" ? entry.VariableExtr4 : null;
            e4a.TerminalConnector = entry.Label4Extr4 == "TerminalConnector" ? entry.VariableExtr4 : null;
            e4a.TerminalType = entry.Label4Extr5 == "TerminalType" ? entry.VariableExtr5 : null;
            e4a.WireMaterial = entry.Label4Extr5 == "WireMaterial" ? entry.VariableExtr5 : null;
            e4a.Material = entry.Label4Extr5 == "Material" ? entry.VariableExtr5 : null;
            e4a.Identifier = entry.Label4Extr6 == "Identifier" ? entry.VariableExtr6 : null;
            e4a.CarrierAnvilPosition = entry.Label4Extr6 == "CarrierAnvilPosition" ? entry.VariableExtr6 : null;
            e4a.PastePrinter = entry.Label4Extr6 == "PastePrinter" ? entry.VariableExtr6 : null;
            e4a.VaduFrameID = entry.Label4Extr6 == "VaduFrameID" ? entry.VariableExtr6 : null;

            // For Warstein Backened there is no BasicType information available.
            e4a.BasicType = null;

            return e4a;
        }

        private static ItemE4A CreateItem(SpaceEntry entry)
        {
            return new ItemE4A()
            {
                Id = "Measlot:" + entry.ProductionOrder,
                IdSystemName = entry.IdSystemName,
                Type = "Fauf"
            };
        }

        private static ProductionActionE4A CreateProductionAction(SpaceEntry entry)
        {
            string id = "SPACEACT1:" + entry.Sitekey + ":" + entry.SpaceInstanceName + ":" + entry.Line + "::" + entry.ParameterName + ":" + entry.ChannelId;
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
