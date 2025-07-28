using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using System.Linq;
using PDS.Core.Api.Config;
using PDS.Core.Api.Utils;
using PDS.Database.Api.Module;
using PDS.SpaceBE.CJJ.Source.Module.Data.SpaceModel;
using PDS.Common.Config;
using PDS.SpaceBE.CJJ.Common.Config;

namespace PDS.SpaceBE.CJJ.Source.Module.Data
{
    [ExcludeFromCodeCoverage]
    public class SpaceDao
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly string _connectionString;

        public SpaceDao([NotNull] IConfigManager configManager,
            [NotNull] IDatabaseManager databaseManager)
        {
            Ensure.NotNull(configManager, nameof(configManager));
            _databaseManager = Ensure.NotNull(databaseManager, nameof(databaseManager));

            //Get space database connection string
            var spaceSourceConfig = configManager.GetAppScope(SpaceConfigs.AppName, ConfigScopes.Source);
            _connectionString = spaceSourceConfig.GetValue<string>(ConfigItems.ConnectionString);
        }

        public IEnumerable<SpaceEntry> GetSpaceDatabaseEntries(DateTime startTime, DateTime endTime)
        {
            using (var conn = _databaseManager.OpenConnection(_connectionString))
            {
                var spaceDictionary = new Dictionary<string, SpaceEntry>();
                var parameters = new { StartTime = startTime, EndTime = endTime };
                return conn.Query<SpaceEntry, SpaceRawValuesEntry, SpaceEntry>(Properties.Resources.SpaceSqlQuery, (main, rawvalues) =>
                {
                    SpaceEntry spaceEntry;
                    if (!spaceDictionary.TryGetValue(main.PKey, out spaceEntry))
                    {
                        spaceEntry = main;
                        spaceEntry.SpaceRawValues = new List<SpaceRawValuesEntry>();
                        spaceDictionary.Add(spaceEntry.PKey, spaceEntry);
                    }
                    spaceEntry.SpaceRawValues.Add(rawvalues);
                    return spaceEntry;
                }, parameters, splitOn: "BreakPoint").Distinct().ToList();
            }
        }
    }
}
