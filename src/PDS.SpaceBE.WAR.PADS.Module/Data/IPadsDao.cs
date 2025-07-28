using System.Collections.Generic;
using PDS.SpaceBE.WAR.PADS.Module.Data.PADSModel;

namespace PDS.SpaceBE.WAR.PADS.Module.Data
{
    public interface IPadsDao
    {
        SpacePads FindExistingDoc(string site, string timeGroup, string id);
        void InsertDoc(SpacePads document);
        void UpdateDoc(string site, string timeGroup, string id, SpacePads document);
    }
}
