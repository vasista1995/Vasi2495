using System;
using System.Collections.Generic;
using System.Linq;
using PDS.Common.Source;
using PDS.SpaceNew.Common;

namespace PDS.SpaceNew.Source.Module.Data.SpaceModel
{

    /// <summary>
    /// It takes all the values from the sql statements and converts them into the below properties.
    /// </summary>
    public class SpaceEntry : ISourceRecord
    {
        private readonly HashSet<string> _dataLakeAttributeNames = new()
        {
            "LdsID",
            "CalcCreated",
            "ChannelType",
            "ILdsID",
            "SpcComExtern",
            "SpcComIntern",
            "AcceptFlag",
            "ChState",
            "UpdatedBy",
            "SpecTargetOrigin",
            "ViolCount",
            "ExtMaMV",
            "ExtEwmaMV",
            "ExtMsMV",
            "OutOfOrder",
            "EwmaS",
            "EwmaR",
            "Calc2ID",
            "LimitEnable",
            "ExtEwmaMVLCL",
            "ExtEwmaMVCenter",
            "ExtEwmaMVUCL",
            "ExtMaMVLCL",
            "ExtMaMVCenter",
            "ExtMaMVUCL",
            "EwmaSLCL",
            "EwmaSCenter",
            "EwmaSUCL",
            "EwmaRLCL",
            "EwmaRCenter",
            "EwmaRUCL",
            "ExtMSMVLCL",
            "ExtMSMVCenter",
            "ExtMSMVUCL",
            "ClOrigin",
            "MVLal",
            "MVUal",
            "RawLal",
            "RawUal",
            "SigmaLal",
            "SigmaUal",
            "RangeLal",
            "RangeUal",
            "ExtSpecLower",
            "ExtSpecUpper",
            "ExtSpecLimEnable",
            "CalculationStrategy"
        };

        public string UniqueSourceKey => PKey;
        public string PKey { get; set; }
        public IEnumerable<IDictionary<string, object>> SpaceAttributes { get; set; }
        public IEnumerable<IDictionary<string, object>> SpaceRawValueAttributes { get; set; }
        public IEnumerable<IDictionary<string, object>> SpaceDataLakeAttributes { get; set; }
        public static HashSet<string> RawValueAttributeNames { get; set; }

        public SpaceEntry(List<IDictionary<string, object>> spaceEntryRecords, string spaceEntryKey)
        {
            PKey = spaceEntryKey;
            SplitAttributes(spaceEntryRecords);
        }

        internal void SplitAttributes(IEnumerable<IDictionary<string, object>> sourceRecords)
        {
            var spaceAttributes = new List<IDictionary<string, object>>();
            var spaceRawValueAttributes = new List<IDictionary<string, object>>();
            var spaceDataLakeAttributes = new List<IDictionary<string, object>>();

            foreach (var sourceRecord in sourceRecords)
            {
                var rawValueProperties = new Dictionary<string, object>();
                var valueProperties = new Dictionary<string, object>();
                var dataLakeProperties = new Dictionary<string, object>();
                foreach (var entry in sourceRecord)
                {
                    if (RawValueAttributeNames.Contains(entry.Key, StringComparer.InvariantCultureIgnoreCase) ||
                        entry.Key.Contains("Raw_Exval_", StringComparison.OrdinalIgnoreCase) ||
                        entry.Key.Contains("Raw_Daval_", StringComparison.OrdinalIgnoreCase))
                    {
                        rawValueProperties[entry.Key] = entry.Value;
                    }
                    else if (_dataLakeAttributeNames.Contains(entry.Key, StringComparer.InvariantCultureIgnoreCase))
                    {
                        dataLakeProperties[entry.Key] = entry.Value;
                    }
                    else
                    {
                        valueProperties[entry.Key] = entry.Value;
                    }
                }

                spaceAttributes.Add(valueProperties);
                spaceRawValueAttributes.Add(rawValueProperties);
                spaceDataLakeAttributes.Add(dataLakeProperties);
            }

            SpaceAttributes = spaceAttributes;
            SpaceRawValueAttributes = spaceRawValueAttributes;
            SpaceDataLakeAttributes = spaceDataLakeAttributes;
        }
    }
}
