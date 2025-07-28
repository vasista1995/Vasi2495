using System.Collections.Generic;
using PDS.Space.Common.Data.PADSModel;
using PDS.SpaceBE.CJJ.Common.Data.E4AModel;
using PDS.SpaceBE.CJJ.PADS.Module.Data.PADSModel;

namespace PDS.SpaceBE.CJJ.PADS.Module.Data
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
