using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Management.Services.Dto;
using Thriot.Management.Services;
using Thriot.Platform.Services.Client;
using Thriot.TestHelpers;

namespace Thriot.Objects.Operations.Tests
{
    [TestClass]
    public class CompanyOperationsTest
    {
        private IAuthenticationContext _authenticationContext;
        private CompanyService _companyService;
        private Management.Model.Operations.ICompanyOperations _companyOperations;

        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void GetCompanyTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();

            var company1Id = _companyService.Create("new company1");
            var company2Id = _companyService.Create("new company2");

            var incoming =
                new List<TelemetryDataSinkParametersDto>
                {
                    new TelemetryDataSinkParametersDto
                    {
                        SinkName = "test",
                        Parameters = new Dictionary<string, string> {{"k1", "v1"}, {"k2", "v2"}}
                    }
                };

            _companyService.UpdateIncomingTelemetryDataSinks(company1Id, incoming);

            var platformCompanyOperations = environmentFactory.ManagementEnvironment.ObjCompanyOperations;

            var c1 = platformCompanyOperations.Get(company1Id);
            var c2 = platformCompanyOperations.Get(company2Id);

            Assert.AreEqual(company1Id, c1.Id);
            Assert.AreEqual(1, c1.TelemetryDataSinkSettings.Incoming.Count());
            Assert.AreEqual("test", c1.TelemetryDataSinkSettings.Incoming.First().SinkName);
            Assert.AreEqual(2, c1.TelemetryDataSinkSettings.Incoming.First().Parameters.Count);

            Assert.AreEqual(company2Id, c2.Id);
            Assert.IsNull(c2.TelemetryDataSinkSettings.Incoming);
        }

        protected void Initialize()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            _authenticationContext = Substitute.For<IAuthenticationContext>();

            var settingProvider = new SettingProvider(environmentFactory.ManagementEnvironment.MgmtSettingOperations);
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            _companyOperations = environmentFactory.ManagementEnvironment.MgmtCompanyOperations;
            var userService = new UserService(userOperations, _authenticationContext, settingProvider, null);

            var userId = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();
            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata().Returns(
                new TelemetryDataSinksMetadataDtoClient
                {
                    Incoming = new List<TelemetryDataSinkMetadataDtoClient>
                    {
                        new TelemetryDataSinkMetadataDtoClient
                        {
                            Name = "test",
                            Description = null,
                            ParametersToInput = new List<string> {"k1", "k2"}
                        }
                    }
                });

            _companyService = new CompanyService(_companyOperations, _authenticationContext, telemetryDataSinkSetupServiceClient, new CapabilityProvider(settingProvider));

            _authenticationContext.GetContextUser().Returns(userId);
        }
    }
}

