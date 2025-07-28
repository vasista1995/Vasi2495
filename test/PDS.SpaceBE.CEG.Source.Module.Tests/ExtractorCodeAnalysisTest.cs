using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceBE.Common.Source.Module.Tests;

namespace PDS.SpaceBE.CEG.Source.Module.Tests
{
    [TestClass]
    public class ExtractorCodeAnalysisTest : ExtractorPropertyEvaluatorTest
    {
        protected override string SiteKey => "CEG";
        protected override string SiteType => "SpaceBE";
    }
}
