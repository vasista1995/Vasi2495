using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;
using System.Text;

namespace PDS.SpaceBE.MKZ.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
    /// </summary>
    ///
    public class DataFlatMetaDataPads : BaseDataFlatMetaDataPads
    {
        [BsonIgnoreIfNull]
        public string PadSize { get; set; }
        [BsonIgnoreIfNull]
        public string Wire { get; set; }
        [BsonIgnoreIfNull]
        public string Classification { get; set; }
        [BsonIgnoreIfNull]
        public string Shift { get; set; }
        [BsonIgnoreIfNull]
        public string Remark { get; set; }
        [BsonIgnoreIfNull]
        public string Process { get; set; }
    }
}
