using IoT.Objects.Operations.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Objects.Operations.Azure.Tests
{
    [TestClass]
    public class ServiceOperationsTest : ServiceOperationsTestBase
    {
        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public override void GetServiceTest()
        {
            base.GetServiceTest();
        }
    }
}

