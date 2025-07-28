using System;
using System.Diagnostics.CodeAnalysis;
using PDS.Core.Api;

namespace PDS.SpaceBE.CJJ.PADS.App
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
            Environment.SetEnvironmentVariable(EnvironmentVariables.SiteKey, "CJJ");
            Environment.SetEnvironmentVariable(EnvironmentVariables.Environment, "DEV");
            Environment.SetEnvironmentVariable(EnvironmentVariables.HttpPort, "5001");
#endif
            return PDS.Base.App.Program.Main(args);
        }
    }
}
