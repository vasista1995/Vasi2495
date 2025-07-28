using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PDS.Space.Common.Data.PADSModel
{
    public class RepetitionPads
    {
        [BsonElement(SpacePadsProperties.Id)]
        public string Id { get; set; }

        public DateTime IdBaseValue { get; set; }
    }
}
