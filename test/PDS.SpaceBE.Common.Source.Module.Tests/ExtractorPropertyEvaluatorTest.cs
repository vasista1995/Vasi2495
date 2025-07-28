using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Match = System.Text.RegularExpressions.Match;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Data;

namespace PDS.SpaceBE.Common.Source.Module.Tests
{
    [TestClass]
    public abstract class ExtractorPropertyEvaluatorTest
    {
        protected abstract string SiteKey { get; }
        protected abstract string SiteType { get; }
        private readonly string _queryFilePath;
        private readonly string _spaceEntryPath;
        private readonly string _baseSpaceEntryPath = @"../../../../../src/PDS.Space.Common/Data/SpaceModel/BaseSpaceEntry.cs";
        private readonly string _spaceRawValuesEntryPath = @"../../../../../src/PDS.Space.Common/Data/SpaceModel/SpaceRawValuesEntry.cs";
        private readonly string _baseSpaceRawValuesEntryPath = @"../../../../../src/PDS.Space.Common/Data/SpaceModel/BaseSpaceRawValuesEntry.cs";

        private readonly string _baseSpaceE4AConverterPath = @"../../../../../src/PDS.Space.Common/BaseSpaceE4AConverter.cs";
        private readonly string _spaceE4AConverterPath;

        public ExtractorPropertyEvaluatorTest()
        {
            _queryFilePath = $@"../../../../../src/PDS.{SiteType}.{SiteKey}.Source.Module/Resources/SpaceSqlQuery.sql";
            _spaceEntryPath = $@"../../../../../src/PDS.{SiteType}.{SiteKey}.Source.Module/Data/SpaceModel/SpaceEntry.cs";
            _spaceRawValuesEntryPath = $@"../../../../../src/PDS.{SiteType}.{SiteKey}.Source.Module/Data/SpaceModel/SpaceRawValuesEntry.cs";
            _spaceE4AConverterPath = $@"../../../../../src/PDS.{SiteType}.{SiteKey}.Source.Module/SpaceE4AConverter.cs";
        }

        // Check: Extractor: Check for duplicate properties in query if they have the same source, otherwise coflicts can exist
        [TestMethod]
        public void CheckDuplicateQueryPropertiesSourceUniqueness()
        {
            var queryPropertyLines = GetQueryPropertyLines(_queryFilePath);
            var propertyNamesLineMapping = GetQueryPropertyNameLineMapping(queryPropertyLines);
            var distinctPopertyNameLineMappings = propertyNamesLineMapping.Select(pair => new KeyValuePair<string, List<string>>(pair.Key, pair.Value
                                                                                .Select(str => str.Trim().Replace(",", "").ToLowerInvariant())
                                                                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                                                                .ToList()))
                                                                           .Where(pair => pair.Value.Count > 1)
                                                                           .ToDictionary(pair => pair.Key, pair => pair.Value);

            if (distinctPopertyNameLineMappings.Keys.Any())
            {
                var messages = new List<string>();
                foreach (var distinctPopertyNameLineMapping in distinctPopertyNameLineMappings)
                {
                    string propertyName = distinctPopertyNameLineMapping.Key;
                    var propertyLines = distinctPopertyNameLineMapping.Value;
                    string message = $"{propertyName}: {string.Join(", ", propertyLines)}";
                    messages.Add(message);
                }

                Assert.Fail($"Found duplicate properties with different source property: \n" +
                            $"{string.Join("\n", messages)}");
            }
        }

        // Check: Extractor: Check for duplicate RawValue properties in query
        [TestMethod]
        public void CheckDuplicateRawValueQueryProperties()
        {
            var queryPropertyNames = GetRawValuesQueryPropertyNames(_queryFilePath);
            CheckDuplicateQueryProperties(queryPropertyNames);
        }

        // Check: Extractor: Check for duplicate not RawValue properties in query
        [TestMethod]
        public void CheckDuplicateNotRawValueQueryProperties()
        {
            var queryPropertyNames = GetNotRawValueQueryPropertyNames(_queryFilePath);
            CheckDuplicateQueryProperties(queryPropertyNames);
        }

        public void CheckDuplicateQueryProperties(List<string> queryPropertyNames)
        {
            var filteredQueryPropertyNames = GetFilteredPropertyNames(queryPropertyNames, new List<string>() { "BreakPoint", "PKey" });
            var duplicatePropertyNames = filteredQueryPropertyNames
                .GroupBy(x => x, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicatePropertyNames.Any())
            {
                Assert.Fail($"Found duplicate properties in query: {string.Join(", ", duplicatePropertyNames)}");
            }
        }

        // Check: Extractor: SpaceEntry properties are used in assignments
        [TestMethod]
        public void CheckQueryPropertyAssignmentUsage()
        {
            var queryPropertyNames = GetQueryPropertyNames(_queryFilePath);
            var filteredQueryPropertyNames = GetFilteredPropertyNames(queryPropertyNames, new List<string>() { "BreakPoint" });

            var baseConverterProperties = ExtractPropertyNamesInAssignments(_baseSpaceE4AConverterPath, new List<string>());
            var localConverterProperties = ExtractPropertyNamesInAssignments(_spaceE4AConverterPath, new List<string>() { "CreateFlatMetaData", "Convert", "CreateItem" });
            var filteredLocalPropertyNames = GetFilteredPropertyNames(localConverterProperties, new List<string>() { "SpaceRawValues" });
            var filteredBasePropertyNames = GetFilteredPropertyNames(baseConverterProperties, new List<string>() { "SpaceRawValues" });

            EvaluateQueryPropertyUsage(filteredQueryPropertyNames, filteredLocalPropertyNames, filteredBasePropertyNames);
        }

        private void EvaluateQueryPropertyUsage(List<string> queryPropertyNames, List<string> localConverterProperties, List<string> baseConverterProperties)
        {
            var additionalBasePropertyNames = baseConverterProperties.Except(queryPropertyNames).ToList();
            var additionalLocalPropertyNames = localConverterProperties.Except(queryPropertyNames).ToList();
            var additionalQueryPropertyNames = queryPropertyNames.Except(baseConverterProperties).Except(localConverterProperties).ToList();

            if (additionalLocalPropertyNames.Count > 0 || additionalQueryPropertyNames.Count > 0)
            {
                Assert.Fail($"There were additional local or query properties found:\n" +
                            $"Additional query properties without usage in code assignments: {string.Join(", ", additionalQueryPropertyNames)}\n" +
                            $"Additional local properties without query assignment: {string.Join(", ", additionalLocalPropertyNames)}\n" +
                            $"Additional base properties without query assignment: {string.Join(", ", additionalBasePropertyNames)}\n");
            }
        }

        // Check: Extractor: Query Properties -> SpaceEntry Properties
        [TestMethod]
        public void CheckSpaceEntryPropertiesMatch()
        {
            var queryPropertyNames = GetQueryPropertyNames(_queryFilePath);
            var localSpaceEntryPropertyNames = ExtractPropertyNamesFromFile(_spaceEntryPath);
            var baseSpaceEntryPropertyNames = ExtractPropertyNamesFromFile(_baseSpaceEntryPath);
            var spaceRawValuesEntryPropertyNames = ExtractPropertyNamesFromFile(_spaceRawValuesEntryPath);
            var baseSpaceRawValuesEntryPropertyNames = ExtractPropertyNamesFromFile(_baseSpaceRawValuesEntryPath);
            var localPropertyNames = localSpaceEntryPropertyNames.Concat(spaceRawValuesEntryPropertyNames).ToList();
            var basePropertyNames = baseSpaceEntryPropertyNames.Concat(baseSpaceRawValuesEntryPropertyNames).ToList();

            var filteredQueryPropertyNames = GetFilteredPropertyNames(queryPropertyNames, new List<string>() { "BreakPoint" });
            var filteredlocalPropertyNames = GetFilteredPropertyNames(localPropertyNames, new List<string>() { "SpaceRawValues" });
            EvaluateQueryPropertyUsage(filteredQueryPropertyNames, filteredlocalPropertyNames, basePropertyNames);
        }

        private List<string> GetRawValuesQueryPropertyNames(string filePath)
        {
            var queryPropertyNames = GetQueryPropertyNames(filePath);
            int index = queryPropertyNames.FindIndex(s => s.Equals("BreakPoint", StringComparison.OrdinalIgnoreCase));

            if (index != -1 && index < queryPropertyNames.Count - 1)
            {
                var rawValueQueryPropertyNames = queryPropertyNames.GetRange(index + 1, queryPropertyNames.Count - index - 1);
                return rawValueQueryPropertyNames;
            }
            else if (index != -1 && index == queryPropertyNames.Count - 1)
            {
                // "Target string is the last element in the list."
                return new List<string>();
            }

            throw new DataException($"Property key 'BreakPoint' was not found in query!");
        }

        private List<string> GetNotRawValueQueryPropertyNames(string filePath)
        {
            var queryProperties = GetQueryPropertyNames(filePath);
            int index = queryProperties.FindIndex(s => s.Equals("BreakPoint", StringComparison.OrdinalIgnoreCase));

            if (index != -1)
            {
                var notRawValuePropertyNames = queryProperties.GetRange(0, index);
                return notRawValuePropertyNames;
            }

            throw new DataException($"Property key 'BreakPoint' was not found in query!");
        }

        private List<string> GetQueryPropertyNames(string filePath)
        {
            var queryPropertyLines = GetQueryPropertyLines(filePath);
            var queryPropertyNamesLineMapping = GetQueryPropertyNameLineMapping(queryPropertyLines);
            var queryPropertyNames = queryPropertyNamesLineMapping.Keys.ToList();
            return queryPropertyNames;
        }

        private List<string> GetQueryPropertyLines(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            string filteredFileContent = RemoveSqlCommentsFromFile(fileContent);
            string propertyLinesContent = GetLinesBeforeFromKeyword(filteredFileContent);
            var propertyLines = GetFileLines(propertyLinesContent);
            return propertyLines;
        }

        // There are only more than 1 entry in list if there are duplicate property Names inside of the query
        private Dictionary<string, List<string>> GetQueryPropertyNameLineMapping(List<string> propertyLines)
        {
            var propertyNamesLineMapping = new Dictionary<string, List<string>>();
            foreach (string line in propertyLines)
            {
                MatchCollection matches = Regex.Matches(line, @"\b(\w+)\s*,?( )*$");
                foreach (Match match in matches)
                {
                    string propertyName = match.Groups[1].Value;
                    if (propertyNamesLineMapping.ContainsKey(propertyName))
                    {
                        propertyNamesLineMapping[propertyName].Add(line);
                    }
                    else
                    {
                        propertyNamesLineMapping.Add(propertyName, new List<string>() { line });
                    }
                }
            }

            return propertyNamesLineMapping;
        }

        private List<string> GetFileLines(string fileContent)
        {
            var lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
            return lines;
        }

        public static string GetCSharpCodeWithoutComments(string fileContent)
        {
            fileContent = Regex.Replace(fileContent, @"^\s*//.*$", string.Empty, RegexOptions.Multiline);
            fileContent = Regex.Replace(fileContent, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);
            return fileContent;
        }

        private string RemoveSqlCommentsFromFile(string fileContent)
        {
            fileContent = Regex.Replace(fileContent, @"/\*(.*?)\*/", "", RegexOptions.Singleline);
            fileContent = Regex.Replace(fileContent, @"--.*$", "", RegexOptions.Multiline);
            return fileContent;
        }

        private string GetLinesBeforeFromKeyword(string sqlQuery)
        {
            int lastIndex = sqlQuery.LastIndexOf("FROM", StringComparison.OrdinalIgnoreCase);

            if (lastIndex >= 0)
            {
                sqlQuery = sqlQuery.Substring(0, lastIndex);
            }
            else
            {
                throw new SyntaxErrorException("Did not find from keyword in sql query!");
            }

            return sqlQuery;
        }

        private List<string> GetFilteredPropertyNames(List<string> propertyNames, List<string> propertyNamesToRemove)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return propertyNames.Where(item => !propertyNamesToRemove.Contains(item, comparer)).ToList();
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

        public static List<string> ExtractPropertyNamesInAssignments(string filePath, List<string> methodNames, bool checkLeft = false)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Could not find file {filePath}", nameof(filePath));

            var leftVariableNames = new List<string>() { "e4a", "dataRawValues" };
            var rightVariableNames = new List<string>() { "entry", "rawvalues", "sourceRecord" };
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
