using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.SpaceBE.WUX.Source.Module.Data.SpaceModel
{
    public class SpaceRawValuesEntry : BaseSpaceRawValuesEntry
    {
        [BsonIgnoreIfNull]
        public string Tool { get; set; }
        [BsonIgnoreIfNull]
        public string Wafer { get; set; }
    }
}
