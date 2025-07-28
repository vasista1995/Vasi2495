
namespace PDS.SpaceBE.Common.Tests.Updater
{
    public static class Program
    {
        ///// <summary>
        ///// Main method of the application
        ///// </summary>
        public static void Main(string[] args)
        {
            //UpdateRegressionTestsForSite("TIJ");
            //UpdateRegressionTestsForSite("CEG");
            //UpdateRegressionTestsForSite("BAT");
            //UpdateRegressionTestsForSite("CJJ");
            UpdateRegressionTestsForSite("WUX");

        }

        private static void UpdateRegressionTestsForSite(string site)
        {
            TestCaseUpdater.UpdateSourceRegressionTests(site);
            TestCaseUpdater.PrepareLoaderInput(site);
            TestCaseUpdater.UpdateLoaderRegressionTests(site);
        }
    }
}
