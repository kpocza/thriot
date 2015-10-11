using NSubstitute;
using Thriot.Framework;
using Thriot.Management.Services.Dto;
using Thriot.Management.Services;
using Thriot.Messaging.Services.Client;
using Thriot.TestHelpers;

namespace Thriot.Platform.PersistentConnections.Tests
{
    public abstract class TestBase
    {
        private IAuthenticationContext _authenticationContext;
        private string _userId;
        protected string _companyId;
        protected string _serviceId;
        protected string _networkId;
        protected static string _deviceId;
        private CompanyService _companyService;
        private ServiceService _serviceService;
        private NetworkService _networkService;
        protected DeviceService _deviceService;
        private IMessagingServiceClient _messagingServiceClient;

        protected void Initialize()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            _authenticationContext = Substitute.For<IAuthenticationContext>();
            _messagingServiceClient = environmentFactory.MessagingEnvironment.MessagingServiceClient;

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

            var deviceOperations = environmentFactory.ManagementEnvironment.MgmtDeviceOperations;
            _deviceService = new DeviceService(deviceOperations, networkOperations, serviceOperations, companyOperations,
                _authenticationContext, _messagingServiceClient);

            var device = new DeviceDto()
            {
                NetworkId = _networkId,
                CompanyId = _companyId,
                ServiceId = _serviceId,
                Name = "new device"
            };

            _deviceId = _deviceService.Create(device);
        }
    }
}
