﻿using System.Collections.Generic;
using System.Security.Authentication;
using Thriot.Framework;
using Thriot.Management.Dto;
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
            var telemetryDataSinkSetupService = Substitute.For<ITelemetryDataSinkSetupService>();

            var msm = new TelemetryDataSinksMetadataDto { Incoming = new List<TelemetryDataSinkMetadataDto>() };

            telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata().Returns(msm);

            var messageMetadataService = new TelemetryMetadataService(telemetryDataSinkSetupService, null, _authenticationContext);

            var incoming = messageMetadataService.GetIncomingTelemetryDataSinksMetadata();
            Assert.AreEqual(0, incoming.Incoming.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void GetIncomingNotAuthenticatedTest()
        {
            _authenticationContext.GetContextUser().Returns((string)null);

            var telemetryDataSinkSetupService = Substitute.For<ITelemetryDataSinkSetupService>();

            var telemetryMetadataService = new TelemetryMetadataService(telemetryDataSinkSetupService, null, _authenticationContext);

            telemetryMetadataService.GetIncomingTelemetryDataSinksMetadata();
        }

        private void Initialize()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            _authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);
            var userService = new UserService(userOperations, _authenticationContext, settingProvider, null);
            _userId = userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate() }, "password", null);
        
            _authenticationContext.GetContextUser().Returns(_userId);
        }
    }
}