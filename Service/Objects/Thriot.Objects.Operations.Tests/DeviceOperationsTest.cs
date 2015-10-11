using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Framework;
using Thriot.Management.Services.Dto;
using Thriot.Management.Services;
using Thriot.Messaging.Services.Client;
using Thriot.TestHelpers;

namespace Thriot.Objects.Operations.Tests
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
        private IMessagingServiceClient _messagingServiceClient;


        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void GetDeviceTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();

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

            var pltDeviceOperations = environmentFactory.ManagementEnvironment.ObjDeviceOperations;
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
            var environmentFactory = EnvironmentFactoryFactory.Create();

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

            var pltDeviceOperations = environmentFactory.ManagementEnvironment.ObjDeviceOperations;
            var ds = pltDeviceOperations.ListDevices(new[] {device1Id, device2Id, device3Id});
            Assert.AreEqual(3, ds.Count());
            Assert.IsTrue(ds.Any(d => d.Id == device2Id && d.Name == device2.Name));
            Assert.IsTrue(ds.All(d => d.Id != device4Id));
        }

        protected void Initialize()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            _authenticationContext = Substitute.For<IAuthenticationContext>();
            _messagingServiceClient = Substitute.For<IMessagingServiceClient>();

            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.ManagementEnvironment.MgmtSettingOperations);

            var userService = new UserService(userOperations, _authenticationContext, settingProvider, null);
            _userId = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            _companyService = new CompanyService(companyOperations, _authenticationContext, null, new CapabilityProvider(settingProvider));

            _authenticationContext.GetContextUser().Returns(_userId);

            _companyId = _companyService.Create("new company");

            var serviceOperations = environmentFactory.ManagementEnvironment.MgmtServiceOperations;
            _serviceService = new ServiceService(serviceOperations, companyOperations, _authenticationContext, null, new CapabilityProvider(settingProvider));
            _serviceId = _serviceService.Create(new ServiceDto() { CompanyId = _companyId, Name = "new service" });

            var networkOperations = environmentFactory.ManagementEnvironment.MgmtNetworkOperations;
            _networkService = new NetworkService(networkOperations, serviceOperations, companyOperations, _authenticationContext, null);

            var network = new NetworkDto()
            {
                Name = "new network",
                ParentNetworkId = null,
                CompanyId = _companyId,
                ServiceId = _serviceId
            };

            _networkId = _networkService.Create(network);

            _messagingServiceClient.Initialize("1234").ReturnsForAnyArgs(1);

            var deviceOperations = environmentFactory.ManagementEnvironment.MgmtDeviceOperations;
            _deviceService = new DeviceService(deviceOperations, networkOperations, serviceOperations, companyOperations,
                _authenticationContext, _messagingServiceClient);
        }
    }
}

