using NSubstitute;
using Thriot.Management.Services.Dto;
using Thriot.Management.Services;
using Thriot.Messaging.Services.Client;
using Thriot.TestHelpers;

namespace Thriot.Plugins.Tests
{
    public abstract class TestBase
    {
        private Management.Services.IAuthenticationContext _authenticationContext;
        private string _userId;
        protected string _companyId;
        protected string _serviceId;
        protected string _networkId;
        protected string _deviceId;
        private CompanyService _companyService;
        private ServiceService _serviceService;
        private NetworkService _networkService;
        protected DeviceService _deviceService;
        private IMessagingServiceClient _messagingServiceClient;

        protected void InitializeDevice()
        {
            if (!IsIntegrationTest())
                return;

            var environmentFactory = EnvironmentFactoryFactory.Create();
            _authenticationContext = Substitute.For<IAuthenticationContext>();
            _messagingServiceClient = Substitute.For<IMessagingServiceClient>();
            var settingProvider = Substitute.For<ISettingProvider>();

            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;
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

            var device = new DeviceDto()
            {
                NetworkId = _networkId,
                CompanyId = _companyId,
                ServiceId = _serviceId,
                Name = "new device"
            };

            _deviceId = _deviceService.Create(device);
        }

        protected virtual string GetConnectionString()
        {
            return EnvironmentFactoryFactory.Create().TelemetryEnvironment.TelemetryConnectionString;
        }

        protected virtual bool IsIntegrationTest()
        {
            {
#pragma warning disable 0162
#if INTEGRATIONTEST
                return true;
#endif
                return false;
#pragma warning restore 0162
            }
        }
    }
}
