using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceBE.Common.Source.Module.Tests;

namespace PDS.SpaceFE.RBG.Source.Module.Tests
{
    [TestClass]
    public class ExtractorCodeAnalysisTest : ExtractorPropertyEvaluatorTest
    {
        protected override string SiteKey => "RBG";
        protected override string SiteType => "SpaceFE";
    }
}
