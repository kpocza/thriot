﻿using IoT.Framework;
using IoT.Management.Dto;
using IoT.Management.Services;
using IoT.ServiceClient.Messaging;
using IoT.UnitTestHelpers;
using NSubstitute;

namespace IoT.Plugins.TestBase
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
        private IMessagingService _messagingService;

        protected void InitializeDevice()
        {
            if (!IsIntegrationTest())
                return;

            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            _authenticationContext = Substitute.For<IAuthenticationContext>();
            _messagingService = Substitute.For<IMessagingService>();
            var settingProvider = Substitute.For<ISettingProvider>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
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

            var device = new DeviceDto()
            {
                NetworkId = _networkId,
                CompanyId = _companyId,
                ServiceId = _serviceId,
                Name = "new device"
            };

            _deviceId = _deviceService.Create(device);
        }

        protected abstract bool IsIntegrationTest();
    }
}
