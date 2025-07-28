using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PDS.Core.Api.Config;
using PDS.Core.Api.Utils;
using PDS.Common.Config;
using PDS.SpaceBE.CEG.Common.Config;
using MongoDB.Driver;
using PDS.SpaceBE.CEG.PADS.Module.Data.PADSModel;
using MongoDB.Bson;
using PDS.Space.Common.Bson;
using PDS.MongoDB.Api;
using PDS.SpaceBE.CEG.Common.Data.E4AModel;

namespace PDS.SpaceBE.CEG.PADS.Module.Data
{
    [ExcludeFromCodeCoverage]
    public class PadsDao : IPadsDao
    {
        private readonly IMongoCollection<SpacePads> _padsCollection;
        private readonly IMongoCollection<SpaceE4A> _padsSpaceE4aCollection;

        public PadsDao([NotNull] IConfigManager configManager, [NotNull] IMongoClientFactory mongoClientFactory)
        {
            Ensure.NotNull(configManager, nameof(configManager));
            Ensure.NotNull(mongoClientFactory, nameof(mongoClientFactory));

            var dbConfig = configManager.GetAppScope(SpaceConfigs.AppName, ConfigScopes.Target);
            string connectionString = dbConfig.GetValue<string>(ConfigItems.ConnectionString);
            string databaseName = dbConfig.GetValue<string>(ConfigItems.Database);
            string collectionName = dbConfig.GetValue<string>(ConfigItems.Collection);
            string rVCollectionName = dbConfig.GetValue<string>(ConfigItems.RVCollection);
            var mongoDBClient = mongoClientFactory.CreateClient(connectionString);
            var padsDatabase = mongoDBClient.GetDatabase(databaseName);
            _padsCollection = padsDatabase.GetCollection<SpacePads>(collectionName);
            _padsSpaceE4aCollection = padsDatabase.GetCollection<SpaceE4A>(rVCollectionName);
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
        public SpaceE4A FindExistingE4ADoc(string id)
        {
            var filter = Builders<SpaceE4A>.Filter.Eq(x => x.IdSource, id);
            return _padsSpaceE4aCollection.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// To Insert the E4A document into the PADS DB
        /// </summary>
        /// <param name="document"></param>
        public void InsertE4ADoc(SpaceE4A document)
        {
            _padsSpaceE4aCollection.InsertOne(document);
        }

        /// <summary>
        /// To Update the E4A document in PADS DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="document"></param>
        public void UpdateE4ADoc(string id, SpaceE4A document)
        {
            var filter = Builders<SpaceE4A>.Filter.Eq(x => x.IdSource, id);
            _padsSpaceE4aCollection.ReplaceOne(
                        filter: filter,
                        options: new ReplaceOptions { IsUpsert = true },
                        replacement: document);
        }
    }
}
