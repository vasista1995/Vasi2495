using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDS.SpaceBE.CJJ.Source.Module.Tests
{
    [TestClass]
    public class SpaceE4AConverterTest
    {
        [TestMethod]
        public void TestGetSourceDataLevel()
        {
            Assert.AreEqual("C", SpaceE4AConverter.GetSourceDataLevel("Y", null));
            Assert.AreEqual("L", SpaceE4AConverter.GetSourceDataLevel("N", null));
            Assert.AreEqual("L", SpaceE4AConverter.GetSourceDataLevel("N", "LT8C"));
            Assert.AreEqual("L", SpaceE4AConverter.GetSourceDataLevel("N", "GreaterThan8Charcters"));
            Assert.AreEqual("C", SpaceE4AConverter.GetSourceDataLevel("Y", "GreaterThan8Charcters"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Not Valid RVStoreFlag: N, Y, corresponding IdSource is ")]
        public void TestGetSourceDataLevelException()
        {
            SpaceE4AConverter.GetSourceDataLevel("LT8C", "N, Y");
        }
    }
}
