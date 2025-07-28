using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDS.SpaceBE.CEG.Source.Module.Tests
{
    [TestClass]
    public class SpaceE4AConverterTest
    {
        [TestMethod]
        public void TestGetSourceDataLevel()
        {
            Assert.AreEqual("C", SpaceE4AConverter.GetSourceDataLevel("Y", "idSource"));
            Assert.AreEqual("L", SpaceE4AConverter.GetSourceDataLevel("N", null));
            Assert.AreEqual("L", SpaceE4AConverter.GetSourceDataLevel("N", "LT8C"));
            Assert.AreEqual("L", SpaceE4AConverter.GetSourceDataLevel("N", "GreaterThan8Characters"));
            Assert.AreEqual("C", SpaceE4AConverter.GetSourceDataLevel("Y", "GreaterThan8Characters"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Not Valid RVStoreFlag: N, Y, corresponding IdSource is ")]
        public void TestGetSourceDataLevelException()
        {
            SpaceE4AConverter.GetSourceDataLevel("C", "idSource");
        }
    }
}
