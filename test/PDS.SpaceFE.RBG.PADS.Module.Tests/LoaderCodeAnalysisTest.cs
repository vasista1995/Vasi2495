using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceBE.Common.PADS.Module.Tests;

namespace PDS.SpaceFE.RBG.PADS.Module.Tests
{
    [TestClass]
    public class LoaderCodeAnalysisTest : LoaderPropertyEvaluatorTest
    {
        protected override string SiteKey => "RBG";
        protected override string SiteType => "SpaceFE";
    }
}
