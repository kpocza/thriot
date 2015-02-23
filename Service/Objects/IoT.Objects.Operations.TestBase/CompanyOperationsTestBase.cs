using System.Collections.Generic;
using System.Linq;
using IoT.Framework;
using IoT.Management.Dto;
using IoT.Management.Services;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TelemetrySetup = IoT.ServiceClient.TelemetrySetup;

namespace IoT.Objects.Operations.TestBase
{
    public abstract class CompanyOperationsTestBase
    {
        private IAuthenticationContext _authenticationContext;
        private CompanyService _companyService;
        private Management.Model.Operations.ICompanyOperations _companyOperations;

        public virtual void GetCompanyTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();

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

            var platformCompanyOperations = environmentFactory.ObjCompanyOperations;

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
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            _authenticationContext = Substitute.For<IAuthenticationContext>();

            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);
            var userOperations = environmentFactory.MgmtUserOperations;
            _companyOperations = environmentFactory.MgmtCompanyOperations;
            var userService = new UserService(userOperations, _authenticationContext, settingProvider, null);

            var userId = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate() }, "password", null);

            var telemetryDataSinkSetupService = Substitute.For<TelemetrySetup.ITelemetryDataSinkSetupService>();
            telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata().Returns(
                new TelemetrySetup.TelemetryDataSinksMetadataDto
                {
                    Incoming = new List<TelemetrySetup.TelemetryDataSinkMetadataDto>
                    {
                        new TelemetrySetup.TelemetryDataSinkMetadataDto
                        {
                            Name = "test",
                            Description = null,
                            ParametersToInput = new List<string> {"k1", "k2"}
                        }
                    }
                });

            _companyService = new CompanyService(_companyOperations, _authenticationContext, telemetryDataSinkSetupService, new CapabilityProvider(settingProvider));

            _authenticationContext.GetContextUser().Returns(userId);
        }
    }
}

