using IoT.Objects.Operations.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Objects.Operations.Azure.Tests
{
    [TestClass]
    public class DeviceOperationsTest : DeviceOperationsTestBase
    {
        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public override void GetDeviceTest()
        {
            base.GetDeviceTest();
        }

        [TestMethod]
        public override void ListDevicesTest()
        {
            base.ListDevicesTest();
        }
    }
}

