using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.CJJ.Common.Data.E4AModel
{
    public class Data1ListRawValuesE4A : BaseData1ListRawValuesE4A
    {
        [BsonIgnoreIfNull]
        public string Machine { get; set; }
    }
}
