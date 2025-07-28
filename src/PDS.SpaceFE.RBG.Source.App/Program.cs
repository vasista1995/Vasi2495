using System;
using System.Diagnostics.CodeAnalysis;
using PDS.Core.Api;

namespace PDS.SpaceFE.RBG.Source.App
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
            Environment.SetEnvironmentVariable(EnvironmentVariables.SiteKey, "RBG");
            Environment.SetEnvironmentVariable(EnvironmentVariables.Environment, "DEV");
#endif
            return PDS.Base.App.Program.Main(args);
        }
    }
}
