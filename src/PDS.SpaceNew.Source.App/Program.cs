using System;
using System.Diagnostics.CodeAnalysis;
using PDS.Core.Api;
using PDS.Space.Common.Data;

namespace PDS.SpaceNew.Source.App
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
            Environment.SetEnvironmentVariable(EnvironmentVariables.SiteKey, "CEG");
            Environment.SetEnvironmentVariable(EnvironmentVariables.Environment, "DEV");
            Environment.SetEnvironmentVariable(EnvironmentVariables.HttpPort, "5001");
            Environment.SetEnvironmentVariable(SpaceConfigVariables.AppName, "SpaceBE");
#endif
            return PDS.Base.App.Program.Main(args);
        }
    }
}
