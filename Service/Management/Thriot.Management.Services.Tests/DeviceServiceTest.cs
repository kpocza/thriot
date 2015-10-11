using System.Security.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Services.Dto;
using Thriot.Messaging.Services.Client;
using Thriot.TestHelpers;

namespace Thriot.Management.Services.Tests
{
    [TestClass]
    public class DeviceServiceTest
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
        public void CreateDeviceTest()
        {
            var id =
                _deviceService.Create(GetDevice());

            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void CreateDeviceNotAuthenticatedTest()
        {
            _authenticationContext.GetContextUser().Returns((string)null);

            _deviceService.Create(GetDevice());
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void TryCreateDeviceUnderOtherNetworkTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            var messagingService = Substitute.For<IMessagingServiceClient>();

            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.ManagementEnvironment.MgmtSettingOperations);

            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);
            var userId1 =
                userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            var companyService = new CompanyService(companyOperations, authenticationContext, null, new CapabilityProvider(settingProvider));

            authenticationContext.GetContextUser().Returns(userId1);

            var companyId1 = companyService.Create("new company1");

            var serviceOperations = environmentFactory.ManagementEnvironment.MgmtServiceOperations;
            var serviceService = new ServiceService(serviceOperations, companyOperations, authenticationContext, null, new CapabilityProvider(settingProvider));

            var serviceId1 = serviceService.Create(new ServiceDto() { CompanyId = companyId1, Name = "svc" });

            var companyId2 = companyService.Create("new company2");

            var serviceId2 = serviceService.Create(new ServiceDto() { CompanyId = companyId2, Name = "svc" });

            var networkOperations = environmentFactory.ManagementEnvironment.MgmtNetworkOperations;
            var networkService = new NetworkService(networkOperations, serviceOperations, companyOperations, authenticationContext, null);

            var deviceOperations = environmentFactory.ManagementEnvironment.MgmtDeviceOperations;
            var deviceService = new DeviceService(deviceOperations, networkOperations, serviceOperations, companyOperations, authenticationContext, messagingService);

            var networkId2 = networkService.Create(new NetworkDto() { ServiceId = serviceId2, CompanyId = companyId2, Name = "svc" });

            var device = new DeviceDto()
            {
                NetworkId = networkId2,
                CompanyId = companyId1,
                ServiceId = serviceId1,
                Name = "test"
            };

            deviceService.Create(device);
        }

        [TestMethod]
        public void GetDeviceTest()
        {
            var id =
                _deviceService.Create(GetDevice());

            var newDevice = _deviceService.Get(id);

            Assert.AreEqual(id, newDevice.Id);
            Assert.AreEqual("new device", newDevice.Name);
            Assert.AreEqual(_networkId, newDevice.NetworkId);
            Assert.AreEqual(_companyId, newDevice.CompanyId);
            Assert.AreEqual(_serviceId, newDevice.ServiceId);
            Assert.AreEqual(32, newDevice.DeviceKey.Length);
            Assert.IsTrue(newDevice.NumericId > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void GetDeviceNotAuthenticatedTest()
        {
            var id =
                _deviceService.Create(GetDevice());

            _authenticationContext.GetContextUser().Returns((string)null);

            _deviceService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void GetDeviceNotAuthorizedTest()
        {
            var id =
                _deviceService.Create(GetDevice());

            _authenticationContext.GetContextUser().Returns("12");

            _deviceService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void GetDeviceNotFoundTest()
        {
            _deviceService.Get(Identity.Next());
        }

        [TestMethod]
        public void UpdateDeviceTest()
        {
            var id =
                _deviceService.Create(GetDevice());

            var origAccessKey = _deviceService.Get(id).DeviceKey;

            var newParams = GetDevice();

            newParams.Id = id;
            newParams.Name = "modified";
            newParams.NetworkId = null; // does not matter
            newParams.CompanyId = null; // does not matter
            newParams.ServiceId = null; // does not matter
            newParams.DeviceKey = Identity.Next(); // does not matter

            _deviceService.Update(newParams);

            var modDevice = _deviceService.Get(id);

            Assert.AreEqual(id, modDevice.Id);
            Assert.AreEqual("modified", modDevice.Name);
            Assert.AreEqual(origAccessKey, modDevice.DeviceKey);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void UpdateDeviceNotAuthenticatedTest()
        {
            var id = _deviceService.Create(GetDevice());

            _authenticationContext.GetContextUser().Returns((string)null);

            _deviceService.Update(new DeviceDto() { Id = id, Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void UpdateDeviceNotAuthorizedTest()
        {
            var id = _deviceService.Create(GetDevice());

            _authenticationContext.GetContextUser().Returns("12");

            _deviceService.Update(new DeviceDto() { Id = id, Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateDeviceNotFoundTest()
        {
            _deviceService.Update(new DeviceDto() { Id = Identity.Next(), Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void DeleteDeviceTest()
        {
            var id =
                _deviceService.Create(GetDevice());

            _deviceService.Delete(id);

            _deviceService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void DeleteDeviceNotAuthenticatedTest()
        {
            var id = _deviceService.Create(GetDevice());

            _authenticationContext.GetContextUser().Returns((string)null);

            _deviceService.Delete(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void DeleteDeviceNotAuthorizedTest()
        {
            var id = _deviceService.Create(GetDevice());

            _authenticationContext.GetContextUser().Returns("12");

            _deviceService.Delete(id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void DeleteDeviceNotFoundTest()
        {
            _deviceService.Delete(Identity.Next());
        }

        [TestMethod]
        [ExpectedException(typeof(NotEmptyException))]
        public void TryDeleteNetworkNotEmptyTest()
        {
            _deviceService.Create(GetDevice());

            _networkService.Delete(_networkId);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void ListDevicesNotAuthenticatedTest()
        {
            _deviceService.Create(GetDevice());

            _authenticationContext.GetContextUser().Returns((string)null);

            _networkService.ListDevices(_networkId);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void ListDeviceNotAuthorizedTest()
        {
            _deviceService.Create(GetDevice());

            _authenticationContext.GetContextUser().Returns("123");

            _networkService.ListDevices(_networkId);
        }

        [TestMethod]
        public void ListNetworksUnderNetworkTest()
        {
            var id =
                _deviceService.Create(GetDevice());

            var devices = _networkService.ListDevices(_networkId);

            Assert.AreEqual(1, devices.Count);
            Assert.AreEqual(id, devices[0].Id);
            Assert.AreEqual("new device", devices[0].Name);
        }

        private DeviceDto GetDevice()
        {
            return new DeviceDto()
            {
                Name = "new device",
                NetworkId = _networkId,
                CompanyId = _companyId,
                ServiceId = _serviceId
            };
        }

        private void Initialize()
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
