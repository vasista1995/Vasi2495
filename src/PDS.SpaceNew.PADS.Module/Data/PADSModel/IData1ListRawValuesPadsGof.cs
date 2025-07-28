using MongoDB.Bson.Serialization.Attributes;

namespace PDS.SpaceNew.PADS.Module.Data.PADSModel
{
    public interface IData1ListRawValuesPadsGof
    {
        [BsonIgnoreIfNull]
        string GOF { get; set; }
    }
}
