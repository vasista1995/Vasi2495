using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.SpaceBE.Common.Source.Module.Tests;

namespace PDS.SpaceBE.CJJ.Source.Module.Tests
{
    [TestClass]
    public class ExtractorCodeAnalysisTest : ExtractorPropertyEvaluatorTest
    {
        protected override string SiteKey => "CJJ";
        protected override string SiteType => "SpaceBE";
    }
}
