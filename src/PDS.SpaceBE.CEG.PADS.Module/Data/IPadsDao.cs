using System.Collections.Generic;
using PDS.SpaceBE.CEG.PADS.Module.Data.PADSModel;
using PDS.SpaceBE.CEG.Common.Data.E4AModel;

namespace PDS.SpaceBE.CEG.PADS.Module.Data
{
    public interface IPadsDao
    {
        SpacePads FindExistingDoc(string site, string timeGroup, string id);
        void InsertDoc(SpacePads document);
        void UpdateDoc(string site, string timeGroup, string id, SpacePads document);
        SpaceE4A FindExistingE4ADoc(string id);
        void InsertE4ADoc(SpaceE4A document);
        void UpdateE4ADoc(string id, SpaceE4A document);
    }
}
