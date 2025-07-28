using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to Data1list section in pads document.
    /// </summary>
    ///
    public class Data1ListPads : BaseData1ListPads
    {
        
        [JsonProperty(SpacePadsProperties.ProcessEquipment)]
        [BsonElement(SpacePadsProperties.ProcessEquipment)]
        [BsonIgnoreIfNull]
        public string ProcessEquipment { get; set; }

        [JsonProperty(SpacePadsProperties.ProcessBatch)]
        [BsonElement(SpacePadsProperties.ProcessBatch)]
        [BsonIgnoreIfNull]
        public string ProcessBatch { get; set; }

        [JsonProperty(SpacePadsProperties.Data1ListRawValues)]
        [BsonElement(SpacePadsProperties.Data1ListRawValues)]
        [BsonIgnoreIfNull]
        public List<Data1ListRawValuesPads> Data1ListRawValues { get; set; }
    }
}
