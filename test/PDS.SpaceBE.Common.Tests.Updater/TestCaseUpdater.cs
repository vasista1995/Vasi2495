using PDS.SpaceBE.Common.PADS.Module.Tests;
using PDS.SpaceBE.Common.Source.Module.Tests;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PDS.SpaceBE.Common.Tests.Updater
{
    public class TestCaseUpdater
    {
        internal static void PrepareLoaderInput(string site)
        {
            string moduleType = "Source";
            string assemblyPath = GetAssemblyPath(site, moduleType);
            var regressionTestUpdate = RegressionTestUpdateFactory.Create<IRegressionTestSourceUpdater>(assemblyPath);
            regressionTestUpdate.PrepareLoaderInput(assemblyPath, site);
        }

        internal static void UpdateLoaderRegressionTests(string site)
        {
            string moduleType = "PADS";
            string assemblyPath = GetAssemblyPath(site, moduleType);
            var regressionTestUpdate = RegressionTestUpdateFactory.Create<IRegressionTestPADSUpdater>(assemblyPath);
            regressionTestUpdate.UpdatePADSRegressionTests(assemblyPath);
        }

        internal static void UpdateSourceRegressionTests(string site)
        {
            string moduleType = "Source";
            string assemblyPath = GetAssemblyPath(site, moduleType);
            var regressionTestUpdate = RegressionTestUpdateFactory.Create<IRegressionTestSourceUpdater>(assemblyPath);
            regressionTestUpdate.UpdateSourceRegressionTests(assemblyPath);
        }

        private static string GetAssemblyPath(string site, string moduleType)
        {
            string assemblyPath = $"../../../../PDS.SpaceBE.{site}.{moduleType}.Module.Tests/bin/Debug/net8.0/PDS.SpaceBE.{site}.{moduleType}.Module.Tests.dll";
            return assemblyPath;
        }
    }

    public class RegressionTestUpdateFactory
    {
        public static T Create<T>(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException($"Dll with path {assemblyPath} does not exist!");
            }

            var assembly = Assembly.LoadFrom(assemblyPath);
            var type = assembly.GetTypes().FirstOrDefault(t => typeof(T).IsAssignableFrom(t));
            if (type == null)
            {
                throw new ArgumentException($"No type which implements {typeof(T)} was found for assembly {assemblyPath}");
            }

            return (T) Activator.CreateInstance(type);
        }
    }
}
