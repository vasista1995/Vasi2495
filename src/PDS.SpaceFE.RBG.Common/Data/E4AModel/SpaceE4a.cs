using Newtonsoft.Json;
using PDS.Common.E4AModel;
using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceFE.RBG.Common.Data.E4AModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to e4a document.
    /// each property has its own class where the subproperties of each property are defined.
    /// </summary>
    public class SpaceE4A : BaseSpaceE4A
    {
        public SpaceE4A(BaseSystemLogE4A systemLog) : base(systemLog)
        {
        }

        [JsonProperty(E4aProperties.DataFlatMetaData)]
        public DataFlatMetaDataE4A DataFlatMetaData { get; set; }

        [JsonProperty(E4aProperties.Data1ListParameters)]
        public Data1ListE4A Data1List { get; set; }
    }
}
