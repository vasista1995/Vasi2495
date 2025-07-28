using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceBE.Common.PADS.Module.Tests;

namespace PDS.SpaceBE.CEG.PADS.Module.Tests
{
    [TestClass]
    public class LoaderCodeAnalysisTest : LoaderPropertyEvaluatorTest
    {
        protected override string SiteKey => "CEG";
        protected override string SiteType => "SpaceBE";
    }
}
