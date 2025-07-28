using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PDS.Common.E4AModel;
using PDS.Space.Common.Data.PADSModel;
using PDS.SpaceNew.Common;

namespace PDS.SpaceNew.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class provides the main properties that must be present in a space mongo document.
    /// The properties for each subclass will be followed in different classes.
    /// </summary>
    public class SpacePads : BaseSpacePads
    {
        [BsonElement(E4aProperties.DataFlatMetaData)]
        [JsonProperty(E4aProperties.DataFlatMetaData)]
        public IDictionary<string, object> DataFlatMetaData { get; set; }

        [BsonElement(SpacePadsProperties.DataFlatStrangeMetadata)]
        [JsonProperty(SpacePadsProperties.DataFlatStrangeMetadata)]
        public IDictionary<string, object> StrangeDataFlatMetaData { get; set; }

        [BsonElement(SpacePadsProperties.Data1ListParameters)]
        [JsonProperty(SpacePadsProperties.Data1ListParameters)]
        public List<Data1ListPads> Data1ListParameters { get; set; }
    }
}
