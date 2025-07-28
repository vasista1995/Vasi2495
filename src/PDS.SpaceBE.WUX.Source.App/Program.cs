using System;
using System.Diagnostics.CodeAnalysis;
using PDS.Core.Api;
using PDS.Space.Common.Data;

namespace PDS.SpaceBE.WUX.Source.App
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        /// <summary>
        /// Main method of the application
        /// </summary>
        public static int Main(string[] args)
        {
#if DEBUG
            Environment.SetEnvironmentVariable(EnvironmentVariables.SiteKey, "WUX");
            Environment.SetEnvironmentVariable(EnvironmentVariables.Environment, "DEV");
            Environment.SetEnvironmentVariable(EnvironmentVariables.HttpPort, "5000");
            Environment.SetEnvironmentVariable(SpaceConfigVariables.CreateRegressionTests, "True");
#endif
            return PDS.Base.App.Program.Main(args);
        }
    }
}
