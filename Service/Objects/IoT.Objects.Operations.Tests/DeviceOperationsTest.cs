﻿using System.Linq;
using IoT.Framework;
using IoT.Management.Dto;
using IoT.Management.Services;
using IoT.ServiceClient.Messaging;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IoT.Objects.Operations.Tests
{
    [TestClass]
    public class DeviceOperationsTest
    {
        private IAuthenticationContext _authenticationContext;
        private string _userId;
        private string _companyId;
        private string _serviceId;
        private string _networkId;
        private CompanyService _companyService;
        private ServiceService _serviceService;
        private NetworkService _networkService;
        private DeviceService _deviceService;
        private IMessagingService _messagingService;


        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void GetDeviceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();

            var device1 = new DeviceDto()
            {
                NetworkId = _networkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new device1"
            };
            var device2 = new DeviceDto()
            {
                NetworkId = _networkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new device2"
            };

            var device1Id = _deviceService.Create(device1);
            var device2Id = _deviceService.Create(device2);

            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;
            var d1 = pltDeviceOperations.Get(device1Id);
            var d2 = pltDeviceOperations.Get(device2Id);

            Assert.AreEqual(device1Id, d1.Id);
            Assert.AreEqual(_companyId, d1.CompanyId);
            Assert.AreEqual(_serviceId, d1.ServiceId);
            Assert.AreEqual(_networkId, d1.NetworkId);
            Assert.AreEqual(device1.Name, d1.Name);
            Assert.AreEqual(device2Id, d2.Id);
            Assert.AreEqual(_companyId, d2.CompanyId);
            Assert.AreEqual(_serviceId, d2.ServiceId);
            Assert.AreEqual(_networkId, d2.NetworkId);
        }

        [TestMethod]
        public void ListDevicesTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();

            var device1 = new DeviceDto()
            {
                NetworkId = _networkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new device1"
            };
            var device2 = new DeviceDto()
            {
                NetworkId = _networkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new device2"
            };
            var device3 = new DeviceDto()
            {
                NetworkId = _networkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new device3"
            };
            var device4 = new DeviceDto()
            {
                NetworkId = _networkId,
                ServiceId = _serviceId,
                CompanyId = _companyId,
                Name = "new device4"
            };
            var device1Id = _deviceService.Create(device1);
            var device2Id = _deviceService.Create(device2);
            var device3Id = _deviceService.Create(device3);
            var device4Id = _deviceService.Create(device4);

            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;
            var ds = pltDeviceOperations.ListDevices(new[] {device1Id, device2Id, device3Id});
            Assert.AreEqual(3, ds.Count());
            Assert.IsTrue(ds.Any(d => d.Id == device2Id && d.Name == device2.Name));
            Assert.IsTrue(ds.All(d => d.Id != device4Id));
        }

        protected void Initialize()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            _authenticationContext = Substitute.For<IAuthenticationContext>();
            _messagingService = Substitute.For<IMessagingService>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, _authenticationContext, settingProvider, null);
            _userId = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate() }, "password", null);

            _companyService = new CompanyService(companyOperations, _authenticationContext, null, new CapabilityProvider(settingProvider));

            _authenticationContext.GetContextUser().Returns(_userId);

            _companyId = _companyService.Create("new company");

            var serviceOperations = environmentFactory.MgmtServiceOperations;
            _serviceService = new ServiceService(serviceOperations, companyOperations, _authenticationContext, null, new CapabilityProvider(settingProvider));
            _serviceId = _serviceService.Create(new ServiceDto() { CompanyId = _companyId, Name = "new service" });

            var networkOperations = environmentFactory.MgmtNetworkOperations;
            _networkService = new NetworkService(networkOperations, serviceOperations, companyOperations, _authenticationContext, null);

            var network = new NetworkDto()
            {
                Name = "new network",
                ParentNetworkId = null,
                CompanyId = _companyId,
                ServiceId = _serviceId
            };

            _networkId = _networkService.Create(network);

            _messagingService.Initialize("1234").ReturnsForAnyArgs(1);

            var deviceOperations = environmentFactory.MgmtDeviceOperations;
            _deviceService = new DeviceService(deviceOperations, networkOperations, serviceOperations, companyOperations,
                _authenticationContext, _messagingService);
        }
    }
}
