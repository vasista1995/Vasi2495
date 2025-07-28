using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PDS.Core.Api.Config;
using PDS.Core.Api.Utils;
using PDS.Common.Config;
using PDS.SpaceBE.TIJ.Common.Config;
using MongoDB.Driver;
using PDS.SpaceBE.TIJ.PADS.Module.Data.PADSModel;
using PDS.Common.Bson;
using MongoDB.Bson;
using System.Collections.Generic;
using PDS.MongoDB.Api;
using PDS.SpaceBE.TIJ.Common.Data.E4AModel;

namespace PDS.SpaceBE.TIJ.PADS.Module.Data
{
    /// <summary>
    /// This class provides all the configuration to send the data and recieve it from MongoDB
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PadsDao : IPadsDao
    {
        private readonly IMongoCollection<SpacePads> _padsCollection;
        private readonly IMongoCollection<BsonDocument> _padsCollection4Waf;
        private readonly IMongoCollection<SpaceE4A> _padsSpaceE4aCollection;

        /// <summary>
        /// All the config details of the PADS DB
        /// </summary>
        /// <param name="configManager"></param>
        /// <param name="mongoClientFactory"></param>
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
            _padsCollection4Waf = padsDatabase.GetCollection<BsonDocument>(collectionName);
            _padsSpaceE4aCollection = padsDatabase.GetCollection<SpaceE4A>(rVCollectionName);
        }

        /// <summary>
        /// Find and retrive existing document from PADS DB to update it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SpacePads FindExistingDoc(string site, string timeGroup, string id)
        {
            var filter = Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.SiteKey, site)
                & Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.TimeGroup, timeGroup)
                & Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.SpaceKey, id);
            return _padsCollection.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// Ti Insert the document into the PADS DB
        /// </summary>
        /// <param name="document"></param>
        public void InsertDoc(SpacePads document)
        {
            _padsCollection.InsertOne(document);
        }

        /// <summary>
        /// To Update the document in PADS DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="document"></param>
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

        /// <summary>
        /// Find and retrive existing E4A document from PADS DB to update it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
