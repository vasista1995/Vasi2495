using System.Collections.Generic;
using Newtonsoft.Json;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceBE.BAT.Common.Data.E4AModel
{
    public class Data1ListE4A : BaseData1ListE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to Data1list section in e4a document.
        /// </summary>

        [JsonProperty(SpaceE4AProperties.DataFlatLimits)]
        public DataFlatLimitsE4A DataFlatLimits { get; set; }

        [JsonProperty(SpaceE4AProperties.Data1ListRawValues)]
        public List<Data1ListRawValuesE4A> Data1ListRawValues { get; set; }
    }
}
