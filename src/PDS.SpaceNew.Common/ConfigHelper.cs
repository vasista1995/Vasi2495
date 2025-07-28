using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDS.Space.Common.Data;

namespace PDS.SpaceNew.Common
{
    public class ConfigHelper
    {
        public static string GetSpaceInstanceName(string appName)
        {
            if (appName == null || !appName.StartsWith("Space", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Unknown {nameof(appName)} with value {appName} received: Only app names starting with \"Space\" are currently allowed!", nameof(appName));

            return appName.Substring(appName.Length - 2);
        }

        public static string GetSpaceAppName()
        {
            string appName = Environment.GetEnvironmentVariable(SpaceConfigVariables.AppName);
            if (appName == null)
                throw new ArgumentNullException(nameof(appName), $"Invalid configuration: EnvironmentVariable for {SpaceConfigVariables.AppName} was not set!");

            return appName;
        }
    }
}
