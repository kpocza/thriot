using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Framework.Tests
{
    [TestClass]
    public class IdentityTest
    {
        [TestMethod]
        public void GenerateIdentityTest()
        {
            var id = Identity.Next();

            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        public void GenerateIdentity2Test()
        {
            var id1 = Identity.Next();
            var id2 = Identity.Next();
            
            Assert.AreNotEqual(id1, id2);
        }

        [TestMethod]
        public void GenerateIncrementalIdentityTest()
        {
            var id = Identity.NextIncremental();

            Assert.IsFalse(id.Contains(" "));
            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        public void GenerateIncrementalIdentity2Test()
        {
            var id1 = Identity.NextIncremental();
            var id2 = Identity.NextIncremental();

            Assert.AreNotEqual(id1, id2);
        }

        [TestMethod]
        public void GenerateIncrementalIdentity3Test()
        {
            var id1 = Identity.NextIncremental();
            var id2 = Identity.NextIncremental();

            Assert.AreEqual(id1.Substring(0, 7), id2.Substring(0, 7));
        }

        [TestMethod]
        public void GenerateIncrementalIdentity4Test()
        {
            var id1 = Identity.NextIncremental();
            var id2 = Identity.NextIncremental();

            Assert.AreNotEqual(id1.Substring(0, 13), id2.Substring(0, 13));
        }
    }
}
