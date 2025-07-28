using System.Diagnostics.CodeAnalysis;

namespace PDS.SpaceNew.Common.Config
{
    /// <summary>
    /// Space specific configuration constants.
    /// </summary>
    ///

    [ExcludeFromCodeCoverage]
    public static class SpaceConfigs
    {
        /// <summary>
        /// The local datetime format of the space application.
        /// </summary>
        public const string LocalDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// The utc datetime format of the space application.
        /// </summary>
        public const string UTCDateTimeFormat = "yyyy-MM-dd HH:mm:ssZ";
    }
}
