using PDS.Space.Common.Data.SpaceModel;

namespace PDS.SpaceFE.RBG.Source.Module.Data.SpaceModel
{
    public class SpaceRawValuesEntry : BaseSpaceRawValuesEntry
    {
        public string WaferName { get; set; }
        public string ProcessEquipment { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string GOF { get; set; }
        public string Slot { get; set; }
        public string WaferSequence { get; set; }
        public string TestPosition { get; set; }
        
    }
}
