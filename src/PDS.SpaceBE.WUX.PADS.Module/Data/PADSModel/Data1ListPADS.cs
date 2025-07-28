using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.WUX.PADS.Module.Data.PADSModel
{
    public class Data1ListPads : BaseData1ListPads
    {
        [JsonProperty(SpacePadsProperties.Data1ListRawValues)]
        [BsonElement(SpacePadsProperties.Data1ListRawValues)]
        [BsonIgnoreIfNull]
        public List<Data1ListRawValuesPads> Data1ListRawValues { get; set; }
        [JsonProperty(SpacePadsProperties.ProcessTool)]
        [BsonElement(SpacePadsProperties.ProcessTool)]
        [BsonIgnoreIfNull]
        public string ProcessTool { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterClass { get; set; }
        [BsonIgnoreIfNull]
        public string TargetCpk { get; set; }
        [BsonIgnoreIfNull]
        public string Segment { get; set; }
        [BsonIgnoreIfNull]
        public string Module { get; set; }
        [BsonIgnoreIfNull]
        public string F56Parameter { get; set; }
        [BsonIgnoreIfNull]
        public string FourDReport { get; set; }
        [BsonIgnoreIfNull]
        public string SpecialCharacteristics { get; set; }
        [BsonIgnoreIfNull]
        public string CFComment { get; set; }
    }
}
