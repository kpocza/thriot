using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Dto;
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
    public class ServiceServiceTest
    {
        private IAuthenticationContext _authenticationContext;
        private string _userId;
        private string _companyId;
        private CompanyService _companyService;
        private ServiceService _serviceService;
        private ICompanyOperations _companyOperations;
        private IServiceOperations _serviceOperations;

        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void CreateServiceTest()
        {
            var id =
                _serviceService.Create(GetService());

            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void CreateServiceNotAuthenticatedTest()
        {
            _authenticationContext.GetContextUser().Returns((string)null);

            _serviceService.Create(GetService());
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void TryCreateServiceUnderOthersCompanyTest()
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

            authenticationContext.GetContextUser().Returns(userId2);

            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var serviceService = new ServiceService(serviceOperations, companyOperations, authenticationContext, null, new CapabilityProvider(settingProvider));

            serviceService.Create(new ServiceDto() { CompanyId = companyId1, Name = "svc"});
        }

        [TestMethod]
        public void GetServiceTest()
        {
            var id = _serviceService.Create(GetService());

            var newService = _serviceService.Get(id);

            Assert.AreEqual(id, newService.Id);
            Assert.AreEqual("new service", newService.Name);
            Assert.AreEqual(_companyId, newService.CompanyId);
            Assert.AreEqual(32, newService.ApiKey.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void GetServiceNotAuthenticatedTest()
        {
            var id = _serviceService.Create(GetService());

            _authenticationContext.GetContextUser().Returns((string)null);

            _serviceService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void GetServiceNotAuthorizedTest()
        {
            var id = _serviceService.Create(GetService());

            _authenticationContext.GetContextUser().Returns("12");

            _serviceService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void GetServiceNotFoundTest()
        {
            _serviceService.Get(Identity.Next());
        }

        [TestMethod]
        public void UpdateServiceTest()
        {
            var id = _serviceService.Create(GetService());

            var origApiKey = _serviceService.Get(id).ApiKey;

            _serviceService.Update(new ServiceDto() {Id = id, Name = "modified", ApiKey = Identity.Next()});

            var modService = _serviceService.Get(id);

            Assert.AreEqual(id, modService.Id);
            Assert.AreEqual("modified", modService.Name);
            Assert.AreEqual(origApiKey, modService.ApiKey); // api key does not change
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void UpdateServiceNotAuthenticatedTest()
        {
            var id = _serviceService.Create(GetService());

            _authenticationContext.GetContextUser().Returns((string)null);

            _serviceService.Update(new ServiceDto() { Id = id, Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void UpdateServiceNotAuthorizedTest()
        {
            var id = _serviceService.Create(GetService());

            _authenticationContext.GetContextUser().Returns("12");

            _serviceService.Update(new ServiceDto() { Id = id, Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateServiceNotFoundTest()
        {
            _serviceService.Update(new ServiceDto() { Id = Identity.Next(), Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void DeleteServiceTest()
        {
            var id = _serviceService.Create(GetService());

            _serviceService.Delete(id);

            _serviceService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void DeleteServiceNotAuthenticatedTest()
        {
            var id = _serviceService.Create(GetService());

            _authenticationContext.GetContextUser().Returns((string)null);

            _serviceService.Delete(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void DeleteServiceNotAuthorizedTest()
        {
            var id = _serviceService.Create(GetService());

            _authenticationContext.GetContextUser().Returns("12");

            _serviceService.Delete(id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void DeleteServiceNotFoundTest()
        {
            _serviceService.Delete(Identity.Next());
        }

        [TestMethod]
        [ExpectedException(typeof(NotEmptyException))]
        public void TryDeleteCompanyNotEmptyTest()
        {
            _serviceService.Create(GetService());

            _companyService.Delete(_companyId);
        }

        [TestMethod]
        public void ListServicesTest()
        {
            var id = _serviceService.Create(GetService());

            var services = _companyService.ListServices(_companyId);

            Assert.AreEqual(1, services.Count);
            Assert.AreEqual(id, services[0].Id);
            Assert.AreEqual("new service", services[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void ListServicesNotAuthenticatedTest()
        {
            _authenticationContext.GetContextUser().Returns((string)null);

            _companyService.ListServices(_companyId);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void ListServicesNotAuthorizedTest()
        {
            _authenticationContext.GetContextUser().Returns("123");

            _companyService.ListServices(_companyId);
        }

        [TestMethod]
        public void UpdateServiceIncomingTelemetryDataSinkSettingsTest()
        {
            var id = _serviceService.Create(GetService());

            var telemetryDataSinkSetupService = Substitute.For<ITelemetryDataSinkSetupService>();
            telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata().Returns(
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
            var serviceService = new ServiceService(_serviceOperations, _companyOperations, _authenticationContext, telemetryDataSinkSetupService, null);

            serviceService.UpdateIncomingTelemetryDataSinks(id, new List<TelemetryDataSinkParametersDto>
                    {
                        new TelemetryDataSinkParametersDto
                        {
                            SinkName = "test",
                            Parameters = new Dictionary<string, string> {{"k1", "v1"}, {"k2", "v2"}}
                        }
                    });

            var modCompany = serviceService.Get(id);

            Assert.AreEqual(id, modCompany.Id);
            Assert.AreEqual(1, modCompany.TelemetryDataSinkSettings.Incoming.Count());
            Assert.AreEqual("test", modCompany.TelemetryDataSinkSettings.Incoming.First().SinkName);
            Assert.AreEqual(2, modCompany.TelemetryDataSinkSettings.Incoming.First().Parameters.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void UpdateCompanyIncomingTelemetryDataSinkSettingsNotAuthenticatedTest()
        {
            var id = _serviceService.Create(GetService());

            _authenticationContext.GetContextUser().Returns((string)null);

            _serviceService.UpdateIncomingTelemetryDataSinks(id, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void UpdateCompanyIncomingTelemetryDataSinkSettingsNotAuthorizedTest()
        {
            var id = _serviceService.Create(GetService());

            _authenticationContext.GetContextUser().Returns("12");

            _serviceService.UpdateIncomingTelemetryDataSinks(id, null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateCompanyIncomingTelemetryDataSinkSettingsNotFoundTest()
        {
            _serviceService.UpdateIncomingTelemetryDataSinks(Identity.Next(), null);
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
        }

        private ServiceDto GetService()
        {
            return new ServiceDto() { CompanyId = _companyId, Name = "new service" };
        }
    }
}
