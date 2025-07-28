using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceBE.Common.Source.Module.Tests;

namespace PDS.SpaceBE.WAR.Source.Module.Tests
{
    [TestClass]
    public class ExtractorCodeAnalysisTest : ExtractorPropertyEvaluatorTest
    {
        protected override string SiteKey => "WAR";
        protected override string SiteType => "SpaceBE";
    }
}
