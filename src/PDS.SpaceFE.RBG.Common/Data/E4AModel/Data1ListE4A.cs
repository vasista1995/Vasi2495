using System.Collections.Generic;
using Newtonsoft.Json;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceFE.RBG.Common.Data.E4AModel
{
    public class Data1ListE4A : BaseData1ListE4A
    {
        [JsonProperty(SpaceE4AProperties.DataFlatLimits)]
        public DataFlatLimitsE4A DataFlatLimits { get; set; }

        [JsonProperty(SpaceE4AProperties.Data1ListRawValues)]
        public List<Data1ListRawValuesE4A> Data1ListRawValues { get; set; }
    }
}
