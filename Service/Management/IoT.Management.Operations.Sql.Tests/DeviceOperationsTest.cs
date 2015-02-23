using IoT.Management.Operations.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Management.Operations.Sql.Tests
{
    [TestClass]
    public class DeviceOperationsTest : DeviceOperationsTestBase
    {
        [TestMethod]
        public override void CreateDeviceTest()
        {
            base.CreateDeviceTest();
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

        [TestMethod]
        public override void DeleteDeviceTest()
        {
            base.DeleteDeviceTest();
        }

        [TestMethod]
        public override void UpdateDeviceTest()
        {
            base.UpdateDeviceTest();
        }
    }
}
