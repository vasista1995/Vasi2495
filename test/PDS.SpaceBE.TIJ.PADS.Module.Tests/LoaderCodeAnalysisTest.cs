using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceBE.Common.PADS.Module.Tests;

namespace PDS.SpaceBE.TIJ.PADS.Module.Tests
{
    [TestClass]
    public class LoaderCodeAnalysisTest : LoaderPropertyEvaluatorTest
    {
        protected override string SiteKey => "TIJ";
        protected override string SiteType => "SpaceBE";
    }
}
