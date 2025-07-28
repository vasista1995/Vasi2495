using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.CEG.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to Rawvalues array subsection of data1list section in pads document.
    /// </summary>
    public class Data1ListRawValuesPads : BaseData1ListRawValuesPads, IData1ListRawValuesPadsGof
    {
        [JsonProperty(SpacePadsProperties.ProcessEquipment)]
        [BsonElement(SpacePadsProperties.ProcessEquipment)]
        [BsonIgnoreIfNull]
        public string ProcessEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string MeasurementEquipment { get; set; }
        [BsonIgnoreIfNull]
        public string MeasurementEquipmentName { get; set; }
        [BsonIgnoreIfNull]
        public string ProcessEquipmentName { get; set; }
    }
}
