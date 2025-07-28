using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceBE.Common.Source.Module.Tests;

namespace PDS.SpaceBE.BAT.Source.Module.Tests
{
    [TestClass]
    public class ExtractorCodeAnalysisTest : ExtractorPropertyEvaluatorTest
    {
        protected override string SiteKey => "BAT";
        protected override string SiteType => "SpaceBE";
    }
}
