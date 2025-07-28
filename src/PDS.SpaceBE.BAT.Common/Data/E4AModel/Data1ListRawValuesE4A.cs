using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.BAT.Common.Data.E4AModel
{
    public class Data1ListRawValuesE4A : BaseData1ListRawValuesE4A
    {
        [BsonIgnoreIfNull]
        public string ProcessTool { get; set; }
    }
}
