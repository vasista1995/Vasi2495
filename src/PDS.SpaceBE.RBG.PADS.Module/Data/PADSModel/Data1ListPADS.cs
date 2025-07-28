using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.RBG.PADS.Module.Data.PADSModel
{
    public class Data1ListPads : BaseData1ListPads
    {
        [JsonProperty(SpacePadsProperties.Data1ListRawValues)]
        [BsonElement(SpacePadsProperties.Data1ListRawValues)]
        [BsonIgnoreIfNull]
        public List<Data1ListRawValuesPads> Data1ListRawValues { get; set; }

    }
}
