using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.BAT.PADS.Module.Data.PADSModel
{
    public class DataFlatMetaDataPads : BaseDataFlatMetaDataPads
    {

        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
        /// </summary>
        ///
        [BsonIgnoreIfNull]
        public string Shift { get; set; }
        [BsonIgnoreIfNull]
        public string Process { get; set; }
    }
}
