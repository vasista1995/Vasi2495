using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PDS.Core.Api.Config;
using PDS.Core.Api.Utils;
using PDS.Common.Config;
using MongoDB.Driver;
using PDS.SpaceNew.PADS.Module.Data.PADSModel;
using MongoDB.Bson;
using PDS.MongoDB.Api;
using PDS.SpaceNew.Common.Config;
using PDS.Common.Bson;
using System.Collections.Generic;
using PDS.SpaceNew.Common;
using MongoDB.Bson.Serialization.Conventions;
using System.Reflection;
using PDS.Space.Common.Data;
using System;
using System.Text.RegularExpressions;

namespace PDS.SpaceNew.PADS.Module.Data
{
    [ExcludeFromCodeCoverage]
    public class PadsDao : IPadsDao
    {
        private readonly IMongoCollection<SpacePads> _padsCollection;
        private readonly IMongoCollection<BsonDocument> _padsCollection4Waf;
        private readonly IMongoCollection<SpaceE4A> _padsSpaceE4aCollection;

        public PadsDao([NotNull] IConfigManager configManager, [NotNull] IMongoClientFactory mongoClientFactory)
        {
            Ensure.NotNull(configManager, nameof(configManager));
            Ensure.NotNull(mongoClientFactory, nameof(mongoClientFactory));

            string appName = ConfigHelper.GetSpaceAppName();
            var dbConfig = configManager.GetAppScope(appName, ConfigScopes.Target);
            string connectionString = dbConfig.GetValue<string>(ConfigItems.ConnectionString);
            string databaseName = dbConfig.GetValue<string>(ConfigItems.Database);
            string collectionName = dbConfig.GetValue<string>(ConfigItems.Collection);
            string rVCollectionName = dbConfig.GetValue<string>(ConfigItems.RVCollection);
            var mongoDBClient = mongoClientFactory.CreateClient(connectionString);
            var padsDatabase = mongoDBClient.GetDatabase(databaseName);

            // Register the convention pack
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

            _padsCollection = padsDatabase.GetCollection<SpacePads>(collectionName);
            _padsCollection4Waf = padsDatabase.GetCollection<BsonDocument>(collectionName);
            _padsSpaceE4aCollection = padsDatabase.GetCollection<SpaceE4A>(rVCollectionName);
        }

        /// <summary>
        /// To Find all existing rawvalues for a given wafer to create wafer aggregates.
        /// </summary>
        /// <param name="measLot"></param>
        /// <param name="motherLotWafer"></param>
        /// <param name="productionActionId"></param>
        /// <returns></returns>
        public List<Data1ListRawValuesPads4Wafer> FindExistingWafDoc(string measLot, string motherLotWafer, string productionActionId)
        {
            string regexlot = "^" + measLot + ".*";
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match",
                    new BsonDocument
                            {
                                { "Data_flat_Metadata.MeasLot",
                        new Regex(regexlot) },
                                { "ProductionAction._id", productionActionId }
                            }),
                        new BsonDocument("$unwind",
                        new BsonDocument("path", "$Data_1list_Parameters")),
                        new BsonDocument("$unwind",
                        new BsonDocument
                            {
                                { "path", "$Data_1list_Parameters.Data_1list_RawValues" },
                                { "includeArrayIndex", "index" }
                            }),
                        new BsonDocument("$match",
                        new BsonDocument("Data_1list_Parameters.Data_1list_RawValues.MotherlotWafer", motherLotWafer)),
                        new BsonDocument("$project",
                        new BsonDocument
                            {
                                { "_id", 0 },
                                { "Data_1list_Parameters", 1 },
                                { "rawvalues", "$Data_1list_Parameters.Data_1list_RawValues" }
                            }),
                        new BsonDocument("$unset",
                        new BsonArray
                            {
                                "Data_1list_Parameters.Data_1list_RawValues",
                                "Data_1list_Parameters.Data_flat_Limits",
                                "Data_1list_Parameters.Data_flat_MeasurementAggregates"
                            }),
                        new BsonDocument("$project",
                        new BsonDocument
                            {
                                { "_id", 0 },
                                { "parameters", "$Data_1list_Parameters" },
                                { "rawvalues", 1 }
                            }),
                        new BsonDocument("$replaceRoot",
                        new BsonDocument("newRoot",
                        new BsonDocument("$mergeObjects",
                        new BsonArray
                                    {
                                        "$$ROOT.parameters",
                                        "$$ROOT.rawvalues"
                                    })))
            };

            return _padsCollection4Waf.Aggregate<Data1ListRawValuesPads4Wafer>(pipeline).ToList();
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
