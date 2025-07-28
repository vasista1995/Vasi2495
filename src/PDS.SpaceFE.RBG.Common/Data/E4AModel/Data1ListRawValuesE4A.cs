using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceFE.RBG.Common.Data.E4AModel
{
    /// <summary>
    /// This class possess all the properties that must be assigned to rawvalues array subsection of Data1list section in e4a document.
    /// </summary>
    public class Data1ListRawValuesE4A : BaseData1ListRawValuesE4A
    {
        public string ProcessEquipment { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string GOF { get; set; }
        public string Slot { get; set; }
        public string WaferSequence { get; set; }
        public string MotherLotWafer { get; set; }
        public string TestPosition { get; set; }
    }
}
