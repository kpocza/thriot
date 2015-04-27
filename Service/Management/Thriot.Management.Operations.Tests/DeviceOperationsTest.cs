using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Model;
using Thriot.TestHelpers;

namespace Thriot.Management.Operations.Tests
{
    [TestClass]
    public class DeviceOperationsTest
    {
        [TestMethod]
        public void CreateDeviceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var deviceOperations = environmentFactory.MgmtDeviceOperations;

            var compServiceNetworkIds = CreateCompanyAndServiceAndNetwork();

            var id = deviceOperations.Create(new Device()
            {
                Name = "child device",
                Network = new Network() { Id = compServiceNetworkIds.NetworkId },
                Service = new Service() { Id = compServiceNetworkIds.ServiceId },
                Company = new Company() { Id = compServiceNetworkIds.CompanyId },
                DeviceKey = Crypto.GenerateSafeRandomToken()
            });

            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        public void GetDeviceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var deviceOperations = environmentFactory.MgmtDeviceOperations;

            var compServiceNetworkIds = CreateCompanyAndServiceAndNetwork();

            var device1 = new Device()
            {
                Network = new Network() { Id = compServiceNetworkIds.NetworkId },
                Service = new Service() { Id = compServiceNetworkIds.ServiceId },
                Company = new Company() { Id = compServiceNetworkIds.CompanyId },
                Name = "new device1",
                DeviceKey = Crypto.GenerateSafeRandomToken()
            };
            var device2 = new Device()
            {
                Network = new Network() { Id = compServiceNetworkIds.NetworkId },
                Service = new Service() { Id = compServiceNetworkIds.ServiceId },
                Company = new Company() { Id = compServiceNetworkIds.CompanyId },
                Name = "new device2",
                DeviceKey = Crypto.GenerateSafeRandomToken()
            };

            var device1Id = deviceOperations.Create(device1);
            var device2Id = deviceOperations.Create(device2);

            var d1 = deviceOperations.Get(device1Id);
            var d2 = deviceOperations.Get(device2Id);

            Assert.AreEqual(device1Id, d1.Id);
            Assert.AreEqual(device1.Name, d1.Name);
            Assert.AreEqual(compServiceNetworkIds.CompanyId, d1.Company.Id);
            Assert.AreEqual(compServiceNetworkIds.ServiceId, d1.Service.Id);
            Assert.AreEqual(compServiceNetworkIds.NetworkId, d1.Network.Id);
            Assert.AreEqual(device1.DeviceKey, d1.DeviceKey);
            Assert.AreEqual(device2Id, d2.Id);
            Assert.AreEqual(device2.Name, d2.Name);
            Assert.AreEqual(compServiceNetworkIds.CompanyId, d2.Company.Id);
            Assert.AreEqual(compServiceNetworkIds.ServiceId, d2.Service.Id);
            Assert.AreEqual(compServiceNetworkIds.NetworkId, d2.Network.Id);
            Assert.AreEqual(device2.DeviceKey, d2.DeviceKey);
        }

        [TestMethod]
        public void ListDevicesTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var deviceOperations = environmentFactory.MgmtDeviceOperations;
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var compServiceNetworkIds = CreateCompanyAndServiceAndNetwork();

            var device1 = new Device()
            {
                Network = new Network() { Id = compServiceNetworkIds.NetworkId },
                Service = new Service() { Id = compServiceNetworkIds.ServiceId },
                Company = new Company() { Id = compServiceNetworkIds.CompanyId },
                Name = "new device1",
                DeviceKey = Crypto.GenerateSafeRandomToken()
            };
            var device2 = new Device()
            {
                Network = new Network() { Id = compServiceNetworkIds.NetworkId },
                Service = new Service() { Id = compServiceNetworkIds.ServiceId },
                Company = new Company() { Id = compServiceNetworkIds.CompanyId },
                Name = "new device2",
                DeviceKey = Crypto.GenerateSafeRandomToken()
            };

            var device1Id = deviceOperations.Create(device1);
            var device2Id = deviceOperations.Create(device2);

            var devices = networkOperations.ListDevices(compServiceNetworkIds.NetworkId);

            Assert.AreEqual(2, devices.Count);

            var d1 = devices.Single(d => d.Id == device1Id);
            var d2 = devices.Single(d => d.Id == device2Id);

            Assert.AreEqual(device1Id, d1.Id);
            Assert.AreEqual(device1.Name, d1.Name);
            Assert.AreEqual(device2Id, d2.Id);
            Assert.AreEqual(device2.Name, d2.Name);
        }

        [TestMethod]
        public void DeleteDeviceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var deviceOperations = environmentFactory.MgmtDeviceOperations;
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var compServiceNetworkIds = CreateCompanyAndServiceAndNetwork();

            var id =
                deviceOperations.Create(new Device()
                {
                    Network = new Network() { Id = compServiceNetworkIds.NetworkId},
                    Service = new Service() { Id = compServiceNetworkIds.ServiceId },
                    Company = new Company() { Id = compServiceNetworkIds.CompanyId },
                    Name = "new device",
                    DeviceKey = Crypto.GenerateSafeRandomToken()
                });

            deviceOperations.Delete(id);

            var devices = networkOperations.ListDevices(compServiceNetworkIds.NetworkId);

            AssertionHelper.AssertThrows<NotFoundException>(() => deviceOperations.Get(id));

            Assert.AreEqual(0, devices.Count);
        }

        [TestMethod]
        public void UpdateDeviceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var deviceOperations = environmentFactory.MgmtDeviceOperations;
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var companyServiceNetworkIds = CreateCompanyAndServiceAndNetwork();

            var id =
                deviceOperations.Create(new Device()
                {
                    Network = new Network() { Id = companyServiceNetworkIds.NetworkId },
                    Service = new Service() { Id = companyServiceNetworkIds.ServiceId },
                    Company = new Company() { Id = companyServiceNetworkIds.CompanyId },
                    Name = "new device",
                    DeviceKey = Crypto.GenerateSafeRandomToken()
                });


            var newDevice = deviceOperations.Get(id);

            newDevice.Name += "mod";
            newDevice.DeviceKey = Identity.Next();
            deviceOperations.Update(newDevice);

            var updatedDevice = deviceOperations.Get(id);
            var devices = networkOperations.ListDevices(companyServiceNetworkIds.NetworkId);

            Assert.AreEqual("new devicemod", updatedDevice.Name);
            Assert.AreEqual(1, devices.Count);
            Assert.AreEqual("new devicemod", devices[0].Name);
            Assert.AreEqual(newDevice.DeviceKey, updatedDevice.DeviceKey);
        }

        private CompanyServiceNetworkIds CreateCompanyAndServiceAndNetwork()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var networkOperations = environmentFactory.MgmtNetworkOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var userId = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            var company = new Company { Name = "new company" };
            var companyId = companyOperations.Create(company, userId);

            var service = new Service { Company = new Company { Id = companyId }, Name = "new service", ApiKey = Crypto.GenerateSafeRandomToken() };
            var serviceId = serviceOperations.Create(service);

            var networkId =
                networkOperations.Create(new Network()
                {
                    Service = new Service() { Id = serviceId },
                    Company = new Company() { Id = companyId },
                    Name = "new network",
                    NetworkKey = Crypto.GenerateSafeRandomToken()
                });

            return new CompanyServiceNetworkIds {CompanyId = companyId, ServiceId = serviceId, NetworkId = networkId};
        }

        private class CompanyServiceNetworkIds
        {
            internal string NetworkId;
            internal string ServiceId;
            internal string CompanyId;
        }
    }
}
