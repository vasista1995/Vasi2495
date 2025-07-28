using System;
using System.Collections.Generic;
using System.Linq;
using PDS.Common.ExtractionLog;
using PDS.Space.Common.Data.E4AModel;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.Source.Module.Data.SpaceModel;

namespace PDS.SpaceNew.Source.Module
{
    /// <summary>
    /// Converts Space source records into E4A documents.
    /// </summary>
    public class SpaceE4AConverter
    {
        /// <summary>
        /// This method is to Convert the source record into e4a file to be published in the kafka producer.
        /// </summary>
        /// <param name="spaceEntry"></param>
        /// <param name="jobRun"></param>
        /// <param name="appName"></param>
        public SpaceE4A Convert(SpaceEntry spaceEntry, IExtractionJobRun jobRun, string appName)
        {
            var spaceE4A = ConvertToSpaceE4A(spaceEntry, jobRun, appName);
            return spaceE4A;
        }

        private SpaceE4A ConvertToSpaceE4A(SpaceEntry spaceEntry, IExtractionJobRun jobRun, string appName)
        {
            var systemLog = CreateSystemLog(spaceEntry, jobRun);
            var unifiedSpaceAttributes = CombineData(spaceEntry.SpaceAttributes);
            var unifiedSpaceDataLakeAttributes = CombineData(spaceEntry.SpaceDataLakeAttributes);
            unifiedSpaceAttributes.Add(RequiredConvertedAttributes.SiteKey, jobRun.SiteKey);

            string spaceInstanceName = appName["Space".Length..];
            unifiedSpaceAttributes.Add(RequiredConvertedAttributes.SpaceInstanceName, spaceInstanceName);

            var spaceE4A = new SpaceE4A(systemLog, unifiedSpaceAttributes, unifiedSpaceDataLakeAttributes, spaceEntry.SpaceRawValueAttributes, spaceEntry.PKey);
            return spaceE4A;
        }

        public static IDictionary<string, object> CombineData(IEnumerable<IDictionary<string, object>> spaceAttributeCollection)
        {
            var spaceAttributesValuesMapping = GetSpaceAttributesValuesMapping(spaceAttributeCollection);
            var spaceAttributesResultMapping = GetSpaceAttributesResultMapping(spaceAttributesValuesMapping);
            return spaceAttributesResultMapping;
        }

        private static IDictionary<string, List<object>> GetSpaceAttributesValuesMapping(IEnumerable<IDictionary<string, object>> spaceAttributeCollection)
        {
            var spaceAttributesValuesMapping = new Dictionary<string, List<object>>();

            foreach (var spaceAttributes in spaceAttributeCollection)
            {
                foreach (var spaceAttribute in spaceAttributes)
                {
                    if (!spaceAttributesValuesMapping.ContainsKey(spaceAttribute.Key))
                    {
                        spaceAttributesValuesMapping[spaceAttribute.Key] = new List<object> { spaceAttribute.Value};
                    }
                    else
                    {
                        var valuesList = spaceAttributesValuesMapping[spaceAttribute.Key];
                        if (!valuesList.Contains(spaceAttribute.Value))
                        {
                            valuesList.Add(spaceAttribute.Value);
                        }
                    }
                }
            }

            return spaceAttributesValuesMapping;
        }

        private static IDictionary<string, object> GetSpaceAttributesResultMapping(IDictionary<string, List<object>> spaceAttributesValuesMapping)
        {
            var spaceAttributesResultMapping = new Dictionary<string, object>();
            foreach (var spaceAttributeValuesPair in spaceAttributesValuesMapping)
            {
                var values = spaceAttributeValuesPair.Value.Distinct();
                if (values.Count() > 1)
                    throw new InvalidOperationException($"Multiple values were found for attribute {spaceAttributeValuesPair.Key}! Values: {string.Join(",", values)}");

                spaceAttributesResultMapping[spaceAttributeValuesPair.Key] = values.First();
            }

            return spaceAttributesResultMapping;
        }

        private BaseSystemLogE4A CreateSystemLog(SpaceEntry sourceRecord, IExtractionJobRun jobRun)
        {
            return new BaseSystemLogE4A(sourceRecord, jobRun);
        }
    }
}
