using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Services.Dto;
using Thriot.Management.Model.Operations;
using Thriot.ServiceClient.TelemetrySetup;
using Thriot.TestHelpers;
using TelemetrySetup = Thriot.ServiceClient.TelemetrySetup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TelemetryDataSinkMetadataDto = Thriot.ServiceClient.TelemetrySetup.TelemetryDataSinkMetadataDto;
using TelemetryDataSinksMetadataDto = Thriot.ServiceClient.TelemetrySetup.TelemetryDataSinksMetadataDto;

namespace Thriot.Management.Services.Tests
{
    [TestClass]
    public class NetworkServiceTest
    {
        private IAuthenticationContext _authenticationContext;
        private string _userId;
        private string _companyId;
        private string _serviceId;
        private CompanyService _companyService;
        private ServiceService _serviceService;
        private NetworkService _networkService;
        private INetworkOperations _networkOperations;
        private ICompanyOperations _companyOperations;
        private IServiceOperations _serviceOperations;

        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void CreateNetworkUnderServiceTest()
        {
            var id =
                _networkService.Create(GetNetwork());

            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        public void CreateNetworkUnderNetworkTest()
        {
            var parentNetworkId =
                _networkService.Create(GetNetwork());

            var id =
                _networkService.Create(GetNetwork(parentNetworkId));

            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void CreateNetworkNotAuthenticatedTest()
        {
            _authenticationContext.GetContextUser().Returns((string)null);

            _networkService.Create(GetNetwork());
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void TryCreateNetworkUnderOtherCompanies1Test()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);
            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var userId1 = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);
            var userId2 = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            var companyService = new CompanyService(companyOperations, authenticationContext, null, new CapabilityProvider(settingProvider));

            authenticationContext.GetContextUser().Returns(userId1);

            var companyId1 = companyService.Create("new company1");

            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var serviceService = new ServiceService(serviceOperations, companyOperations, authenticationContext, null, new CapabilityProvider(settingProvider));

            var serviceId1 = serviceService.Create(new ServiceDto() { CompanyId = companyId1, Name = "svc" });

            authenticationContext.GetContextUser().Returns(userId2);

            var companyId2 = companyService.Create("new company2");

            var network = new NetworkDto()
            {
                CompanyId = companyId2,
                ServiceId = serviceId1,
                Name = "test"
            };

            var networkOperations = environmentFactory.MgmtNetworkOperations;
            var networkService = new NetworkService(networkOperations, serviceOperations, companyOperations, authenticationContext, null);
            networkService.Create(network);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void TryCreateNetworkUnderOtherCompanies2Test()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);
            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var userId1 = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            var companyService = new CompanyService(companyOperations, authenticationContext, null, new CapabilityProvider(settingProvider));

            authenticationContext.GetContextUser().Returns(userId1);

            var companyId1 = companyService.Create("new company1");

            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var serviceService = new ServiceService(serviceOperations, companyOperations, authenticationContext, null, new CapabilityProvider(settingProvider));

            var serviceId1 = serviceService.Create(new ServiceDto() { CompanyId = companyId1, Name = "svc" });

            var companyId2 = companyService.Create("new company2");

            var network = new NetworkDto()
            {
                CompanyId = companyId2,
                ServiceId = serviceId1,
                Name = "test"
            };

            var networkOperations = environmentFactory.MgmtNetworkOperations;
            var networkService = new NetworkService(networkOperations, serviceOperations, companyOperations, authenticationContext, null);
            networkService.Create(network);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void TryCreateNetworkUnderOtherNetworkTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);
            var userService = new UserService(userOperations, authenticationContext, settingProvider, null);

            var userId1 = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            var companyService = new CompanyService(companyOperations, authenticationContext, null, new CapabilityProvider(settingProvider));

            authenticationContext.GetContextUser().Returns(userId1);

            var companyId1 = companyService.Create("new company1");

            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var serviceService = new ServiceService(serviceOperations, companyOperations, authenticationContext, null, new CapabilityProvider(settingProvider));

            var serviceId1 = serviceService.Create(new ServiceDto { CompanyId = companyId1, Name = "svc" });

            var companyId2 = companyService.Create("new company2");

            var serviceId2 = serviceService.Create(new ServiceDto() { CompanyId = companyId2, Name = "svc" });

            var networkOperations = environmentFactory.MgmtNetworkOperations;
            var networkService = new NetworkService(networkOperations, serviceOperations, companyOperations, authenticationContext, null);

            var networkId2 = networkService.Create(new NetworkDto() {ServiceId = serviceId2, CompanyId = companyId2, Name = "svc" });

            var network = new NetworkDto()
            {
                ParentNetworkId = networkId2,
                CompanyId = companyId1,
                ServiceId = serviceId1,
                Name = "test"
            };

            networkService.Create(network);
        }

        [TestMethod]
        public void GetNetworkUnderServiceTest()
        {
            var id =
                _networkService.Create(GetNetwork());

            var newNetwork = _networkService.Get(id);

            Assert.AreEqual(id, newNetwork.Id);
            Assert.AreEqual("new network", newNetwork.Name);
            Assert.AreEqual(_companyId, newNetwork.CompanyId);
            Assert.AreEqual(_serviceId, newNetwork.ServiceId);
        }

        [TestMethod]
        public void GetNetworkUnderNetworkTest()
        {
            var parentNetworkId =
                _networkService.Create(GetNetwork());

            var id =
                _networkService.Create(GetNetwork(parentNetworkId));

            var newNetwork = _networkService.Get(id);

            Assert.AreEqual(id, newNetwork.Id);
            Assert.AreEqual("new network", newNetwork.Name);
            Assert.AreEqual(parentNetworkId, newNetwork.ParentNetworkId);
            Assert.AreEqual(_companyId, newNetwork.CompanyId);
            Assert.AreEqual(_serviceId, newNetwork.ServiceId);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void GetNetworkNotAuthenticatedTest()
        {
            var id =
                _networkService.Create(GetNetwork());

            _authenticationContext.GetContextUser().Returns((string)null);

            _networkService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void GetDeviceNotAuthorizedTest()
        {
            var id =
                _networkService.Create(GetNetwork());

            _authenticationContext.GetContextUser().Returns("12");

            _networkService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void GetNetworkNotFoundTest()
        {
            _networkService.Get(Identity.Next());
        }

        [TestMethod]
        public void UpdateNetworkUnderServiceTest()
        {
            var id = _networkService.Create(GetNetwork());

            var newParams = GetNetwork();
            newParams.Id = id;
            newParams.Name = "modified";
            newParams.CompanyId = null; // does not matter
            newParams.ServiceId = null; // does not matter

            _networkService.Update(newParams);

            var modNetwork = _networkService.Get(id);

            Assert.AreEqual(id, modNetwork.Id);
            Assert.AreEqual("modified", modNetwork.Name);
        }

        [TestMethod]
        public void UpdateNetworkUnderNetworkTest()
        {
            var parentNetworkId =
                _networkService.Create(GetNetwork());

            var id =
                _networkService.Create(GetNetwork(parentNetworkId));

            var newParams = GetNetwork();
            newParams.Id = id;
            newParams.Name = "modified";
            newParams.ParentNetworkId = null; // does not matter
            newParams.CompanyId = null; // does not matter
            newParams.ServiceId = null; // does not matter

            _networkService.Update(newParams);

            var modNetwork = _networkService.Get(id);

            Assert.AreEqual(id, modNetwork.Id);
            Assert.AreEqual("modified", modNetwork.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void UpdateNetworkNotAuthenticatedTest()
        {
            var id = _networkService.Create(GetNetwork());

            _authenticationContext.GetContextUser().Returns((string)null);

            _networkService.Update(new NetworkDto() { Id = id, Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void UpdateNetworkNotAuthorizedTest()
        {
            var id = _networkService.Create(GetNetwork());

            _authenticationContext.GetContextUser().Returns("12");

            _networkService.Update(new NetworkDto() { Id = id, Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateNetworkNotFoundTest()
        {
            _networkService.Update(new NetworkDto() { Id = Identity.Next(), Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void DeleteNetworkUnderServiceTest()
        {
            var id = _networkService.Create(GetNetwork());

            _networkService.Delete(id);

            _networkService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void DeleteNetworkUnderNetworkTest()
        {
            var parentNetworkId =
                _networkService.Create(GetNetwork());

            var id =
                _networkService.Create(GetNetwork(parentNetworkId));

            _networkService.Delete(id);

            _networkService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void DeleteNetworkNotAuthenticatedTest()
        {
            var id = _networkService.Create(GetNetwork());

            _authenticationContext.GetContextUser().Returns((string)null);

            _networkService.Delete(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void DeleteNetworkNotAuthorizedTest()
        {
            var id = _networkService.Create(GetNetwork());

            _authenticationContext.GetContextUser().Returns("12");

            _networkService.Delete(id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void DeleteNetworkNotFoundTest()
        {
            _networkService.Delete(Identity.Next());
        }

        [TestMethod]
        [ExpectedException(typeof(NotEmptyException))]
        public void TryDeleteServiceNotEmptyTest()
        {
            _networkService.Create(GetNetwork());

            _serviceService.Delete(_serviceId);
        }

        [TestMethod]
        [ExpectedException(typeof(NotEmptyException))]
        public void TryDeleteNetworkNotEmptyTest()
        {
            var parentNetworkId =
                _networkService.Create(GetNetwork());

            _networkService.Create(GetNetwork(parentNetworkId));

            _networkService.Delete(parentNetworkId);
        }

        [TestMethod]
        public void ListNetworksUnderServiceTest()
        {
            var id = _networkService.Create(GetNetwork());

            var networks = _serviceService.ListNetworks(_serviceId);

            Assert.AreEqual(1, networks.Count);
            Assert.AreEqual(id, networks[0].Id);
            Assert.AreEqual("new network", networks[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void ListNetworksNotAuthenticatedUnderServiceTest()
        {
            _networkService.Create(GetNetwork());

            _authenticationContext.GetContextUser().Returns((string)null);

            _serviceService.ListNetworks(_serviceId);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void ListNetworksNotAuthorizedUnderServiceTest()
        {
            _networkService.Create(GetNetwork());

            _authenticationContext.GetContextUser().Returns("123");

            _serviceService.ListNetworks(_serviceId);
        }

        [TestMethod]
        public void ListNetworksUnderNetworkTest()
        {
            var parentNetworkId =
                _networkService.Create(GetNetwork());

            var id =
                _networkService.Create(GetNetwork(parentNetworkId));

            var networks = _networkService.ListNetworks(parentNetworkId);

            Assert.AreEqual(1, networks.Count);
            Assert.AreEqual(id, networks[0].Id);
            Assert.AreEqual("new network", networks[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void ListNetworksNotAuthenticatedUnderNetworkTest()
        {
            var parentNetworkId =
                _networkService.Create(GetNetwork());

            _networkService.Create(GetNetwork(parentNetworkId));

            _authenticationContext.GetContextUser().Returns((string)null);

            _networkService.ListNetworks(parentNetworkId);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void ListNetworksNotAuthorizedUnderNetworkTest()
        {
            var parentNetworkId =
                _networkService.Create(GetNetwork());

            _networkService.Create(GetNetwork(parentNetworkId));

            _authenticationContext.GetContextUser().Returns("123");

            _networkService.ListNetworks(parentNetworkId);
        }

        [TestMethod]
        public void UpdateNetworkIncomingTelemetryDataSinkSettingsTest()
        {
            var id = _networkService.Create(GetNetwork());

            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();
            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata().Returns(
                new TelemetryDataSinksMetadataDto
                {
                    Incoming =
                        new List<TelemetryDataSinkMetadataDto>
                        {
                            new TelemetryDataSinkMetadataDto
                            {
                                Name = "test",
                                Description = null,
                                ParametersToInput = new List<string> {"k1", "k2"}
                            }
                        }
                });

            var networkService = new NetworkService(_networkOperations, _serviceOperations, _companyOperations, _authenticationContext, telemetryDataSinkSetupServiceClient);

            networkService.UpdateIncomingTelemetryDataSinks(id, new List<TelemetryDataSinkParametersDto>
                    {
                        new TelemetryDataSinkParametersDto
                        {
                            SinkName = "test",
                            Parameters = new Dictionary<string, string> {{"k1", "v1"}, {"k2", "v2"}}
                        }
                    });

            var modNetwork = networkService.Get(id);

            Assert.AreEqual(id, modNetwork.Id);
            Assert.AreEqual(1, modNetwork.TelemetryDataSinkSettings.Incoming.Count());
            Assert.AreEqual("test", modNetwork.TelemetryDataSinkSettings.Incoming.First().SinkName);
            Assert.AreEqual(2, modNetwork.TelemetryDataSinkSettings.Incoming.First().Parameters.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void UpdateNetworkIncomingTelemetryDataSinkSettingsNotAuthenticatedTest()
        {
            var id = _networkService.Create(GetNetwork());

            _authenticationContext.GetContextUser().Returns((string)null);

            _networkService.UpdateIncomingTelemetryDataSinks(id, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void UpdateNetworkIncomingTelemetryDataSinkSettingsNotAuthorizedTest()
        {
            var id = _networkService.Create(GetNetwork());

            _authenticationContext.GetContextUser().Returns("12");

            _networkService.UpdateIncomingTelemetryDataSinks(id, null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateNetworkIncomingTelemetryDataSinkSettingsNotFoundTest()
        {
            _networkService.UpdateIncomingTelemetryDataSinks(Identity.Next(), null);
        }

        private NetworkDto GetNetwork(string parentNetworkId = null)
        {
            return new NetworkDto()
            {
                Name = "new network",
                ParentNetworkId = parentNetworkId,
                CompanyId = _companyId,
                ServiceId = _serviceId
            };
        }

        private void Initialize()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            _authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            _companyOperations = environmentFactory.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);
            var userService = new UserService(userOperations, _authenticationContext, settingProvider, null);

            _userId = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            _companyService = new CompanyService(_companyOperations, _authenticationContext, null, new CapabilityProvider(settingProvider));

            _authenticationContext.GetContextUser().Returns(_userId);

            _companyId = _companyService.Create("new company");

            _serviceOperations = environmentFactory.MgmtServiceOperations;
            _serviceService = new ServiceService(_serviceOperations, _companyOperations, _authenticationContext, null, new CapabilityProvider(settingProvider));
            _serviceId = _serviceService.Create(new ServiceDto() { CompanyId = _companyId, Name = "new service" });

            _networkOperations = environmentFactory.MgmtNetworkOperations;
            _networkService = new NetworkService(_networkOperations, _serviceOperations, _companyOperations, _authenticationContext, null);
        }
    }
}
