using System.Collections.Generic;
using PDS.Space.Common.Data.SpaceModel;

namespace PDS.SpaceFE.RBG.Source.Module.Data.SpaceModel
{
    /// <summary>
    /// It takes all the values from the sql statements and converts them into the below properties.
    /// </summary>
    public class SpaceEntry : BaseSpaceEntry
    {
        public string SPSNumber { get; set; }
        public string ProcessGroup { get; set; }
        public string Recipe { get; set; }
        public string ProcessLine { get; set; }
        public string ProcessEquipment { get; set; }
        public string MeasurementEquipmentType { get; set; }
        public string Design { get; set; }
        public string Layer { get; set; }
        public string MeasurementBatch { get; set; }
        public string ProcessBatch { get; set; }
        public string MotherlotWafer { get; set; }
        public List<SpaceRawValuesEntry> SpaceRawValues { get; set; }
    }
}
