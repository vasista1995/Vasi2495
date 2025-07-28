namespace PDS.SpaceBE.Common.Source.Module.Tests
{
    public interface IRegressionTestSourceUpdater
    {
        void UpdateSourceRegressionTests(string assemblyPath);
        void PrepareLoaderInput(string assemblyPath, string site);
    }
}
