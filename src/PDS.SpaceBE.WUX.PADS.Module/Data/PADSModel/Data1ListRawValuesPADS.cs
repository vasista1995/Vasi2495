using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.WUX.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to Rawvalues array subsection of data1list section in pads document.
    /// </summary>
    public class Data1ListRawValuesPads : BaseData1ListRawValuesPads
    {
        [BsonIgnoreIfNull]
        [JsonProperty(SpacePadsProperties.ProcessTool)]
        [BsonElement(SpacePadsProperties.ProcessTool)]
        public string ProcessTool { get; internal set; }
        [BsonIgnoreIfNull]
        public string Wafer { get; internal set; }
    }
}
