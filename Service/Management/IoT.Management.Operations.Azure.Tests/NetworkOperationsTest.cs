using IoT.Management.Operations.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Management.Operations.Azure.Tests
{
    [TestClass]
    public class NetworkOperationsTest : NetworkOperationsTestBase
    {
        #region Under service

        [TestMethod]
        public override void CreateNetworkUnderServiceTest()
        {
            base.CreateNetworkUnderServiceTest();
        }

        [TestMethod]
        public override void GetNetworkUnderServiceTest()
        {
            base.GetNetworkUnderServiceTest();
        }

        [TestMethod]
        public override void ListNetworkUnderServiceTest()
        {
            base.ListNetworkUnderServiceTest();
        }

        [TestMethod]
        public override void DeleteNetworkUnderServiceTest()
        {
            base.DeleteNetworkUnderServiceTest();
        }

        [TestMethod]
        public override void UpdateNetworkUnderServiceTest()
        {
            base.UpdateNetworkUnderServiceTest();
        }

        [TestMethod]
        public override void UpdateNetworkUnderService2Test()
        {
            base.UpdateNetworkUnderService2Test();
        }

        #endregion

        #region Under other network

        [TestMethod]
        public override void CreateNetworkUnderNetworkTest()
        {
            base.CreateNetworkUnderNetworkTest();
        }

        [TestMethod]
        public override void GetNetworkUnderNetworkTest()
        {
            base.GetNetworkUnderNetworkTest();
        }

        [TestMethod]
        public override void ListNetworkUnderNetworkTest()
        {
            base.ListNetworkUnderNetworkTest();
        }

        [TestMethod]
        public override void DeleteNetworkUnderNetworkTest()
        {
            base.DeleteNetworkUnderNetworkTest();
        }

        [TestMethod]
        public override void UpdateNetworkUnderNetworkTest()
        {
            base.UpdateNetworkUnderNetworkTest();
        }

        [TestMethod]
        public override void UpdateNetworkUnderNetwork2Test()
        {
            base.UpdateNetworkUnderNetwork2Test();
        }
        
        #endregion
    }
}

