using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.SpaceBE.RBG.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to Rawvalues array subsection of data1list section in pads document.
    /// </summary>
    public class Data1ListRawValuesPads : BaseData1ListRawValuesPads
    {
        [BsonIgnoreIfNull]
        public string WaferLot { get; set; }
    }
}
