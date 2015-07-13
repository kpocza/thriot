using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Framework;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.ServiceClient.Messaging;
using Thriot.ServiceClient.TelemetrySetup;
using Thriot.TestHelpers;
using TelemetryDataSinkMetadataDto = Thriot.ServiceClient.TelemetrySetup.TelemetryDataSinkMetadataDto;
using TelemetryDataSinksMetadataDto = Thriot.ServiceClient.TelemetrySetup.TelemetryDataSinksMetadataDto;
using TelemetrySetup = Thriot.ServiceClient.TelemetrySetup;

namespace Thriot.Objects.Operations.Tests
{
    [TestClass]
    public class NetworkOperationsTest
    {
        private IAuthenticationContext _authenticationContext;
        private string _userId;
        private string _companyId;
        private string _serviceId;
        private CompanyService _companyService;
        private ServiceService _serviceService;
        private NetworkService _networkService;
        private DeviceService _deviceService;

        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void GetNetworkUnderServiceTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();

            var network1Id = _networkService.Create(new NetworkDto()
            {
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new network1"
            });

            var network2Id = _networkService.Create(new NetworkDto()
            {
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new network2"
            });

            var pltNetworkOperations = environmentFactory.ObjNetworkOperations;

            var dg1 = pltNetworkOperations.Get(network1Id);
            var dg2 = pltNetworkOperations.Get(network2Id);

            Assert.AreEqual(network1Id, dg1.Id);
            Assert.AreEqual(_companyId, dg1.CompanyId);
            Assert.AreEqual(_serviceId, dg1.ServiceId);
            Assert.IsNull(dg1.ParentNetworkId);
            Assert.AreEqual(32, dg1.NetworkKey.Length);
            Assert.AreEqual(network2Id, dg2.Id);
            Assert.AreEqual(_companyId, dg2.CompanyId);
            Assert.AreEqual(_serviceId, dg2.ServiceId);
            Assert.IsNull(dg2.ParentNetworkId);
            Assert.AreEqual(32, dg2.NetworkKey.Length);
            Assert.AreNotEqual(dg1.NetworkKey, dg2.NetworkKey);
        }

        [TestMethod]
        public void GetNetworkUnderNetworkTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();

            var parentNetworkId = CreateParentNetwork();

            var network1Id = _networkService.Create(new NetworkDto()
            {
                ParentNetworkId = parentNetworkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new network1"
            });

            var network2Id = _networkService.Create(new NetworkDto()
            {
                ParentNetworkId = parentNetworkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new network2"
            });

            var pltNetworkOperations = environmentFactory.ObjNetworkOperations;

            var dg1 = pltNetworkOperations.Get(network1Id);
            var dg2 = pltNetworkOperations.Get(network2Id);

            Assert.AreEqual(network1Id, dg1.Id);
            Assert.AreEqual(_companyId, dg1.CompanyId);
            Assert.AreEqual(_serviceId, dg1.ServiceId);
            Assert.AreEqual(parentNetworkId, dg1.ParentNetworkId);
            Assert.AreEqual(network2Id, dg2.Id);
            Assert.AreEqual(_companyId, dg2.CompanyId);
            Assert.AreEqual(_serviceId, dg2.ServiceId);
            Assert.AreEqual(parentNetworkId, dg2.ParentNetworkId);
        }

        [TestMethod]
        public void GetNetworkMessagingSinkTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();

            var networkId = _networkService.Create(new NetworkDto()
            {
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new network1"
            });

            var incoming =
                new List<TelemetryDataSinkParametersDto>
                {
                    new TelemetryDataSinkParametersDto
                    {
                        SinkName = "test",
                        Parameters = new Dictionary<string, string> {{"k1", "v1"}, {"k2", "v2"}}
                    }
                };
            _networkService.UpdateIncomingTelemetryDataSinks(networkId, incoming);

            var pltNetworkOperations = environmentFactory.ObjNetworkOperations;

            var dg = pltNetworkOperations.Get(networkId);

            Assert.AreEqual(networkId, dg.Id);
            Assert.AreEqual(_companyId, dg.CompanyId);
            Assert.AreEqual(_serviceId, dg.ServiceId);
            Assert.AreEqual(1, dg.TelemetryDataSinkSettings.Incoming.Count());
            Assert.AreEqual("test", dg.TelemetryDataSinkSettings.Incoming.First().SinkName);
            Assert.AreEqual(2, dg.TelemetryDataSinkSettings.Incoming.First().Parameters.Count);
        }

        [TestMethod]
        public void ListDevicesTest()
        {
            var networkId = _networkService.Create(new NetworkDto()
            {
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new network1"
            });

            var environmentFactory = EnvironmentFactoryFactory.Create();

            var device1 = new DeviceDto()
            {
                NetworkId = networkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new device1"
            };
            var device2 = new DeviceDto()
            {
                NetworkId = networkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new device2"
            };
            var device3 = new DeviceDto()
            {
                NetworkId = networkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new device3"
            };

            _deviceService.Create(device1);
            var device2Id = _deviceService.Create(device2);
            _deviceService.Create(device3);

            var pltNetworkOperations = environmentFactory.ObjNetworkOperations;
            var ds = pltNetworkOperations.ListDevices(networkId);
            Assert.AreEqual(3, ds.Count());
            Assert.IsTrue(ds.Any(d => d.Id == device2Id && d.Name == device2.Name));
        }

        protected void Initialize()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            _authenticationContext = Substitute.For<IAuthenticationContext>();
            var messagingService = Substitute.For<IMessagingService>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, _authenticationContext, settingProvider, null);
            _userId = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            _companyService = new CompanyService(companyOperations, _authenticationContext, null, new CapabilityProvider(settingProvider));

            _authenticationContext.GetContextUser().Returns(_userId);

            _companyId = _companyService.Create("new company");

            var serviceOperations = environmentFactory.MgmtServiceOperations;
            _serviceService = new ServiceService(serviceOperations, companyOperations, _authenticationContext, null, new CapabilityProvider(settingProvider));
            _serviceId = _serviceService.Create(new ServiceDto() { CompanyId = _companyId, Name = "new service" });

            var networkOperations = environmentFactory.MgmtNetworkOperations;
            var telemetryDataSinkSetupService = Substitute.For<ITelemetryDataSinkSetupService>();
            telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata().Returns(
                new TelemetryDataSinksMetadataDto
                {
                    Incoming = new List<TelemetryDataSinkMetadataDto>
                    {
                        new TelemetryDataSinkMetadataDto
                        {
                            Name = "test",
                            Description = null,
                            ParametersToInput = new List<string> {"k1", "k2"}
                        }
                    }
                }); 
            _networkService = new NetworkService(networkOperations, serviceOperations, companyOperations, _authenticationContext, telemetryDataSinkSetupService);

            messagingService.Initialize("1234").ReturnsForAnyArgs(1);

            var deviceOperations = environmentFactory.MgmtDeviceOperations;
            _deviceService = new DeviceService(deviceOperations, networkOperations, serviceOperations, companyOperations,
                _authenticationContext, messagingService);
        }

        private string CreateParentNetwork()
        {
            var id =
                _networkService.Create(new NetworkDto()
                {
                    ServiceId = _serviceId,
                    CompanyId = _companyId,
                    Name = "new network"
                });
            return id;
        }
    }
}

