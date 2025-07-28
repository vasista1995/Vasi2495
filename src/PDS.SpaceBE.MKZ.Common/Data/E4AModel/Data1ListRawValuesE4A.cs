using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.MKZ.Common.Data.E4AModel
{
    public class Data1ListRawValuesE4A : BaseData1ListRawValuesE4A
    {
        [BsonIgnoreIfNull]
        public string Tool { get; set; }
        [BsonIgnoreIfNull]
        public string Machine { get; set; }
    }
}
