using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Framework;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.ServiceClient.TelemetrySetup;
using Thriot.TestHelpers;
using TelemetryDataSinkMetadataDto = Thriot.ServiceClient.TelemetrySetup.TelemetryDataSinkMetadataDto;
using TelemetryDataSinksMetadataDto = Thriot.ServiceClient.TelemetrySetup.TelemetryDataSinksMetadataDto;
using TelemetrySetup = Thriot.ServiceClient.TelemetrySetup;

namespace Thriot.Objects.Operations.Tests
{
    [TestClass]
    public class ServiceOperationsTest
    {
        private IAuthenticationContext _authenticationContext;
        private string _companyId;
        private ServiceService _serviceService;

        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void GetServiceTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();

            var service1 = new ServiceDto() { CompanyId = _companyId, Name = "new service1", ApiKey = Identity.Next() };
            var service2 = new ServiceDto() { CompanyId = _companyId, Name = "new service2", ApiKey = Identity.Next() };

            var service1Id = _serviceService.Create(service1);
            var service2Id = _serviceService.Create(service2);

            var incoming =
                new List<TelemetryDataSinkParametersDto>
                {
                    new TelemetryDataSinkParametersDto
                    {
                        SinkName = "test",
                        Parameters = new Dictionary<string, string> {{"k1", "v1"}, {"k2", "v2"}}
                    }
                };

            _serviceService.UpdateIncomingTelemetryDataSinks(service1Id, incoming);

            var platformServiceOperations = environmentFactory.ObjServiceOperations;

            var s1 = platformServiceOperations.Get(service1Id);
            var s2 = platformServiceOperations.Get(service2Id);

            Assert.AreEqual(service1Id, s1.Id);
            Assert.AreEqual(_companyId, s1.CompanyId);
            Assert.AreEqual(32, s1.ApiKey.Length);
            Assert.AreEqual(1, s1.TelemetryDataSinkSettings.Incoming.Count());
            Assert.AreEqual("test", s1.TelemetryDataSinkSettings.Incoming.First().SinkName);
            Assert.AreEqual(2, s1.TelemetryDataSinkSettings.Incoming.First().Parameters.Count); Assert.AreEqual(service2Id, s2.Id);
            Assert.AreEqual(_companyId, s2.CompanyId);
            Assert.AreEqual(32, s2.ApiKey.Length);
            Assert.IsNull(s2.TelemetryDataSinkSettings.Incoming);
        }

        protected void Initialize()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            _authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var userService = new UserService(userOperations, _authenticationContext, settingProvider, null);
            var userId = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            var companyService = new CompanyService(companyOperations, _authenticationContext, null, new CapabilityProvider(settingProvider));

            _authenticationContext.GetContextUser().Returns(userId);

            _companyId = companyService.Create("new company");

            var serviceOperations = environmentFactory.MgmtServiceOperations;
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
            _serviceService = new ServiceService(serviceOperations, companyOperations, _authenticationContext, telemetryDataSinkSetupService, new CapabilityProvider(settingProvider));
        }
    }
}

