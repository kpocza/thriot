using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using IoT.Client.DotNet.Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Client.DotNet.IntegrationTests
{
    [TestClass]
    public class ManagementClientTest : TestBase
    {
        #region User

        [TestMethod]
        public void RegisterUserTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register { Email = Guid.NewGuid() + "@test.hu", Name = "test user", Password = "p@ssw0rd"});

            Assert.IsTrue(managementClient.User.IsLoggedIn);
        }

        [TestMethod]
        public void LoginLogoffGetUserTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            var email = Guid.NewGuid() + "@test.hu";

            managementClient.User.Register(new Register { Email = email, Name = "test user", Password = "p@ssw0rd" });
            Assert.IsTrue(managementClient.User.IsLoggedIn);

            managementClient.User.Logoff();
            Assert.IsFalse(managementClient.User.IsLoggedIn);

            managementClient.User.Login(new Login { Email =  email, Password = "p@ssw0rd"});
            Assert.IsTrue(managementClient.User.IsLoggedIn);

            var user = managementClient.User.Get();

            Assert.IsNotNull(user.Id);
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual("test user", user.Name);

            managementClient.User.Logoff();
            Assert.IsFalse(managementClient.User.IsLoggedIn);
        }

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.Unauthorized)]
        public void GetUserNotAuthTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Get();
        }

        [TestMethod]
        public void FindUserTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            var email = "user+" + Guid.NewGuid() + "@test.hu";
            managementClient.User.Register(new Register { Email = email, Name = "test user", Password = "p@ssw0rd" });

            var me = managementClient.User.Get();
            var user = managementClient.User.FindUser(email);

            Assert.AreEqual(me.Id, user.Id);
            Assert.AreEqual(me.Email, user.Email);
        }

	    #endregion    

        #region Company

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.NotFound)]
        public void CreateGetUpdateGetDeleteGetCompanyTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var companyId = managementClient.Company.Create(new Company {Name = "company"});
            Assert.IsNotNull(companyId);

            var company = managementClient.Company.Get(companyId);
            Assert.AreEqual("company", company.Name);

            company.Name += "mod";

            managementClient.Company.Update(company);

            company = managementClient.Company.Get(companyId);
            Assert.AreEqual("companymod", company.Name);

            managementClient.Company.Delete(companyId);

            managementClient.Company.Get(companyId);
        }

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.BadRequest)]
        public void CreateCompanyBadRequestTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            managementClient.Company.Create(new Company { Name = null });
        }

        [TestMethod]
        public void MultiCreateListCompanyTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register { Email = Guid.NewGuid() + "@test.hu", Name = "test user", Password = "p@ssw0rd" });

            managementClient.Company.Create(new Company { Name = "company1" });
            managementClient.Company.Create(new Company { Name = "company2" });

            var companies = managementClient.Company.List();

            Assert.AreEqual(2, companies.Count());
            Assert.IsTrue(companies.Any(c => c.Name == "company1"));
            Assert.IsTrue(companies.Any(c => c.Name == "company2"));
        }

        [TestMethod]
        public void CompanyAddUserListUsersTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            var email1 = Guid.NewGuid() + "@test.hu";
            var email2 = Guid.NewGuid() + "@test.hu";
            managementClient.User.Register(new Register { Email = email1, Name = "test user", Password = "p@ssw0rd" });
            managementClient.User.Register(new Register { Email = email2, Name = "test user", Password = "p@ssw0rd" });
            managementClient.User.Logoff();
            managementClient.User.Login(new Login() {Email = email1, Password = "p@ssw0rd"});

            var companyId = managementClient.Company.Create(new Company { Name = "company" });
            Assert.IsNotNull(companyId);

            var company = managementClient.Company.Get(companyId);

            var user2 = managementClient.User.FindUser(email2);

            managementClient.Company.AddUser(new CompanyUser {CompanyId = company.Id, UserId = user2.Id});

            var users = managementClient.Company.ListUsers(company.Id);
            Assert.AreEqual(2, users.Count());

            managementClient.User.Logoff();
            managementClient.User.Login(new Login() { Email = email2, Password = "p@ssw0rd" });

            var companies = managementClient.Company.List();
            Assert.AreEqual(1, companies.Count());
        }

        #endregion

        #region Service

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.NotFound)]
        public void CreateGetUpdateGetDeleteGetServiceTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var companyId = managementClient.Company.Create(new Company {Name = "company"});

            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "árvíztűrő tükörfúrógép"});
            Assert.IsNotNull(serviceId);

            var service = managementClient.Service.Get(serviceId);
            Assert.AreEqual("árvíztűrő tükörfúrógép", service.Name);
            Assert.IsNotNull(service.ApiKey);
            Assert.AreEqual(companyId, service.CompanyId);

            service.Name += "mod";
            service.ApiKey = "aaa";

            managementClient.Service.Update(service);

            service = managementClient.Service.Get(serviceId);

            Assert.AreEqual("árvíztűrő tükörfúrógépmod", service.Name);
            Assert.AreEqual(32, service.ApiKey.Length);

            managementClient.Service.Delete(serviceId);

            managementClient.Service.Get(serviceId);
        }
  
        [TestMethod]
        public void MultiCreateListServiceTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register { Email = Guid.NewGuid() + "@test.hu", Name = "test user", Password = "p@ssw0rd" });

            var companyId = managementClient.Company.Create(new Company { Name = "company1" });

            managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service1" });
            managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service2" });

            var services = managementClient.Company.ListServices(companyId);

            Assert.AreEqual(2, services.Count());
            Assert.IsTrue(services.Any(c => c.Name == "service1"));
            Assert.IsTrue(services.Any(c => c.Name == "service2"));
        }
        
        #endregion

        #region Network

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.NotFound)]
        public void CreateGetUpdateGetDeleteGetNetworkUnderServiceTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var companyId = managementClient.Company.Create(new Company { Name = "company" });

            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service" });

            var networkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "árvíztűrő tükörfúrógép" });
            Assert.IsNotNull(networkId);

            var network = managementClient.Network.Get(networkId);
            Assert.AreEqual("árvíztűrő tükörfúrógép", network.Name);
            Assert.AreEqual(companyId, network.CompanyId);
            Assert.AreEqual(serviceId, network.ServiceId);

            network.Name += "mod";

            managementClient.Network.Update(network);

            network = managementClient.Network.Get(networkId);

            Assert.AreEqual("árvíztűrő tükörfúrógépmod", network.Name);

            managementClient.Network.Delete(networkId);

            managementClient.Network.Get(networkId);
        }

        [TestMethod]
        public void MultiCreateListNetworkUnderServiceTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register { Email = Guid.NewGuid() + "@test.hu", Name = "test user", Password = "p@ssw0rd" });

            var companyId = managementClient.Company.Create(new Company { Name = "company1" });
            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service1" });

            managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "network1" });
            managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "network2" });

            var networks = managementClient.Service.ListNetworks(serviceId);

            Assert.AreEqual(2, networks.Count());
            Assert.IsTrue(networks.Any(c => c.Name == "network1"));
            Assert.IsTrue(networks.Any(c => c.Name == "network2"));
        }

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.NotFound)]
        public void CreateGetUpdateGetDeleteGetNetworkUnderNetworkTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var companyId = managementClient.Company.Create(new Company { Name = "company" });

            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service" });

            var parentNetworkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "árvíztűrő tükörfúrógép" });

            var networkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, ParentNetworkId = parentNetworkId, Name = "árvíztűrő tükörfúrógép" });
            Assert.IsNotNull(networkId);

            var network = managementClient.Network.Get(networkId);
            Assert.AreEqual("árvíztűrő tükörfúrógép", network.Name);
            Assert.AreEqual(companyId, network.CompanyId);
            Assert.AreEqual(serviceId, network.ServiceId);

            network.Name += "mod";

            managementClient.Network.Update(network);

            network = managementClient.Network.Get(networkId);

            Assert.AreEqual("árvíztűrő tükörfúrógépmod", network.Name);

            managementClient.Network.Delete(networkId);

            managementClient.Network.Get(networkId);
        }

        [TestMethod]
        public void MultiCreateListNetworkUnderNetworkTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register { Email = Guid.NewGuid() + "@test.hu", Name = "test user", Password = "p@ssw0rd" });

            var companyId = managementClient.Company.Create(new Company { Name = "company1" });
            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service1" });

            var parentNetworkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "árvíztűrő tükörfúrógép" });

            managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, ParentNetworkId = parentNetworkId, Name = "network1" });
            managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, ParentNetworkId = parentNetworkId, Name = "network2" });

            var networks = managementClient.Network.ListNetworks(parentNetworkId);

            Assert.AreEqual(2, networks.Count());
            Assert.IsTrue(networks.Any(c => c.Name == "network1"));
            Assert.IsTrue(networks.Any(c => c.Name == "network2"));
        }

        #endregion

        #region Device

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.NotFound)]
        public void CreateGetUpdateGetDeleteGetDeviceTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var companyId = managementClient.Company.Create(new Company { Name = "company" });

            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service" });

            var networkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "árvíztűrő tükörfúrógép" });

            var deviceId = managementClient.Device.Create(new Device { CompanyId = companyId, ServiceId = serviceId, NetworkId = networkId, Name = "árvíztűrő tükörfúrógép" });
            Assert.IsNotNull(deviceId);

            var device = managementClient.Device.Get(deviceId);
            Assert.AreEqual("árvíztűrő tükörfúrógép", device.Name);
            Assert.IsNotNull(device.DeviceKey);
            Assert.AreEqual(companyId, device.CompanyId);
            Assert.AreEqual(serviceId, device.ServiceId);

            device.Name += "mod";
            device.DeviceKey = "aaa";

            managementClient.Device.Update(device);

            device = managementClient.Device.Get(deviceId);

            Assert.AreEqual("árvíztűrő tükörfúrógépmod", device.Name);
            Assert.AreEqual(32, device.DeviceKey.Length);

            managementClient.Device.Delete(deviceId);

            managementClient.Device.Get(deviceId);
        }

        [TestMethod]
        public void MultiCreateListDeviceTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register { Email = Guid.NewGuid() + "@test.hu", Name = "test user", Password = "p@ssw0rd" });

            var companyId = managementClient.Company.Create(new Company { Name = "company1" });
            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service1" });
            var networkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "network1" });

            managementClient.Device.Create(new Device { CompanyId = companyId, ServiceId = serviceId, NetworkId = networkId, Name = "device1" });
            managementClient.Device.Create(new Device { CompanyId = companyId, ServiceId = serviceId, NetworkId = networkId, Name = "device2" });

            var devices = managementClient.Network.ListDevices(networkId);

            Assert.AreEqual(2, devices.Count());
            Assert.IsTrue(devices.Any(c => c.Name == "device1"));
            Assert.IsTrue(devices.Any(c => c.Name == "device2"));
        }

        #endregion

        #region TelemetryMetadata and settings

        [TestMethod]
        public void TelemetryMetadataGetTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var telemetryDataSinksMetadata = managementClient.TelemetryDataSinksMetadata.Get();

            Assert.IsTrue(telemetryDataSinksMetadata.Incoming.Count > 0);
            Assert.IsTrue(telemetryDataSinksMetadata.Incoming.Sum(i => i.ParametersToInput.Count) > 0);
        }

        [TestMethod]
        public void UpdateIncomingMessageSinksCompanyServiceNetworkTest()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var companyId = managementClient.Company.Create(new Company { Name = "company" });
            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "árvíztűrő tükörfúrógép" });
            var networkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "árvíztűrő tükörfúrógép" });

            var telemetryDataSinkParameters = new List<TelemetryDataSinkParameters>
            {
                new TelemetryDataSinkParameters
                {
                    SinkName = SinkData,
                    Parameters = new Dictionary<string, string>()
                },
                new TelemetryDataSinkParameters
                {
                    SinkName = SinkTimeSeries,
                    Parameters = new Dictionary<string, string>()
                }
            };

            managementClient.Company.UpdateIncomingTelemetryDataSinks(companyId, telemetryDataSinkParameters);
            managementClient.Service.UpdateIncomingTelemetryDataSinks(serviceId, telemetryDataSinkParameters);
            managementClient.Network.UpdateIncomingTelemetryDataSinks(networkId, telemetryDataSinkParameters);

            var company = managementClient.Company.Get(companyId);
            var service = managementClient.Service.Get(serviceId);
            var network = managementClient.Network.Get(networkId);

            Assert.AreEqual(2, company.TelemetryDataSinkSettings.Incoming.Count);
            Assert.IsTrue(company.TelemetryDataSinkSettings.Incoming.Any(i => i.SinkName == SinkData.ToLowerInvariant()));
            Assert.IsTrue(company.TelemetryDataSinkSettings.Incoming.Any(i => i.SinkName == SinkTimeSeries.ToLowerInvariant()));

            Assert.AreEqual(2, service.TelemetryDataSinkSettings.Incoming.Count);
            Assert.IsTrue(service.TelemetryDataSinkSettings.Incoming.Any(i => i.SinkName == SinkData.ToLowerInvariant()));
            Assert.IsTrue(service.TelemetryDataSinkSettings.Incoming.Any(i => i.SinkName == SinkTimeSeries.ToLowerInvariant()));

            Assert.AreEqual(2, network.TelemetryDataSinkSettings.Incoming.Count);
            Assert.IsTrue(network.TelemetryDataSinkSettings.Incoming.Any(i => i.SinkName == SinkData.ToLowerInvariant()));
            Assert.IsTrue(network.TelemetryDataSinkSettings.Incoming.Any(i => i.SinkName == SinkTimeSeries.ToLowerInvariant()));
        }

        [TestMethod]
        public void UpdateIncomingMessageSinksWithParams()
        {
            var managementClient = ManagementClientFactory.Create(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var companyId = managementClient.Company.Create(new Company { Name = "company" });
            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "árvíztűrő tükörfúrógép" });
            var networkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "árvíztűrő tükörfúrógép" });

            var telemetryDataSinkParameters = new List<TelemetryDataSinkParameters>
            {
                new TelemetryDataSinkParameters
                {
                    SinkName = ParamSinkData,
                    Parameters = new Dictionary<string, string>()
                    {
                        {"ConnectionString", ParamSinkDataConnectionString},
                        {"Table", "CurrentData"}
                    }
                }
            };

            managementClient.Company.UpdateIncomingTelemetryDataSinks(companyId, telemetryDataSinkParameters);
            managementClient.Service.UpdateIncomingTelemetryDataSinks(serviceId, telemetryDataSinkParameters);
            managementClient.Network.UpdateIncomingTelemetryDataSinks(networkId, telemetryDataSinkParameters);

            var company = managementClient.Company.Get(companyId);
            var service = managementClient.Service.Get(serviceId);
            var network = managementClient.Network.Get(networkId);

            Assert.AreEqual(1, company.TelemetryDataSinkSettings.Incoming.Count);
            Assert.IsTrue(company.TelemetryDataSinkSettings.Incoming.Any(i => i.SinkName == ParamSinkData.ToLowerInvariant()));

            Assert.AreEqual(1, service.TelemetryDataSinkSettings.Incoming.Count);
            Assert.IsTrue(service.TelemetryDataSinkSettings.Incoming.Any(i => i.SinkName == ParamSinkData.ToLowerInvariant()));

            Assert.AreEqual(1, network.TelemetryDataSinkSettings.Incoming.Count);
            Assert.IsTrue(network.TelemetryDataSinkSettings.Incoming.Any(i => i.SinkName == ParamSinkData.ToLowerInvariant()));
        }

        #endregion
    }
}
