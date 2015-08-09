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
    public class CompanyServiceTest
    {
        private IAuthenticationContext _authenticationContext;
        private string _userId;
        private CompanyService _companyService;
        private ICompanyOperations _companyOperations;
        private UserService _userService;

        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void CreateCompanyTest()
        {
            var id = _companyService.Create("new company");

            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void CreateCompanyNotAuthenticatedTest()
        {
            _authenticationContext.GetContextUser().Returns((string)null);

            _companyService.Create("new company");
        }

        [TestMethod]
        public void GetCompanyTest()
        {
            var id = _companyService.Create("new company");

            var newCompany = _companyService.Get(id);

            Assert.AreEqual(id, newCompany.Id);
            Assert.AreEqual("new company", newCompany.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void GetCompanyNotAuthenticatedTest()
        {
            var id = _companyService.Create("new company");

            _authenticationContext.GetContextUser().Returns((string)null);

            _companyService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void GetCompanyNotAuthorizedTest()
        {
            var id = _companyService.Create("new company");

            _authenticationContext.GetContextUser().Returns("12");

            _companyService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void GetCompanyNotFoundTest()
        {
            _companyService.Get(Identity.Next());
        }

        [TestMethod]
        public void UpdateCompanyTest()
        {
            var id = _companyService.Create("new company");

            _companyService.Update(new CompanyDto() {Id= id, Name = "modified"});

            var modCompany = _companyService.Get(id);

            Assert.AreEqual(id, modCompany.Id);
            Assert.AreEqual("modified", modCompany.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void UpdateCompanyNotAuthenticatedTest()
        {
            var id = _companyService.Create("new company");

            _authenticationContext.GetContextUser().Returns((string)null);

            _companyService.Update(new CompanyDto() { Id = id, Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void UpdateCompanyNotAuthorizedTest()
        {
            var id = _companyService.Create("new company");

            _authenticationContext.GetContextUser().Returns("12");

            _companyService.Update(new CompanyDto() { Id = id, Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateCompanyNotFoundTest()
        {
            _companyService.Update(new CompanyDto() { Id = Identity.Next(), Name = "modified" });
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void DeleteCompanyTest()
        {
            var id = _companyService.Create("new company");

            _companyService.Delete(id);

            _companyService.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void DeleteCompanyNotAuthenticatedTest()
        {
            var id = _companyService.Create("new company");

            _authenticationContext.GetContextUser().Returns((string)null);

            _companyService.Delete(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void DeleteCompanyNotAuthorizedTest()
        {
            var id = _companyService.Create("new company");

            _authenticationContext.GetContextUser().Returns("12");

            _companyService.Delete(id);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void DeleteCompanyNotFoundTest()
        {
            _companyService.Delete(Identity.Next());
        }

        [TestMethod]
        public void ListUsersTest()
        {
            var id = _companyService.Create("new company");

            var users = _companyService.ListUsers(id);

            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(_userId, users[0].Id);
            Assert.AreEqual("user", users[0].Name);
            Assert.IsTrue(users[0].Email.EndsWith("@gmail.com"));
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void ListUsersNotAuthenticatedTest()
        {
            _authenticationContext.GetContextUser().Returns((string)null);

            _companyService.ListUsers("123");
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void ListUsersNotAuthorizedTest()
        {
            var id = _companyService.Create("new company");

            _authenticationContext.GetContextUser().Returns("123");

            _companyService.ListUsers(id);
        }

        [TestMethod]
        public void UpdateCompanyIncomingTelemetryDataSinkSettingsTest()
        {
            var id = _companyService.Create("new company");

            var telemetryDataSinkSetupServiceClient = Substitute.For<ITelemetryDataSinkSetupServiceClient>();
            telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata().Returns(
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

            var companyService = new CompanyService(_companyOperations, _authenticationContext, telemetryDataSinkSetupServiceClient, null);

            companyService.UpdateIncomingTelemetryDataSinks(id, new List<TelemetryDataSinkParametersDto>
                    {
                        new TelemetryDataSinkParametersDto
                        {
                            SinkName = "test",
                            Parameters = new Dictionary<string, string> {{"k1", "v1"}, {"k2", "v2"}}
                        }
                    });

            var modCompany = companyService.Get(id);

            Assert.AreEqual(id, modCompany.Id);
            Assert.AreEqual(1, modCompany.TelemetryDataSinkSettings.Incoming.Count());
            Assert.AreEqual("test", modCompany.TelemetryDataSinkSettings.Incoming.First().SinkName);
            Assert.AreEqual(2, modCompany.TelemetryDataSinkSettings.Incoming.First().Parameters.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void UpdateCompanyIncomingTelemetryDataSinkSettingsNotAuthenticatedTest()
        {
            var id = _companyService.Create("new company");

            _authenticationContext.GetContextUser().Returns((string)null);

            _companyService.UpdateIncomingTelemetryDataSinks(id, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void UpdateCompanyIncomingTelemetryDataSinkSettingsNotAuthorizedTest()
        {
            var id = _companyService.Create("new company");

            _authenticationContext.GetContextUser().Returns("12");

            _companyService.UpdateIncomingTelemetryDataSinks(id, null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateCompanyIncomingTelemetryDataSinkSettingsNotFoundTest()
        {
            _companyService.UpdateIncomingTelemetryDataSinks(Identity.Next(), null);
        }

        [TestMethod]
        public void AddUserTest()
        {
            var companyId = _companyService.Create("new company");

            var user2Id = _userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);
            
            _companyService.AddUser(new CompanyUserDto {CompanyId = companyId, UserId = user2Id});

            var users = _companyService.ListUsers(companyId);
            Assert.AreEqual(2, users.Count);

            _companyService.AddUser(new CompanyUserDto { CompanyId = companyId, UserId = user2Id });

            users = _companyService.ListUsers(companyId);
            Assert.AreEqual(2, users.Count);

            _authenticationContext.GetContextUser().Returns(user2Id);

            var companies = _userService.ListCompanies();
            Assert.AreEqual(1, companies.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void AddUserNoAccessToCompanyTest()
        {
            var companyId = _companyService.Create("new company");

            var user2Id = _userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            _authenticationContext.GetContextUser().Returns(user2Id);

            _companyService.AddUser(new CompanyUserDto { CompanyId = companyId, UserId = user2Id });
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void AddUserNotAuthenticatedTest()
        {
            var companyId = _companyService.Create("new company");

            _authenticationContext.GetContextUser().Returns((string)null);

            _companyService.AddUser(new CompanyUserDto { CompanyId = companyId, UserId = "12345" });
        }

        private void Initialize()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            _authenticationContext = Substitute.For<IAuthenticationContext>();

            var userOperations = environmentFactory.MgmtUserOperations;
            _companyOperations = environmentFactory.MgmtCompanyOperations;
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);
            _userService = new UserService(userOperations, _authenticationContext, settingProvider, null);

            _userId = _userService.Register(new RegisterDto() { Name = "user", Email = EmailHelper.Generate(), Password = "password" }, null);

            _companyService = new CompanyService(_companyOperations, _authenticationContext, null, new CapabilityProvider(settingProvider));
        
            _authenticationContext.GetContextUser().Returns(_userId);
        }
    }
}
