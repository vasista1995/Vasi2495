using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PDS.Space.Common.Data.PADSModel;
using System;

namespace PDS.SpaceBE.RBG.PADS.Module.Data.PADSModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to Rawvalues array subsection of data1list section in pads document.
    /// </summary>
    public class Data1ListRawValuesPads4Wafer : BaseData1ListRawValuesPads
    {
        
        [BsonIgnoreIfNull]
        public string WaferLot { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterName { get; set; }
        [BsonIgnoreIfNull]
        public string ParameterUnit { get; set; }

        //NOTE: 'new' Needed to work correctly
        [BsonIgnoreIfNull]
        public new string IsFlagged { get; set; }
        
    }
}
