using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace PDS.SpaceBE.Common.PADS.Module.Tests
{
    [TestClass]
    public abstract class LoaderPropertyEvaluatorTest
    {
        protected abstract string SiteKey { get; }
        protected abstract string SiteType { get; }
        private readonly string _spaceStrangeDataPropertiesPath;
        private readonly string _spaceStrangeData1ListRawValuesPropertiesPath;
        private readonly string _spaceLotPropertiesPath;
        private readonly string _spaceWaferPropertiesPath;
        private readonly string _spaceData1ListPropertiesPath;
        private readonly string _spaceFlatMetaDataPropertiesPath;

        public LoaderPropertyEvaluatorTest()
        {
            _spaceStrangeDataPropertiesPath = $@"../../../../../src/PDS.{SiteType}.{SiteKey}.PADS.Module/Data/PADSModel/StrangeDataFlatMetaDataPads.cs";
            _spaceStrangeData1ListRawValuesPropertiesPath = $@"../../../../../src/PDS.{SiteType}.{SiteKey}.PADS.Module/Data/PADSModel/Data1ListRawValuesPads.cs";

            _spaceLotPropertiesPath = $@"../../../../../src/PDS.{SiteType}.{SiteKey}.PADS.Module/Aggregations/LotAggregation.cs";
            _spaceWaferPropertiesPath = $@"../../../../../src/PDS.{SiteType}.{SiteKey}.PADS.Module/Aggregations/WaferAggregation.cs";
            _spaceData1ListPropertiesPath = $@"../../../../../src/PDS.{SiteType}.{SiteKey}.PADS.Module/Data/PADSModel/Data1ListPads.cs";
            _spaceFlatMetaDataPropertiesPath = $@"../../../../../src/PDS.{SiteType}.{SiteKey}.PADS.Module/Data/PADSModel/DataFlatMetaDataPads.cs";
        }

        [TestMethod]
        public void CheckLoaderLotFlatMetaDataImplementation()
        {
            var missingMethodPropertyNameMappings = GetFlatMetaDataLotMissingMethodPropertyNameMappings();
            EvaluateIncorrectImplementedProperties(missingMethodPropertyNameMappings);
        }

        private Dictionary<string, List<string>> GetFlatMetaDataLotMissingMethodPropertyNameMappings()
        {
            var localPropertyNames = ExtractPropertyNamesFromFile(_spaceFlatMetaDataPropertiesPath);
            var methodNames = new List<string>() { "CreateDataFlatMetaData", "UpdateDataFlatMetaData" };
            var missingMethodPropertyNameMappings = GetMissingMethodPropertyNameMappings(_spaceLotPropertiesPath, localPropertyNames, methodNames);
            return missingMethodPropertyNameMappings;
        }

        [TestMethod]
        public void CheckLoaderLotData1ListImplementation()
        {
            var missingMethodPropertyNameMappings = GetData1ListLotMissingMethodPropertyNameMappings();
            EvaluateIncorrectImplementedProperties(missingMethodPropertyNameMappings);
        }

        private Dictionary<string, List<string>> GetData1ListLotMissingMethodPropertyNameMappings()
        {
            var localPropertyNames = ExtractPropertyNamesFromFile(_spaceData1ListPropertiesPath);
            var filteredlocalPropertyNames = GetFilteredPropertyNames(localPropertyNames, new List<string>() { "Data1ListRawValues" });
            var methodNames = new List<string>() { "CreateData1list", "UpdateNewData1list", "UpdateOldData1list" };
            var missingMethodPropertyNameMappings = GetMissingMethodPropertyNameMappings(_spaceLotPropertiesPath, filteredlocalPropertyNames, methodNames);
            return missingMethodPropertyNameMappings;
        }

        [TestMethod]
        public void CheckLoaderLotDataRawValuesImplementation()
        {
            var missingMethodPropertyNameMappings = GetRawValuesLotMissingMethodPropertyNameMappings();
            EvaluateIncorrectImplementedProperties(missingMethodPropertyNameMappings);
        }

        private Dictionary<string, List<string>> GetRawValuesLotMissingMethodPropertyNameMappings()
        {
            var localPropertyNames = ExtractPropertyNamesFromFile(_spaceStrangeData1ListRawValuesPropertiesPath);
            var methodNames = new List<string>() { "AddExistingRawValues", "CreateNewRawValues" };
            var missingMethodPropertyNameMappings = GetMissingMethodPropertyNameMappings(_spaceLotPropertiesPath, localPropertyNames, methodNames);
            return missingMethodPropertyNameMappings;
        }

        [TestMethod]
        public void CheckLoaderLotStrangeMetaDataImplementation()
        {
            var missingMethodPropertyNameMappings = GetStrangeMetaDataLotMissingMethodPropertyNameMappings();
            EvaluateIncorrectImplementedProperties(missingMethodPropertyNameMappings);
        }

        private Dictionary<string, List<string>> GetStrangeMetaDataLotMissingMethodPropertyNameMappings()
        {
            var localPropertyNames = ExtractPropertyNamesFromFile(_spaceStrangeDataPropertiesPath);
            var methodNames = new List<string>() { "CreateStrangeMetaData", "UpdateDataFlatStrangeMetaData" };
            var missingMethodPropertyNameMappings = GetMissingMethodPropertyNameMappings(_spaceLotPropertiesPath, localPropertyNames, methodNames);
            return missingMethodPropertyNameMappings;
        }

        [TestMethod]
        public void CheckLoaderWaferFlatMetaDataImplementation()
        {
            var missingMethodPropertyNameMappings = GetFlatMetaDataWaferMissingMethodPropertyNameMappings();
            EvaluateIncorrectImplementedProperties(missingMethodPropertyNameMappings);
        }

        private Dictionary<string, List<string>> GetFlatMetaDataWaferMissingMethodPropertyNameMappings()
        {
            var missingMethodPropertyNameMappings = new Dictionary<string, List<string>>();
            if (File.Exists(_spaceWaferPropertiesPath))
            {
                var localPropertyNames = ExtractPropertyNamesFromFile(_spaceFlatMetaDataPropertiesPath);
                var methodNames = new List<string>() { "CreateDataFlatMetaData", "UpdateDataFlatMetaData" };
                missingMethodPropertyNameMappings = GetMissingMethodPropertyNameMappings(_spaceWaferPropertiesPath, localPropertyNames, methodNames);
            }

            return missingMethodPropertyNameMappings;
        }

        [TestMethod]
        public void CheckLoaderWaferData1ListImplementation()
        {
            var missingMethodPropertyNameMappings = GetData1ListWaferMissingMethodPropertyNameMappings();
            EvaluateIncorrectImplementedProperties(missingMethodPropertyNameMappings);
        }

        private Dictionary<string, List<string>> GetData1ListWaferMissingMethodPropertyNameMappings()
        {
            var missingMethodPropertyNameMappings = new Dictionary<string, List<string>>();
            if (File.Exists(_spaceWaferPropertiesPath))
            {
                var localPropertyNames = ExtractPropertyNamesFromFile(_spaceData1ListPropertiesPath);
                var filteredlocalPropertyNames = GetFilteredPropertyNames(localPropertyNames, new List<string>() { "Data1ListRawValues" });
                var methodNames = new List<string>() { "CreateData1list", "UpdateData1ListNew", "UpdateData1ListExisting" };
                missingMethodPropertyNameMappings = GetMissingMethodPropertyNameMappings(_spaceWaferPropertiesPath, filteredlocalPropertyNames, methodNames);
            }

            return missingMethodPropertyNameMappings;
        }

        [TestMethod]
        public void CheckLoaderWaferStrangeMetaDataImplementation()
        {
            var missingMethodPropertyNameMappings = GetStrangeMetaDataWaferMissingMethodPropertyNameMappings();
            EvaluateIncorrectImplementedProperties(missingMethodPropertyNameMappings);
        }

        private Dictionary<string, List<string>> GetStrangeMetaDataWaferMissingMethodPropertyNameMappings()
        {
            var waferMissingMethodPropertyNameMappings = new Dictionary<string, List<string>>();
            if (File.Exists(_spaceWaferPropertiesPath))
            {
                var waferLocalPropertyNames = ExtractPropertyNamesFromFile(_spaceStrangeDataPropertiesPath);
                var waferMethodNames = new List<string>() { "CreateStrangeMetaData", "UpdateStrangeMetaData" };
                waferMissingMethodPropertyNameMappings = GetMissingMethodPropertyNameMappings(_spaceWaferPropertiesPath, waferLocalPropertyNames, waferMethodNames);
            }

            return waferMissingMethodPropertyNameMappings;
        }

        [TestMethod]
        public void CheckLoaderStrangeMetaDataForMissingImplementation()
        {
            // Lot
            var lotMissingMethodPropertyNameMappings = GetStrangeMetaDataLotMissingMethodPropertyNameMappings();
            var missingLotPropertyNames = GetCommonPropertyNames(lotMissingMethodPropertyNameMappings);

            // Wafer
            var waferMissingMethodPropertyNameMappings = GetStrangeMetaDataWaferMissingMethodPropertyNameMappings();
            var missingWaferPropertyNames = GetCommonPropertyNames(waferMissingMethodPropertyNameMappings);

            var notImplementedPropertyNames = GetDuplicatePropertyNames(missingLotPropertyNames, missingWaferPropertyNames);
            EvaluateMissingPropertyImplementations(notImplementedPropertyNames);
        }

        [TestMethod]
        public void CheckLoaderData1ListForMissingImplementation()
        {
            // Lot
            var lotMissingMethodPropertyNameMappings = GetData1ListLotMissingMethodPropertyNameMappings();
            var missingLotPropertyNames = GetCommonPropertyNames(lotMissingMethodPropertyNameMappings);

            // Wafer
            var waferMissingMethodPropertyNameMappings = GetData1ListWaferMissingMethodPropertyNameMappings();
            var missingWaferPropertyNames = GetCommonPropertyNames(waferMissingMethodPropertyNameMappings);

            var notImplementedPropertyNames = GetDuplicatePropertyNames(missingLotPropertyNames, missingWaferPropertyNames);
            EvaluateMissingPropertyImplementations(notImplementedPropertyNames);
        }

        [TestMethod]
        public void CheckLoaderFlatMetaDataForMissingImplementation()
        {
            // Lot
            var lotMissingMethodPropertyNameMappings = GetFlatMetaDataLotMissingMethodPropertyNameMappings();
            var missingLotPropertyNames = GetCommonPropertyNames(lotMissingMethodPropertyNameMappings);

            // Wafer
            var waferMissingMethodPropertyNameMappings = GetFlatMetaDataWaferMissingMethodPropertyNameMappings();
            var missingWaferPropertyNames = GetCommonPropertyNames(waferMissingMethodPropertyNameMappings);

            var notImplementedPropertyNames = GetDuplicatePropertyNames(missingLotPropertyNames, missingWaferPropertyNames);
            EvaluateMissingPropertyImplementations(notImplementedPropertyNames);
        }

        [TestMethod]
        public void CheckLoaderData1ListRawValuesForMissingImplementation()
        {
            // Lot
            var lotMissingMethodPropertyNameMappings = GetRawValuesLotMissingMethodPropertyNameMappings();
            var missingLotPropertyNames = GetCommonPropertyNames(lotMissingMethodPropertyNameMappings);
            EvaluateMissingPropertyImplementations(missingLotPropertyNames);
        }

        private void EvaluateMissingPropertyImplementations(List<string> notImplementedPropertyNames)
        {
            if (notImplementedPropertyNames.Any())
            {
                Assert.Fail($"Following properties are declared but not implemented as wafer- or lotaggregation: \n{string.Join("\n", notImplementedPropertyNames)}");
            }
        }

        public static List<string> GetCommonPropertyNames(Dictionary<string, List<string>> inputDict)
        {
            if (!inputDict.Values.Any())
            {
                return new List<string>();
            }

            var commonValues = inputDict.Values.Aggregate((x, y) => x.Intersect(y).ToList());
            return commonValues;
        }

        public static List<string> GetDuplicatePropertyNames(List<string> list1, List<string> list2)
        {
            return list1.Intersect(list2, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private void EvaluateIncorrectImplementedProperties(Dictionary<string, List<string>> missingMethodPropertyNameMappings)
        {
            var messages = new List<string>();
            foreach (var missingMethodPropertyNameKeyPair in missingMethodPropertyNameMappings)
            {
                string methodName = missingMethodPropertyNameKeyPair.Key;
                var missingPropertyNames = missingMethodPropertyNameKeyPair.Value;
                string message = $"Missing in {methodName}(): {string.Join(", ", missingPropertyNames)}";
                messages.Add(message);
            }

            var missingProperties = missingMethodPropertyNameMappings.Values.SelectMany(x => x).ToList();
            if (missingProperties.Count > 0)
            {
                Assert.Fail($"There were incorrect implemented properties found: \n{string.Join("\n", messages)}");
            }
        }

        private List<string> GetFilteredPropertyNames(List<string> propertyNames, List<string> propertyNamesToRemove)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return propertyNames.Where(item => !propertyNamesToRemove.Contains(item, comparer)).ToList();
        }

        public static Dictionary<string, List<string>> GetMissingMethodPropertyNameMappings(string implementationPropertiesPath, List<string> propertyNames, List<string> methodNames)
        {
            var methodPropertyNameMappings = CreateDictionary(implementationPropertiesPath, propertyNames, methodNames);
            var propertyNamesToFilterOut = FindAllValuesForKeys(methodPropertyNameMappings);
            var missingMethodPropertyNameMappings = FilterDictionary(methodPropertyNameMappings, propertyNamesToFilterOut);
            return missingMethodPropertyNameMappings;
        }

        public static Dictionary<string, List<string>> CreateDictionary(string implementationPropertiesPath, List<string> propertyNames, List<string> methodNames)
        {
            var methodPropertyNameMappings = new Dictionary<string, List<string>>();
            foreach (string methodName in methodNames)
            {
                var createStrangeMetaDataLotProperties = ExtractPropertyNamesInAssignments(implementationPropertiesPath, new List<string>() { methodName }, true);
                var notImplementedPropertyNames = GetNotImplementedPropertyNames(propertyNames, createStrangeMetaDataLotProperties);
                if (notImplementedPropertyNames.Any())
                {
                    methodPropertyNameMappings.Add(methodName, notImplementedPropertyNames);
                }
                else
                {
                    methodPropertyNameMappings.Add(methodName, new List<string>());
                }
            }

            return methodPropertyNameMappings;
        }

        public static Dictionary<string, List<string>> FilterDictionary(Dictionary<string, List<string>> inputDict, List<string> valuesToFilter)
        {
            var filteredDict = new Dictionary<string, List<string>>();
            foreach (var kvp in inputDict)
            {
                var filteredValues = kvp.Value.Where(v => !valuesToFilter.Contains(v)).ToList();
                if (filteredValues.Any())
                {
                    filteredDict[kvp.Key] = filteredValues;
                }
                else
                {
                    filteredDict[kvp.Key] = new List<string>();
                }
            }

            return filteredDict;
        }

        public static List<string> FindAllValuesForKeys(Dictionary<string, List<string>> inputDict)
        {
            var firstKvp = inputDict.FirstOrDefault();
            if (firstKvp.Equals(default(KeyValuePair<string, List<string>>)))
            {
                return new List<string>();
            }

            var commonValues = new HashSet<string>(firstKvp.Value);
            foreach (var kvp in inputDict.Skip(1))
            {
                commonValues.IntersectWith(kvp.Value);
            }

            return commonValues.ToList();
        }

        private static List<string> GetNotImplementedPropertyNames(List<string> declaredPropertyNames, List<string> usedPropertiesInMethod)
        {
            var notImplementedPropertyNames = declaredPropertyNames.Except(usedPropertiesInMethod).ToList();
            return notImplementedPropertyNames;
        }

        private List<string> ExtractPropertyNamesFromFile(string filePath)
        {
            var propertyNames = new List<string>();

            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Could not find file {filePath}", nameof(filePath));

                string code = System.IO.File.ReadAllText(filePath);
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();

                var propertyDeclarations = root.DescendantNodes().OfType<PropertyDeclarationSyntax>();
                foreach (var propertyDeclaration in propertyDeclarations)
                {
                    propertyNames.Add(propertyDeclaration.Identifier.ValueText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return propertyNames;
        }

        public static string GetCSharpCodeWithoutComments(string fileContent)
        {
            fileContent = Regex.Replace(fileContent, @"^\s*//.*$", string.Empty, RegexOptions.Multiline);
            fileContent = Regex.Replace(fileContent, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);
            return fileContent;
        }

        public static List<string> ExtractPropertyNamesInAssignments(string filePath, List<string> methodNames, bool checkLeft = false)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Could not find file {filePath}", nameof(filePath));

            var leftVariableNames = new List<string>() { "dataFlatMetaData", "dataFlatStrangeMetaData", "data1List", "rawValues", "otherparams" };
            var rightVariableNames = new List<string>() { "e4aMetaData", "checkDataList", "checkMetaData", "checkRawValues", "lotparamvalues", "lot", "checkparams", "lotDataList", "lotMetaData" };
            var variableNamesToSearch = checkLeft ? leftVariableNames : rightVariableNames;

            string code = File.ReadAllText(filePath);
            code = GetCSharpCodeWithoutComments(code);
            var memberAccessRegex = new Regex(@"(?<Object>\w+)\.(?<Property>\w+)");
            var properties = new HashSet<string>();

            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();
            var nodes = root.DescendantNodes().ToList();
            var filteredNodes = nodes.Where(n => !methodNames.Any() || n.Ancestors().OfType<MethodDeclarationSyntax>().Any(n => methodNames.Contains(n.Identifier.ValueText, StringComparer.OrdinalIgnoreCase)));
            var memberAccessNodes = filteredNodes.Where(n => n is MemberAccessExpressionSyntax);
            foreach (var node in memberAccessNodes)
            {
                 var match = memberAccessRegex.Match(((MemberAccessExpressionSyntax) node).ToString());
                if (match.Success && variableNamesToSearch.Contains(match.Groups["Object"].Value))
                {
                    properties.Add(match.Groups["Property"].Value);
                }
            }

            return properties.Distinct().ToList();
        }
    }
}
