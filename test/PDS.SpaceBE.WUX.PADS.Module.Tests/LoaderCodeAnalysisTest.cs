using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceBE.Common.PADS.Module.Tests;

namespace PDS.SpaceBE.WUX.PADS.Module.Tests
{
    [TestClass]
    public class LoaderCodeAnalysisTest : LoaderPropertyEvaluatorTest
    {
        protected override string SiteKey => "WUX";
        protected override string SiteType => "SpaceBE";
    }
}
