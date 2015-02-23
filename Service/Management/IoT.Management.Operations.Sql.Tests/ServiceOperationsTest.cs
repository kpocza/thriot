using IoT.Management.Operations.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Management.Operations.Sql.Tests
{
    [TestClass]
    public class ServiceOperationsTest : ServiceOperationsTestBase
    {
        [TestMethod]
        public override void CreateServiceTest()
        {
            base.CreateServiceTest();
        }

        [TestMethod]
        public override void GetServiceTest()
        {
            base.GetServiceTest();
        }

        [TestMethod]
        public override void ListServicesTest()
        {
            base.ListServicesTest();
        }

        [TestMethod]
        public override void DeleteServiceTest()
        {
            base.DeleteServiceTest();
        }

        [TestMethod]
        public override void UpdateServiceTest()
        {
            base.UpdateServiceTest();
        }

        [TestMethod]
        public override void UpdateService2Test()
        {
            base.UpdateService2Test();
        }
    }
}

