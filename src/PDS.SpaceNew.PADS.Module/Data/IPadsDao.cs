using System.Collections.Generic;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.PADS.Module.Data.PADSModel;

namespace PDS.SpaceNew.PADS.Module.Data
{
    public interface IPadsDao
    {
        SpacePads FindExistingDoc(string site, string timeGroup, string id);
        List<Data1ListRawValuesPads4Wafer> FindExistingWafDoc(string measLot, string motherLotWafer, string paId);
        void InsertDoc(SpacePads document);
        void UpdateDoc(string site, string timeGroup, string id, SpacePads document);
        SpaceE4A FindExistingE4ADoc(string id);
        void InsertE4ADoc(SpaceE4A document);
        void UpdateE4ADoc(string id, SpaceE4A document);
    }
}
