using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDS.SpaceBE.Common.Source.Module.Tests
{

    [TestClass]
    public abstract class RegressionTestsCommon : IRegressionTestSourceUpdater
    {
        private const string SourceFolderName = "Source";
        private const string ExpectedFolderName = "Expected";

        public abstract string Gete4ADocumentJson(string sourceFilePath);

        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void TestDynamic(string sourceFilePath, string expectedFilePath)
        {
            string sourcee4AdocumentJson = Gete4ADocumentJson(sourceFilePath);
            string expectede4AJson = File.ReadAllText(expectedFilePath);
            sourcee4AdocumentJson.Should().BeEquivalentTo(expectede4AJson);
        }

        public void UpdateSourceRegressionTests(string assemblyPath)
        {
            var testCases = GetTestDataByAssembly(assemblyPath).ToList();
            foreach (object[] testCase in testCases)
            {
                string sourceFilePath = testCase[0].ToString();
                string expectedFilePath = testCase[1].ToString();
                string directoryPath = Path.GetDirectoryName(expectedFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string updatedJson = Gete4ADocumentJson(sourceFilePath);
                File.WriteAllText(expectedFilePath, updatedJson);
            }
        }

        public void PrepareLoaderInput(string assemblyPath, string site)
        {
            var loaderSourceTestFilePaths = GetLoaderSourceTestFilePaths(assemblyPath);
            CopyToLoaderSourceDirectory(loaderSourceTestFilePaths, assemblyPath, site);
        }

        private void CopyToLoaderSourceDirectory(List<string> loaderSourceTestFilePaths, string assemblyPath, string site)
        {
            string dllFileName = Path.GetFileNameWithoutExtension(assemblyPath);
            foreach (string loaderSourceTestFilePath in loaderSourceTestFilePaths)
            {
                string loaderSourceTestFileDestinationPath = Regex.Replace(loaderSourceTestFilePath, $@"\b{dllFileName}\b", $"PDS.SpaceBE.{site}.PADS.Module.Tests");

                string pattern = $@"\b{ExpectedFolderName}\b";
                string replacement = SourceFolderName;
                loaderSourceTestFileDestinationPath = Regex.Replace(loaderSourceTestFileDestinationPath, $"^(.*?){pattern}(?!.*?{pattern})", $"$1{replacement}");
                string directoryPath = Path.GetDirectoryName(loaderSourceTestFileDestinationPath);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                File.Copy(loaderSourceTestFilePath, loaderSourceTestFileDestinationPath, true);
            }
        }

        private List<string> GetLoaderSourceTestFilePaths(string assemblyPath)
        {
            var loaderSourceTestFilePaths = new List<string>();
            var testCases = GetTestDataByAssembly(assemblyPath).ToList();
            foreach (object[] testCase in testCases)
            {
                string expectedFilePath = testCase[1].ToString();
                loaderSourceTestFilePaths.Add(expectedFilePath);
            }

            return loaderSourceTestFilePaths;
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
                    yield return new object[] { sourceFilePath, expectedFilePath };
                }
            }
        }
    }
}
