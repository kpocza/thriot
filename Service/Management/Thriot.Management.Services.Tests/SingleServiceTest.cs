using System;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Dto;
using Thriot.Management.Model;
using Thriot.TestHelpers;

namespace Thriot.Management.Services.Tests
{
    [TestClass]
    public class SingleServiceTest
    {
        private SettingProvider _settingProvider;
        private UserService _userService;
        private CompanyService _companyService;
        private ServiceService _serviceService;

        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            SettingInitializer.RemoveExtraEntries();
        }

        [TestMethod]
        public void RegisterUserEnsureEnvironmentTest()
        {
            var email = EmailHelper.Generate();

            var userId = _userService.Register(new RegisterDto() { Name = "new user", Email = email, Password = "password" }, null);

            Assert.AreEqual(userId, _settingProvider.UserForPrebuiltEntity);
            Assert.IsNotNull(_settingProvider.PrebuiltCompany);
            Assert.IsNotNull(_settingProvider.PrebuiltService);
        }

        [TestMethod]
        public void RegisterUserEnsureEnvironmentNotTheSecondTest()
        {
            var email = EmailHelper.Generate();

            var userId = _userService.Register(new RegisterDto() { Name = "new user", Email = email, Password = "password" }, null);

            email = EmailHelper.Generate();
            var user2Id = _userService.Register(new RegisterDto() { Name = "new user", Email = email, Password = "password" }, null);

            Assert.AreEqual(userId, _settingProvider.UserForPrebuiltEntity);
            Assert.AreNotEqual(user2Id, _settingProvider.UserForPrebuiltEntity);
            Assert.IsNotNull(_settingProvider.PrebuiltCompany);
            Assert.IsNotNull(_settingProvider.PrebuiltService);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void TryCreateAnotherCompanyTest()
        {
            var email = EmailHelper.Generate();

            _userService.Register(new RegisterDto() { Name = "new user", Email = email, Password = "password" }, null);

            _companyService.Create("Another Company");
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void CreateServiceTest()
        {
            var email = EmailHelper.Generate();

            _userService.Register(new RegisterDto() { Name = "new user", Email = email, Password = "password" }, null);

            _serviceService.Create(new ServiceDto { CompanyId = _settingProvider.PrebuiltCompany, Name = "Another service" });
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void TryDeleteServiceTest()
        {
            var email = EmailHelper.Generate();

            _userService.Register(new RegisterDto() { Name = "new user", Email = email, Password = "password" }, null);

            _serviceService.Delete(_settingProvider.PrebuiltService);
        }

        private void Initialize()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = new TestAuthenticationContext();

            var userOperations = environmentFactory.MgmtUserOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var settingOperations = environmentFactory.MgmtSettingOperations;

            settingOperations.Update(new Setting(Setting.ServiceProfile, ServiceProfile.SingleService.ToString()));

            _settingProvider = new SettingProvider(settingOperations);

            _companyService = new CompanyService(companyOperations, authenticationContext, null, new CapabilityProvider(_settingProvider));
            _serviceService = new ServiceService(serviceOperations, companyOperations, authenticationContext, null, new CapabilityProvider(_settingProvider));
            var environmentPrebuilder = new EnvironmentPrebuilder(authenticationContext, _settingProvider, _companyService, _serviceService);

            _userService = new UserService(userOperations, authenticationContext, _settingProvider, environmentPrebuilder);
        }

        class TestAuthenticationContext : IAuthenticationContext
        {
            private string _userId;

            public void SetUserPrincipalContext(IUserPrincipalContext userPrincipalContext)
            {
                throw new NotImplementedException();
            }

            public IPrincipal BuildContextUserPrincipal(string userId)
            {
                throw new System.NotImplementedException();
            }

            public void SetContextUser(string userId)
            {
                _userId = userId;
            }

            public void RemoveContextUser()
            {
                throw new System.NotImplementedException();
            }

            public string GetContextUser()
            {
                return _userId;
            }
        }
    }
}
