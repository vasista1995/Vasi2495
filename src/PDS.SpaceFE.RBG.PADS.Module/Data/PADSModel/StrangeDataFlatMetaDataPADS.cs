using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel
{
    public class StrangeDataFlatMetaDataPads : BaseStrangeDataFlatMetaDataPads
    { 

        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        /// 
        
        [BsonIgnoreIfNull]
        public string Design { get; set; }
        [BsonIgnoreIfNull]
        public string Layer { get; set; }
        [BsonIgnoreIfNull]
        public string MeasurementBatch { get; set; }        
        [BsonIgnoreIfNull]
        public string ProcessGroup { get; set; }       
        [BsonIgnoreIfNull]
        public string ProcessLine { get; set; }
        
    }
}
