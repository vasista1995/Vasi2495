using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PDS.SpaceNew.Common.Data.Config;

namespace PDS.SpaceNew.Common
{
    public class ConfigFileController
    {
        public static AttributeMappingConfig GetSpaceAttributeMappingConfig(string siteKey, string spaceInstanceName)
        {
            var serializer = new JsonSerializer();
            using var streamReader = new StreamReader(Path.Combine("Resources", spaceInstanceName, siteKey, $"{nameof(AttributeMappingConfig)}.json"));
            using var jsonReader = new JsonTextReader(streamReader);
            var attributeMappingConfig = serializer.Deserialize<AttributeMappingConfig>(jsonReader);
            return attributeMappingConfig;
        }

        public static PropertyPlacementConfig GetSpaceAttributePADSStructureMappingConfig(string siteKey, string spaceInstanceName)
        {
            var serializer = new JsonSerializer();
            using var globalStreamReader = new StreamReader(Path.Combine("Resources", $"{nameof(PropertyPlacementConfig)}.json"));
            using var globalJsonReader = new JsonTextReader(globalStreamReader);
            var generalAttributeMappingConfig = serializer.Deserialize<PropertyPlacementConfig>(globalJsonReader);

            using var siteStreamReader = new StreamReader(Path.Combine("Resources", spaceInstanceName, siteKey, $"{nameof(PropertyPlacementConfig)}.json"));
            using var siteJsonReader = new JsonTextReader(siteStreamReader);
            var siteSpecificAttributeMappingConfig = serializer.Deserialize<PropertyPlacementConfig>(siteJsonReader);

            // Important: Get new HashSet which is not case sensitive
            generalAttributeMappingConfig.DataFlatMetaData = new HashSet<string>(generalAttributeMappingConfig.DataFlatMetaData, StringComparer.InvariantCultureIgnoreCase);
            generalAttributeMappingConfig.Data1List = new HashSet<string>(generalAttributeMappingConfig.Data1List, StringComparer.InvariantCultureIgnoreCase);
            generalAttributeMappingConfig.Data1ListRawValues = new HashSet<string>(generalAttributeMappingConfig.Data1ListRawValues, StringComparer.InvariantCultureIgnoreCase);

            siteSpecificAttributeMappingConfig.DataFlatMetaData = new HashSet<string>(siteSpecificAttributeMappingConfig.DataFlatMetaData, StringComparer.InvariantCultureIgnoreCase);
            siteSpecificAttributeMappingConfig.Data1List = new HashSet<string>(siteSpecificAttributeMappingConfig.Data1List, StringComparer.InvariantCultureIgnoreCase);
            siteSpecificAttributeMappingConfig.Data1ListRawValues = new HashSet<string>(siteSpecificAttributeMappingConfig.Data1ListRawValues, StringComparer.InvariantCultureIgnoreCase);
            siteSpecificAttributeMappingConfig.DataFlatStrangeMetaData = new HashSet<string>(siteSpecificAttributeMappingConfig.DataFlatStrangeMetaData, StringComparer.InvariantCultureIgnoreCase);

            // Override general attribute configuration by site specific attribute configuration
            // 1. Get distinct configured attributes in site specific configuration
            var siteSpecificAttributes = siteSpecificAttributeMappingConfig.DataFlatMetaData.Union(siteSpecificAttributeMappingConfig.Data1List)
                                                                                            .Union(siteSpecificAttributeMappingConfig.Data1ListRawValues)
                                                                                            .Union(siteSpecificAttributeMappingConfig.DataFlatStrangeMetaData)
                                                                                            .ToList().Distinct();

            // 2. Remove configuration of these attributes in general configuration
            generalAttributeMappingConfig.DataFlatMetaData.RemoveWhere(x => siteSpecificAttributes.Contains(x, StringComparer.OrdinalIgnoreCase));
            generalAttributeMappingConfig.Data1List.RemoveWhere(x => siteSpecificAttributes.Contains(x, StringComparer.OrdinalIgnoreCase));
            generalAttributeMappingConfig.Data1ListRawValues.RemoveWhere(x => siteSpecificAttributes.Contains(x, StringComparer.OrdinalIgnoreCase));

            // 3. Insert site specific attribute configuration into general configuration
            var generalDataFlatMetaData = new HashSet<string>(generalAttributeMappingConfig.DataFlatMetaData.Union(siteSpecificAttributeMappingConfig.DataFlatMetaData), StringComparer.InvariantCultureIgnoreCase);
            var generalData1List = new HashSet<string>(generalAttributeMappingConfig.Data1List.Union(siteSpecificAttributeMappingConfig.Data1List), StringComparer.InvariantCultureIgnoreCase);
            var generalData1ListRawValues = new HashSet<string>(generalAttributeMappingConfig.Data1ListRawValues.Union(siteSpecificAttributeMappingConfig.Data1ListRawValues), StringComparer.InvariantCultureIgnoreCase);
            generalAttributeMappingConfig.DataFlatMetaData = generalDataFlatMetaData;
            generalAttributeMappingConfig.Data1List = generalData1List;
            generalAttributeMappingConfig.Data1ListRawValues = generalData1ListRawValues;

            return generalAttributeMappingConfig;
        }

        public static IdCreationConfig GetSpaceIdCreationConfig(string siteKey, string spaceInstanceName)
        {
            var serializer = new JsonSerializer();
            using var streamReader = new StreamReader(Path.Combine("Resources", spaceInstanceName, siteKey, $"{nameof(IdCreationConfig)}.json"));
            using var jsonReader = new JsonTextReader(streamReader);
            var idCreationConfig = serializer.Deserialize<IdCreationConfig>(jsonReader);

            // Important: Get new Dictionary which is case insensitive
            idCreationConfig.IdCreationMappings = new Dictionary<string, IdCreationPartsConfig>(idCreationConfig.IdCreationMappings, StringComparer.InvariantCultureIgnoreCase);
            return idCreationConfig;
        }

        public static PropertyRawValueTransferConfig GetSpacePropertyRawValueTransferConfig(string siteKey, string spaceInstanceName)
        {
            var serializer = new JsonSerializer();
            using var streamReader = new StreamReader(Path.Combine("Resources", spaceInstanceName, siteKey, $"{nameof(PropertyRawValueTransferConfig)}.json"));
            using var jsonReader = new JsonTextReader(streamReader);
            var rawValueTransferPropertyConfig = serializer.Deserialize<PropertyRawValueTransferConfig>(jsonReader);

            // Important: Get new HashSet which is not case sensitive
            rawValueTransferPropertyConfig.RawValueTransferProperties = new HashSet<string>(rawValueTransferPropertyConfig.RawValueTransferProperties, StringComparer.InvariantCultureIgnoreCase);
            rawValueTransferPropertyConfig.MetaDataPropertiesToKeep = new HashSet<string>(rawValueTransferPropertyConfig.MetaDataPropertiesToKeep, StringComparer.InvariantCultureIgnoreCase);
            return rawValueTransferPropertyConfig;
        }

        public static AttributeCalculationConfig GetSpaceAttributeCalculationConfig(string siteKey, string spaceInstanceName)
        {
            var serializer = new JsonSerializer();
            using var streamReader = new StreamReader(Path.Combine("Resources", spaceInstanceName, siteKey, $"{nameof(AttributeCalculationConfig)}.json"));
            using var jsonReader = new JsonTextReader(streamReader);
            var rawValueTransferPropertyConfig = serializer.Deserialize<AttributeCalculationConfig>(jsonReader);

            return rawValueTransferPropertyConfig;
        }

        public static MethodInvokeConfig GetMethodInvokeConfig()
        {
            var serializer = new JsonSerializer();
            using var streamReader = new StreamReader(Path.Combine("Resources", $"{nameof(MethodInvokeConfig)}.json"));
            using var jsonReader = new JsonTextReader(streamReader);
            var spaceMethodInvokeConfig = serializer.Deserialize<SpaceMethodInvokeConfig>(jsonReader);

            // Translate Abbreviated methodInvokeConfig and enrich MethodInvokeMapping
            var translatedMethodInvokeMapping = GetTranslatedMehodInvokeMapping(spaceMethodInvokeConfig.AbbreviatedMethodInvokeMapping.InputParameterCountMethodInvokeMapping);

            // Important: Transform Dictionary to be Case independent (StringComparer.InvariantCultureIgnoreCase) This makes the code more reliable
            var ordinalIgnoreCaseMethodInvokeMapping = GetOrdinalIgnoreCaseMethodInvokeMapping(spaceMethodInvokeConfig.MethodInvokeMapping.InputParameterCountMethodInvokeMapping);
            // Add translated parameterInvokeMapping entries
            AddToMethodInvokeConfig(ordinalIgnoreCaseMethodInvokeMapping, translatedMethodInvokeMapping);

            return new MethodInvokeConfig() { InputParameterCountMethodInvokeMapping = ordinalIgnoreCaseMethodInvokeMapping };
        }

        private static void AddToMethodInvokeConfig(
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> targetParameterCountMethodInvokeMapping,
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> sourceParameterCountMethodInvokeMapping)
        {
            foreach (var sourceParameterCountMethodInvokeMappingKeyValuePair in sourceParameterCountMethodInvokeMapping)
            {
                string inputParameterCount = sourceParameterCountMethodInvokeMappingKeyValuePair.Key;
                var sourceMethodInvokeMapping = sourceParameterCountMethodInvokeMappingKeyValuePair.Value;

                if (!targetParameterCountMethodInvokeMapping.ContainsKey(inputParameterCount))
                {
                    targetParameterCountMethodInvokeMapping.Add(inputParameterCount, sourceMethodInvokeMapping);
                }
                else
                {
                    var targetMethodInvokeMapping = targetParameterCountMethodInvokeMapping[inputParameterCount];
                    AddMethodInvokeMappingInformation(targetMethodInvokeMapping, sourceMethodInvokeMapping, inputParameterCount);
                }
            }
        }

        private static void AddMethodInvokeMappingInformation(
            Dictionary<string, Dictionary<string, List<string>>> targetMethodInvokeMapping,
            Dictionary<string, Dictionary<string, List<string>>> sourceMethodInvokeMapping,
            string inputParameterCount)
        {
            foreach (var sourceMethodInvokeMappingKeyValuePair in sourceMethodInvokeMapping)
            {
                string methodName = sourceMethodInvokeMappingKeyValuePair.Key;
                var sourceParameterMapping = sourceMethodInvokeMappingKeyValuePair.Value;

                if (!targetMethodInvokeMapping.ContainsKey(methodName))
                {
                    targetMethodInvokeMapping.Add(methodName, sourceParameterMapping);
                }
                else
                {
                    var targetParameterMapping = targetMethodInvokeMapping[methodName];
                    AddParameterMappingInformation(targetParameterMapping, sourceParameterMapping, inputParameterCount, methodName);
                }
            }
        }

        private static void AddParameterMappingInformation(
            Dictionary<string, List<string>> targetParameterMapping,
            Dictionary<string, List<string>> sourceParameterMapping,
            string inputParameterCount,
            string methodName)
        {
            foreach (var sourceParameterMappingKeyValuePair in sourceParameterMapping)
            {
                string parameterAssignmentName = sourceParameterMappingKeyValuePair.Key;
                var methodInputParameters = sourceParameterMappingKeyValuePair.Value;

                if (!targetParameterMapping.ContainsKey(parameterAssignmentName))
                {
                    targetParameterMapping.Add(parameterAssignmentName, methodInputParameters);
                }
                else
                {
                    throw new InvalidOperationException($"MethodInvokeMapping dictionary already " +
                        $"contains a definition for {inputParameterCount}.{methodName}.{parameterAssignmentName}!");
                }
            }
        }

        private static Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> GetTranslatedMehodInvokeMapping(Dictionary<string, Dictionary<string, List<string>>> inputParameterCountMethodInvokeMapping)
        {
            var parameterInvokeMapping = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
            foreach (var inputParameterCountMethodInvokeValuePair in inputParameterCountMethodInvokeMapping)
            {
                var parameterCountInvokeMapping = new Dictionary<string, Dictionary<string, List<string>>>();

                string inputParameterCountString = inputParameterCountMethodInvokeValuePair.Key;
                int inputParameterCount = int.Parse(inputParameterCountString);
                var methodInvokeMapping = inputParameterCountMethodInvokeValuePair.Value;

                foreach (var methodParameterKeyValuePair in methodInvokeMapping)
                {
                    string methodName = methodParameterKeyValuePair.Key;
                    var parameterNames = methodParameterKeyValuePair.Value;

                    var parameterMapping = new Dictionary<string, List<string>>();
                    foreach (string parameterName in parameterNames)
                    {
                        // Add parameterName as many times as parameter count value
                        var parameters = new List<string>();
                        for (int i = 0; i < inputParameterCount; i++)
                        {
                            parameters.Add(parameterName);
                        };

                        if (parameterMapping.ContainsKey(parameterName))
                            throw new ArgumentException($"{parameterName} already exists in invoke method definition! Correct the {nameof(MethodInvokeConfig)}.json!");

                        parameterMapping.Add(parameterName, parameters);
                    }

                    parameterCountInvokeMapping.Add(methodName, parameterMapping);
                }

                parameterInvokeMapping.Add(inputParameterCountString, parameterCountInvokeMapping);
            }

            return parameterInvokeMapping;
        }

        private static Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> GetOrdinalIgnoreCaseMethodInvokeMapping(Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> inputParameterCountMethodInvokeMapping)
        {
            var parameterInvokeMapping = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var inputParameterCountMethodInvokeValuePair in inputParameterCountMethodInvokeMapping)
            {
                var parameterCountInvokeMapping = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.InvariantCultureIgnoreCase);

                string inputParameterCountString = inputParameterCountMethodInvokeValuePair.Key;
                var methodInvokeMapping = inputParameterCountMethodInvokeValuePair.Value;
                foreach (var methodInvokeMappingKeyValuePair in methodInvokeMapping)
                {
                    string methodName = methodInvokeMappingKeyValuePair.Key;
                    var parameterMapping = new Dictionary<string, List<string>>(methodInvokeMappingKeyValuePair.Value, StringComparer.InvariantCultureIgnoreCase);

                    parameterCountInvokeMapping.Add(methodName, parameterMapping);
                }

                parameterInvokeMapping.Add(inputParameterCountString, new Dictionary<string, Dictionary<string, List<string>>>(parameterCountInvokeMapping, StringComparer.InvariantCultureIgnoreCase));
            }

            return parameterInvokeMapping;
        }

        public static Dictionary<string, string> GetMappingConfig(SpaceE4A e4aDocument,
            IDictionary<int, Dictionary<string, string>> attributesMappingConfig)
        {
            int ldsId = GetLdsId(e4aDocument);
            if (!attributesMappingConfig.ContainsKey(ldsId))
            {
                Console.WriteLine($"Received unknown ldsId {ldsId}! Skipping document with id {e4aDocument.PKey}");
                return null;
            }

            var attributeMappingConfig = attributesMappingConfig[ldsId];
            return attributeMappingConfig;
        }


        private static int GetLdsId(SpaceE4A e4aEntry)
        {
            string ldsIdString = e4aEntry.SpaceAttributes.GetValueOrThrow(RequiredRawAttributes.LDSId).ToString();
            if (int.TryParse(ldsIdString, out int ldsId))
            {
                return ldsId;
            }
            else
            {
                throw new FormatException($"Conversion of LdsId with value {ldsIdString} to int failed!");
            }
        }
    }
}
