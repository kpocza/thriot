using IoT.Objects.Operations.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Objects.Operations.Sql.Tests
{
    [TestClass]
    public class NetworkOperationsTest : NetworkOperationsTestBase
    {
        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public override void GetNetworkUnderServiceTest()
        {
            base.GetNetworkUnderServiceTest();
        }

        [TestMethod]
        public override void GetNetworkUnderNetworkTest()
        {
            base.GetNetworkUnderNetworkTest();
        }

        [TestMethod]
        public override void GetNetworkMessagingSinkTest()
        {
            base.GetNetworkMessagingSinkTest();
        }

        [TestMethod]
        public override void ListDevicesTest()
        {
            base.ListDevicesTest();
        }
    }
}

