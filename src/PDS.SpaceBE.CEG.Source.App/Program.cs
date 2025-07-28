using System;
using PDS.Core.Api;

namespace PDS.SpaceBE.CEG.Source.App
{
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
            Environment.SetEnvironmentVariable(EnvironmentVariables.HttpPort, "5000");
#endif
            return PDS.Base.App.Program.Main(args);
        }
    }
}
