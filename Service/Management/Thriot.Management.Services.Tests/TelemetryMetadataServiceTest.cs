using System.Collections.Generic;
using System.Security.Authentication;
using Thriot.Management.Services.Dto;
using Thriot.Platform.Services.Client;
using Thriot.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Thriot.Management.Services.Tests
{
    [TestClass]
    public class TelemetryMetadataServiceTest
    {
        private IAuthenticationContext _authenticationContext;
        private string _userId;

        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void GetIncomingTest()
        {
            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();

            var msm = new TelemetryDataSinksMetadataDtoClient { Incoming = new List<TelemetryDataSinkMetadataDtoClient>() };

            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata().Returns(msm);

            var messageMetadataService = new TelemetryMetadataService(telemetryDataSinkSetupServiceClient, null, _authenticationContext);

            var incoming = messageMetadataService.GetIncomingTelemetryDataSinksMetadata();
            Assert.AreEqual(0, incoming.Incoming.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void GetIncomingNotAuthenticatedTest()
        {
            _authenticationContext.GetContextUser().Returns((string)null);

            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();

            var telemetryMetadataService = new TelemetryMetadataService(telemetryDataSinkSetupServiceClient, null, _authenticationContext);

            telemetryMetadataService.GetIncomingTelemetryDataSinksMetadata();
        }

        private void Initialize()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            _authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.ManagementEnvironment.MgmtSettingOperations);
            var userService = new UserService(userOperations, _authenticationContext, settingProvider, null);
            _userId = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);
        
            _authenticationContext.GetContextUser().Returns(_userId);
        }
    }
}
