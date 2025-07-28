using PDS.Space.Common.Data.E4AModel;

namespace PDS.SpaceFE.RBG.Common.Data.E4AModel
{
    public class DataFlatMetaDataE4A : BaseDataFlatMetaDataE4A
    {
        /// <summary>
        /// This class possess all the properties that must be assigned to DataFlatmetadata section in e4a document.
        /// </summary>
        public string ProcessEquipment { get; set; }
        public string Design { get; set; }
        public string Layer { get; set; }
        public string ProcessBatch { get; set; }
        public string MeasurementBatch { get; set; }
        public string SPSNumber { get; set; }
        public string Recipe { get; set; }
        public string ProcessGroup { get; set; }
        public string EquipmentType { get; set; }
        public string ProcessLine { get; set; }
    }
}
