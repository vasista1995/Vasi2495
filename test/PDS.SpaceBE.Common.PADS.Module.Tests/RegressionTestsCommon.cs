using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Text.RegularExpressions;
using System.Reflection;

namespace PDS.SpaceBE.Common.PADS.Module.Tests
{

    [TestClass]
    public abstract class RegressionTestsCommon : IRegressionTestPADSUpdater
    {
        private const string SourceFolderName = "Source";
        private const string ExpectedFolderName = "Expected";

        public abstract string GetSpacePadsJson(string sourceFilePath);
        public abstract void TestCreateOperLotAggregates(string sourceFilePath, string expectedFilePath);

        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void TestDynamic(string sourceFilePath, string expectedFilePath)
        {
            string expectedJson = File.ReadAllText(expectedFilePath);
            string e4AdocumentJson = GetSpacePadsJson(sourceFilePath);
            e4AdocumentJson.Should().BeEquivalentTo(expectedJson);
        }

        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void TestCreateOperLotAggregatesMain(string sourceFilePath, string expectedFilePath)
        {
            TestCreateOperLotAggregates(sourceFilePath, expectedFilePath);
        }

        public void UpdatePADSRegressionTests(string assemblyPath)
        {
            var testCases = GetTestDataByAssembly(assemblyPath).ToList();
            foreach (var testCase in testCases)
            {
                string sourceFilePath = testCase[0].ToString();
                string expectedFilePath = testCase[1].ToString();
                string directoryPath = Path.GetDirectoryName(expectedFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string updatedJson = GetSpacePadsJson(sourceFilePath);
                File.WriteAllText(expectedFilePath, updatedJson);
            }
        }

        public static IEnumerable<object[]> GetTestData()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            return GetTestDataByAssembly(assemblyPath);
        }

        public static IEnumerable<object[]> GetTestDataByAssembly(string assemblyPath)
        {
            string projectDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(assemblyPath))));
            string resourcesDirectoryPath = Path.Combine(projectDirectory, "Resources", "RegressionTests");
            var ldsDirectoryPaths = Directory.GetDirectories(resourcesDirectoryPath, "*", SearchOption.TopDirectoryOnly);

            foreach (string ldsDirectoryPath in ldsDirectoryPaths)
            {
                string sourceDirectoryPath = Path.Combine(ldsDirectoryPath, SourceFolderName);
                var sourceFilePaths = Directory.GetFiles(sourceDirectoryPath, "*.json");
                foreach (string sourceFilePath in sourceFilePaths)
                {
                    string pattern = $"\\b{SourceFolderName}\\b";
                    string replacement = ExpectedFolderName;
                    string expectedFilePath = Regex.Replace(sourceFilePath, $"^(.*?){pattern}(?!.*?{pattern})", $"$1{replacement}");
                    if (expectedFilePath != null)
                    {
                        yield return new object[] { sourceFilePath, expectedFilePath };
                    }
                }
            }
        }
    }
}
