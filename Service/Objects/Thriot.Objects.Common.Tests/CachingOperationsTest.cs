using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Objects.Common.CachingOperations;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.TestHelpers;

namespace Thriot.Objects.Common.Tests
{
    [TestClass]
    public class CachingOperationsTest
    {
        [TestMethod]
        public void DeviceOperationsTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();

            var cachingDeviceOperations = new DeviceOperations(deviceOperations);
            cachingDeviceOperations.Remove("1");

            deviceOperations.Get("1").Returns(TestDataCreator.Device("1", "2", "3", "4", "5", 1));

            var device = cachingDeviceOperations.Get("1");
            Assert.AreEqual("2", device.DeviceKey);

            deviceOperations.Received().Get("1");

            deviceOperations.ClearReceivedCalls();

            var device2 = cachingDeviceOperations.Get("1");
            Assert.AreEqual("2", device2.DeviceKey);

            deviceOperations.DidNotReceive().Get("1");
        }

        [TestMethod]
        public void NetworkOperationsTest()
        {
            var networkOperations = Substitute.For<INetworkOperations>();

            var cachingNetworkOperations = new NetworkOperations(networkOperations);
            cachingNetworkOperations.Remove("1");

            networkOperations.Get("1").Returns(TestDataCreator.Network("1", "key", "2", "3", "4", null));

            var network = cachingNetworkOperations.Get("1");
            Assert.AreEqual("2", network.ParentNetworkId);

            networkOperations.Received().Get("1");

            networkOperations.ClearReceivedCalls();

            var network2 = cachingNetworkOperations.Get("1");
            Assert.AreEqual("2", network2.ParentNetworkId);

            networkOperations.DidNotReceive().Get("1");
        }

        [TestMethod]
        public void ServiceOperationsTest()
        {
            var serviceOperations = Substitute.For<IServiceOperations>();

            var cachingServiceOperations = new ServiceOperations(serviceOperations);
            cachingServiceOperations.Remove("1");

            serviceOperations.Get("1").Returns(TestDataCreator.Service("1", "2", "3", null));

            var service = cachingServiceOperations.Get("1");
            Assert.AreEqual("2", service.ApiKey);

            serviceOperations.Received().Get("1");

            serviceOperations.ClearReceivedCalls();

            var service2 = cachingServiceOperations.Get("1");
            Assert.AreEqual("2", service2.ApiKey);

            serviceOperations.DidNotReceive().Get("1");
        }

        [TestMethod]
        public void CompanyOperationsTest()
        {
            var companyOperations = Substitute.For<ICompanyOperations>();

            var cachingCompanyOperations = new CompanyOperations(companyOperations);
            cachingCompanyOperations.Remove("1");

            companyOperations.Get("1").Returns(TestDataCreator.Company("1", null));

            var company = cachingCompanyOperations.Get("1");
            Assert.AreEqual("1", company.Id);

            companyOperations.Received().Get("1");

            companyOperations.ClearReceivedCalls();

            var company2 = cachingCompanyOperations.Get("1");
            Assert.AreEqual("1", company2.Id);

            companyOperations.DidNotReceive().Get("1");
        }

        [TestMethod]
        public void SettingOperationsTest()
        {
            var settingOperations = Substitute.For<ISettingOperations>();

            var cachingSettingOperations = new SettingOperations(settingOperations);
            cachingSettingOperations.Remove("cat.conf");

            var id = new SettingId("cat", "conf");

            settingOperations.Get(id).Returns(TestDataCreator.Setting(id, "2"));

            var setting = cachingSettingOperations.Get(id);
            Assert.AreEqual("2", setting.Value);

            settingOperations.Received().Get(id);

            settingOperations.ClearReceivedCalls();

            var setting2 = cachingSettingOperations.Get(id);
            Assert.AreEqual("2", setting2.Value);

            settingOperations.DidNotReceive().Get(id);
        }
    }
}
