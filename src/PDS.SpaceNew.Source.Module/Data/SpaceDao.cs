using System;
using System.Diagnostics.CodeAnalysis;
using PDS.Core.Api.Config;
using PDS.Core.Api.Utils;
using PDS.Database.Api.Module;
using PDS.Common.Config;
using System.Data;
using PDS.Core.Api;
using System.IO;
using PDS.SpaceNew.Common.Config;
using Dapper;
using System.Collections.Generic;
using PDS.Space.Common.Data;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.Common.Data.Config;

namespace PDS.SpaceNew.Source.Module.Data
{
    [ExcludeFromCodeCoverage]
    public class SpaceDao
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;
        private readonly IDatabaseManager _databaseManager;
        private readonly string _viewName;

        public SpaceDao([NotNull] IConfigManager configManager,
            [NotNull] IDatabaseManager databaseManager)
        {
            Ensure.NotNull(configManager, nameof(configManager));
            _databaseManager = Ensure.NotNull(databaseManager, nameof(databaseManager));

            string appName = ConfigHelper.GetSpaceAppName();

            //Get space database connection string
            var spaceSourceConfig = configManager.GetAppScope(appName, ConfigScopes.Source);
            _connectionString = spaceSourceConfig.GetValue<string>(ConfigItems.ConnectionString);
            //_commandTimeout = spaceSourceConfig.GetValueOrDefault("SqlCommandTimeout", 300);, commandTimeout: _commandTimeout
            _viewName = spaceSourceConfig.GetValue<string>(SpaceConfigVariables.ViewName);
        }

        public IEnumerable<IDictionary<string, object>> GetSpaceData(DateTime startTime, DateTime endTime, int ldsId)
        {
            string sqlQuery = Properties.Resources.SpaceSqlQuery;
            sqlQuery = sqlQuery.Replace("#ViewName#", _viewName);

            // Add the WHERE clause from a site configuration
            string appName = ConfigHelper.GetSpaceAppName();
            string spaceInstanceName = ConfigHelper.GetSpaceInstanceName(appName);
            string siteKey = System.Environment.GetEnvironmentVariable(EnvironmentVariables.SiteKey);
            string whereClause = File.ReadAllText(Path.Combine("Resources", spaceInstanceName, siteKey, "SqlWhereClause.sql"));
            sqlQuery += whereClause;

            using (var connection = _databaseManager.OpenConnection(_connectionString))
            {
                var parameters = new
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    LdsId = ldsId
                };

                var results = connection.Query(sqlQuery, parameters);
                var spaceDatabaseEntries = new List<IDictionary<string, object>>();

                foreach (var result in results)
                {
                    var databaseEntry = (IDictionary<string, object>) result;
                    var caseInsensitiveDatabaseEntry = new Dictionary<string, object>(databaseEntry, StringComparer.InvariantCultureIgnoreCase) { };
                    spaceDatabaseEntries.Add(caseInsensitiveDatabaseEntry);
                }

                return spaceDatabaseEntries;
            }
        }
    }
}
