using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceBE.Common.PADS.Module.Tests;

namespace PDS.SpaceBE.WAR.PADS.Module.Tests
{
    [TestClass]
    public class LoaderCodeAnalysisTest : LoaderPropertyEvaluatorTest
    {
        protected override string SiteKey => "WAR";
        protected override string SiteType => "SpaceBE";
    }
}
