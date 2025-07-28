using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;
using System.Text;

namespace PDS.SpaceBE.CJJ.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to DataFlatmetadata section in pads document.
    /// </summary>
    ///
    public class DataFlatMetaDataPads : BaseDataFlatMetaDataPads
    {
        [BsonIgnoreIfNull]
        public string Classification { get; set; }
        [BsonIgnoreIfNull]
        public string Shift { get; set; }
        [BsonIgnoreIfNull]
        public string Process { get; set; }
        [BsonIgnoreIfNull]
        public string Baunumber { get; set; }
        [BsonIgnoreIfNull]
        public string OperatorId { get; set; }
        [BsonIgnoreIfNull]
        public string Material { get; set; }
    }
}
