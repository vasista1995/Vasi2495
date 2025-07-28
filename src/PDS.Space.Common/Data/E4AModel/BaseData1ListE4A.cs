using MongoDB.Bson.Serialization.Attributes;

namespace PDS.Space.Common.Data.E4AModel
{
    public class BaseData1ListE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to Data1list section in e4a document.
        /// </summary>
        [BsonIgnoreIfNull]
        public int SampleSize { get; set; }
    }
}
