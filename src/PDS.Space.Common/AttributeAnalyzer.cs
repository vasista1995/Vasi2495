using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using PDS.Common.Utils;
using PDS.Space.Common.Data.E4AModel;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.Space.Common
{

    public class AttributeAnalyzer
    {
        private Dictionary<string, HashSet<string>> _attributeValues;
        private readonly Dictionary<string, List<string>> _newValuesAdded;
        private readonly string _libraryFileName = "attribute_library.json";
        private readonly string _analysisLogFileName = "analysis_log.txt";
        private readonly string _excludeDirectoryFileName = "exclude_keys.txt";
        private readonly long _regressionTestsPerLdsId = 30;
        private readonly string _generalExcludeDirectoryPath;
        private readonly string _regressionTestDirectoryPath;

        public AttributeAnalyzer(string regressionTestDirectoryPath)
        {
            _newValuesAdded = new Dictionary<string, List<string>>();
            _regressionTestDirectoryPath = regressionTestDirectoryPath;
            _generalExcludeDirectoryPath = Path.Combine(regressionTestDirectoryPath, _excludeDirectoryFileName);
        }

        public void AnalyzeFile(BaseSpaceEntry sourceRecord, string ldsIdString)
        {
            if (!long.TryParse(ldsIdString, out long ldsId))
            {
                Console.WriteLine($"Conversion of lds id with value {ldsIdString} to long was not possible!");
                return;
            }

            string libraryFilePath = Path.Combine(_regressionTestDirectoryPath, ldsIdString, _libraryFileName);
            string analysisLogFilePath = Path.Combine(_regressionTestDirectoryPath, ldsIdString, _analysisLogFileName);
            string ldsDirectoryPath = Path.Combine(_regressionTestDirectoryPath, ldsIdString);
            string sourceDirectoryPath = Path.Combine(ldsDirectoryPath, "Source");
            string excludeKeysDirectoryPath = Path.Combine(ldsDirectoryPath, "exclude_keys.txt");

            var directory = new DirectoryInfo(sourceDirectoryPath);
            if (!directory.Exists)
                Directory.CreateDirectory(sourceDirectoryPath);

            int fileCount = directory.GetFiles().Length;
            if (fileCount >= _regressionTestsPerLdsId)
                return; // Number of max regression test files reached

            LoadLibrary(libraryFilePath);

            HashSet<string> excludedKeys;
            if (File.Exists(excludeKeysDirectoryPath))
                excludedKeys = LoadExcludeKeysFromFile(excludeKeysDirectoryPath);
            else
            {
                excludedKeys = LoadExcludeKeysFromFile(_generalExcludeDirectoryPath);
            }

            string sourceRecordJson = JsonUtils.ToJson(sourceRecord);
            bool addToRegressionTests = AnalyzeJsonFile(sourceRecordJson, excludedKeys);
            if (addToRegressionTests)
            {
                string sampleDateString = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss_fff");
                string testFileName = $"test_{sampleDateString}.json";
                string sourceFilePath = Path.Combine(sourceDirectoryPath, testFileName);

                if (!Directory.Exists(sourceDirectoryPath))
                    Directory.CreateDirectory(sourceDirectoryPath);

                if (File.Exists(sourceFilePath))
                    File.Delete(sourceFilePath);

                SaveLibrary(libraryFilePath);
                LogAnalysisResult(testFileName, analysisLogFilePath);
                SaveObjectToJsonFile(sourceRecord, sourceFilePath);
            }
        }

        public string GetJsonString(BaseSpaceEntry spaceEntry)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(spaceEntry, options);
            return jsonString;
        }

        public static void SaveObjectToJsonFile(BaseSpaceEntry sourceRecord, string filePath)
        {
            var spaceE4A = JsonUtils.ToJson(sourceRecord, indent: true);

            // Write the JSON string to the file
            File.WriteAllText(filePath, spaceE4A);
            Console.WriteLine($"Object saved to: {filePath}");
        }

        private HashSet<string> LoadExcludeKeysFromFile(string filePath)
        {
            var excludeKeys = new HashSet<string>();
            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string trimmedKey = line.Trim();
                        if (!string.IsNullOrEmpty(trimmedKey))
                            excludeKeys.Add(trimmedKey);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading exclude keys from file: {ex.Message}");
            }
            return excludeKeys;
        }

        public bool AnalyzeJsonFile(string jsonText, HashSet<string> excludeKeys)
        {
            try
            {
                var jsonObj = JObject.Parse(jsonText);
                bool addToRegressionTests = false;

                AnalyzeJsonObject(jsonObj, "", ref addToRegressionTests, excludeKeys);
                return addToRegressionTests;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing JSON file: {ex.Message}");
                return false;
            }
        }

        public void LogAnalysisResult(string fileName, string analysisLogFilePath)
        {
            using (var writer = File.AppendText(analysisLogFilePath))
            {
                writer.Write($"{fileName}: [");
                bool first = true;
                foreach (var key in _newValuesAdded.Keys)
                {
                    if (!first)
                        writer.Write(", ");
                    writer.Write($"{key}: {string.Join(",", _newValuesAdded[key])}");
                    first = false;
                }
                writer.WriteLine("]");
            }
        }

        private void AnalyzeJsonObject(JObject jsonObject, string parentPath, ref bool addToRegressionTests, HashSet<string> excludeKeys)
        {
            foreach (var property in jsonObject.Properties())
            {
                string propertyName = property.Name;
                var propertyValue = property.Value;

                string propertyPath = string.IsNullOrEmpty(parentPath) ? propertyName : $"{parentPath}.{propertyName}";

                if (excludeKeys.Contains(propertyPath))
                    continue;

                if (propertyValue is JObject nestedObject)
                    AnalyzeJsonObject(nestedObject, propertyPath, ref addToRegressionTests, excludeKeys);
                else if (propertyValue is JArray array)
                {
                    foreach (var item in array)
                    {
                        if (item is JObject nestedArrayObject)
                            AnalyzeJsonObject(nestedArrayObject, propertyPath, ref addToRegressionTests, excludeKeys);
                    }
                }
                else
                {
                    string attributeValue = propertyValue.ToString();

                    if (!string.IsNullOrEmpty(attributeValue))
                    {
                        if (!_attributeValues.ContainsKey(propertyPath))
                            _attributeValues[propertyPath] = new HashSet<string>();

                        if (!_attributeValues[propertyPath].Contains(attributeValue))
                        {
                            if (!_newValuesAdded.ContainsKey(propertyPath))
                                _newValuesAdded[propertyPath] = new List<string>();

                            _attributeValues[propertyPath].Add(attributeValue);
                            _newValuesAdded[propertyPath].Add(attributeValue);
                            addToRegressionTests = true;
                        }
                    }
                }
            }
        }

        private void LoadLibrary(string libraryFilePath)
        {
            if (File.Exists(libraryFilePath))
            {
                string libraryJson = File.ReadAllText(libraryFilePath);
                _attributeValues = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, HashSet<string>>>(libraryJson);
            }
            else
            {
                _attributeValues = new Dictionary<string, HashSet<string>>();
            }
        }

        public void SaveLibrary(string libraryFilePath)
        {
            string libraryJson = Newtonsoft.Json.JsonConvert.SerializeObject(_attributeValues, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(libraryFilePath, libraryJson);
        }

        public void PrintLibrary()
        {
            foreach (var attribute in _attributeValues)
            {
                Console.WriteLine($"Attribute: {attribute.Key}");
                Console.WriteLine("Unique Values:");
                foreach (var value in attribute.Value)
                {
                    Console.WriteLine($"  - {value}");
                }
                Console.WriteLine();
            }
        }
    }
}
