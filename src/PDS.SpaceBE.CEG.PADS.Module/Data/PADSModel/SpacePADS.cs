using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PDS.Common.E4AModel;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.CEG.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class provides the main properties that must be present in a space mongo document.
    /// The properties for each subclass will be followed in different classes.
    /// </summary>
    public class SpacePads : BaseSpacePads
    {
        [BsonElement(E4aProperties.DataFlatMetaData)]
        [JsonProperty(E4aProperties.DataFlatMetaData)]
        public DataFlatMetaDataPads DataFlatMetaData { get; set; }

        [BsonElement(SpacePadsProperties.DataFlatStrangeMetadata)]
        [JsonProperty(SpacePadsProperties.DataFlatStrangeMetadata)]
        public StrangeDataFlatMetaDataPads StrangeDataFlatMetaData { get; set; }

        [JsonProperty(E4aProperties.Data1ListParameters)]
        [BsonElement(E4aProperties.Data1ListParameters)]
        [BsonIgnoreIfNull]
        public List<Data1ListPads> Data1List { get; set; }
        
    }
}
