using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PDS.Core.Api.Config;
using PDS.Core.Api.Utils;
using PDS.Common.Config;
using PDS.SpaceBE.WAR.Common.Config;
using MongoDB.Driver;
using PDS.SpaceBE.WAR.PADS.Module.Data.PADSModel;
using MongoDB.Bson;
using PDS.Space.Common.Bson;
using PDS.MongoDB.Api;

namespace PDS.SpaceBE.WAR.PADS.Module.Data
{
    [ExcludeFromCodeCoverage]
    public class PadsDao : IPadsDao
    {
        private readonly IMongoCollection<SpacePads> _padsCollection;

        public PadsDao([NotNull] IConfigManager configManager, [NotNull] IMongoClientFactory mongoClientFactory)
        {
            Ensure.NotNull(configManager, nameof(configManager));
            Ensure.NotNull(mongoClientFactory, nameof(mongoClientFactory));

            var dbConfig = configManager.GetAppScope(SpaceConfigs.AppName, ConfigScopes.Target);
            string connectionString = dbConfig.GetValue<string>(ConfigItems.ConnectionString);
            string databaseName = dbConfig.GetValue<string>(ConfigItems.Database);
            string collectionName = dbConfig.GetValue<string>(ConfigItems.Collection);
            var mongoDBClient = mongoClientFactory.CreateClient(connectionString);
            var padsDatabase = mongoDBClient.GetDatabase(databaseName);
            _padsCollection = padsDatabase.GetCollection<SpacePads>(collectionName);
        }

        public SpacePads FindExistingDoc(string site, string timeGroup, string id)
        {
            var filter = Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.SiteKey, site)
                & Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.TimeGroup, timeGroup)
                & Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.SpaceKey, id);
            return _padsCollection.Find(filter).FirstOrDefault();
        }


        public void InsertDoc(SpacePads document)
        {
            _padsCollection.InsertOne(document);
        }

        public void UpdateDoc(string site, string timeGroup, string id, SpacePads document)
        {
            var filter = Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.SiteKey, site)
               & Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.TimeGroup, timeGroup)
               & Builders<SpacePads>.Filter.Eq(x => x.Id, id);
            _padsCollection.ReplaceOne(
                        filter: filter,
                        options: new ReplaceOptions { IsUpsert = true },
                        replacement: document);

        }
    }
}
