using System.Collections.Generic;
using PDS.SpaceFE.RBG.PADS.Module.Data.PADSModel;

namespace PDS.SpaceFE.RBG.PADS.Module.Data
{
    public interface IPadsDao
    {
        SpacePads FindExistingDoc(string site, string timeGroup, string id);
        List<Data1ListRawValuesPads4Wafer> FindExistingWafDoc(string lot, string id, string paId);
        void InsertDoc(SpacePads document);
        void UpdateDoc(string site, string timeGroup, string id, SpacePads document);
    }
}
