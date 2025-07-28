using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PDS.Core.Api.Config;
using PDS.Core.Api.Utils;
using PDS.Common.Config;
using PDS.SpaceFE.RBG.Common.Config;
using MongoDB.Driver;
using PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel;
using PDS.Common.Bson;
using MongoDB.Bson;
using System.Collections.Generic;
using PDS.Space.Common.Bson;
using PDS.MongoDB.Api;

namespace PDS.SpaceFE.RBG.PADS.Module.Data
{
    [ExcludeFromCodeCoverage]
    public class PadsDao : IPadsDao
    {
        private readonly IMongoCollection<SpacePads> _padsCollection;
        private readonly IMongoCollection<BsonDocument> _padsCollection4Waf;

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
            _padsCollection4Waf = padsDatabase.GetCollection<BsonDocument>(collectionName);
        }

        public SpacePads FindExistingDoc(string site, string timeGroup, string id)
        {
            var filter1 = Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.SiteKey, site)
                & Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.TimeGroup, timeGroup)
                & Builders<SpacePads>.Filter.Eq(x => x.SearchPatterns.SpaceKey, id);

            return _padsCollection.Find(filter1).FirstOrDefault();
        }

        public List<Data1ListRawValuesPads4Wafer> FindExistingWafDoc(string lot,string id, string paId)
        {
            string regexlot = "^" + lot + ".*";
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match",
                new BsonDocument
                    {
                        { "ProductionAction._id",paId},
                        { "Data_flat_Metadata.MeasLot",
                new BsonDocument("$regex", regexlot) }

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
                new BsonDocument{
                    { "Data_1list_Parameters.Data_1list_RawValues.ItemID:MotherlotWafer", id }
                }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { BsonFields.Id, 0 },
                        { nameof(Data1ListRawValuesPads4Wafer.ParameterName), "$Data_1list_Parameters.ParameterName" },
                        { nameof(Data1ListRawValuesPads4Wafer.ChannelId), "$Data_1list_Parameters.ChannelId" },
                        { nameof(Data1ListRawValuesPads4Wafer.ParameterUnit), "$Data_1list_Parameters.ParameterUnit" },
                        { nameof(Data1ListRawValuesPads4Wafer.ItemIdMotherlotWafer), "$Data_1list_Parameters.Data_1list_RawValues.ItemID:MotherlotWafer" },
                        { nameof(Data1ListRawValuesPads4Wafer.ProcessEquipment), "$Data_1list_Parameters.Data_1list_RawValues.process_equipment" },
                        { nameof(Data1ListRawValuesPads4Wafer.Seqnr), "$Data_1list_Parameters.Data_1list_RawValues.Seqnr" },
                        { nameof(Data1ListRawValuesPads4Wafer.SampleId), "$Data_1list_Parameters.Data_1list_RawValues.SampleId" },
                        { nameof(Data1ListRawValuesPads4Wafer.Value), "$Data_1list_Parameters.Data_1list_RawValues.Value" },
                        { nameof(Data1ListRawValuesPads4Wafer.IsFlagged), "$Data_1list_Parameters.Data_1list_RawValues.is_flagged" },
                        { nameof(Data1ListRawValuesPads4Wafer.NumViolations), "$Data_1list_Parameters.Data_1list_RawValues.NumViolations" },
                        { nameof(Data1ListRawValuesPads4Wafer.PrimaryViolation),"$Data_1list_Parameters.Data_1list_RawValues.PrimaryViolation"},
                        { nameof(Data1ListRawValuesPads4Wafer.PrimaryViolationComments),"$Data_1list_Parameters.Data_1list_RawValues.PrimaryViolationComments"},
                        { nameof(Data1ListRawValuesPads4Wafer.ViolationComments),"$Data_1list_Parameters.Data_1list_RawValues.ViolationComments"},
                        { nameof(Data1ListRawValuesPads4Wafer.ViolationList),"$Data_1list_Parameters.Data_1list_RawValues.ViolationList"},
                        { nameof(Data1ListRawValuesPads4Wafer.SampleTimestamp), "$Data_1list_Parameters.Data_1list_RawValues.SampleTimestamp"},
                        { nameof(Data1ListRawValuesPads4Wafer.SampleTimestampUtc), "$Data_1list_Parameters.Data_1list_RawValues.SampleTimestampUtc"},
                        { nameof(Data1ListRawValuesPads4Wafer.SampleMax),"$Data_1list_Parameters.Data_1list_RawValues.SampleMax"},
                        { nameof(Data1ListRawValuesPads4Wafer.SampleMin),"$Data_1list_Parameters.Data_1list_RawValues.SampleMin"},
                        { nameof(Data1ListRawValuesPads4Wafer.SampleMean),"$Data_1list_Parameters.Data_1list_RawValues.SampleMean"},
                        { nameof(Data1ListRawValuesPads4Wafer.SampleSize),"$Data_1list_Parameters.Data_1list_RawValues.SampleSize"},
                        { nameof(Data1ListRawValuesPads4Wafer.GOF),"$Data_1list_Parameters.Data_1list_RawValues.GOF"},
                        { nameof(Data1ListRawValuesPads4Wafer.CreatedTimestamp),"$Data_1list_Parameters.Data_1list_RawValues.CreatedTimestamp"},
                        { nameof(Data1ListRawValuesPads4Wafer.CreatedTimestampUtc),"$Data_1list_Parameters.Data_1list_RawValues.CreatedTimestampUtc"},
                        { nameof(Data1ListRawValuesPads4Wafer.UpdatedTimestamp),"$Data_1list_Parameters.Data_1list_RawValues.UpdatedTimestamp"},
                        { nameof(Data1ListRawValuesPads4Wafer.UpdatedTimestampUtc),"$Data_1list_Parameters.Data_1list_RawValues.UpdatedTimestampUtc"},
                        { nameof(Data1ListRawValuesPads4Wafer.TestPosition),"$Data_1list_Parameters.Data_1list_RawValues.TestPosition"},
                        { nameof(Data1ListRawValuesPads4Wafer.WaferSequence),"$Data_1list_Parameters.Data_1list_RawValues.WaferSequence"},
                        { nameof(Data1ListRawValuesPads4Wafer.Slot),"$Data_1list_Parameters.Data_1list_RawValues.Slot"},
                        { nameof(Data1ListRawValuesPads4Wafer.InternalComment),"$Data_1list_Parameters.Data_1list_RawValues.InternalComment"}
                    })
            };
            return _padsCollection4Waf.Aggregate<Data1ListRawValuesPads4Wafer>(pipeline).ToList();
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
