﻿using System;
using System.Security.Principal;
using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Management.Dto;
using IoT.Management.Model;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Management.Services.Tests
{
    [TestClass]
    public class SingleCompanyTest
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

            var userId = _userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            Assert.AreEqual(userId, _settingProvider.UserForPrebuiltEntity);
            Assert.IsNotNull(_settingProvider.PrebuiltCompany);
            Assert.IsNull(_settingProvider.PrebuiltService);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void TryCreateAnotherCompanyTest()
        {
            var email = EmailHelper.Generate();

            _userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            _companyService.Create("Another Company");
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void TryDeleteCompanyTest()
        {
            var email = EmailHelper.Generate();

            _userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            _companyService.Delete(_settingProvider.PrebuiltCompany);
        }

        [TestMethod]
        public void CreateServiceTest()
        {
            var email = EmailHelper.Generate();

            _userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            var serviceId = _serviceService.Create(new ServiceDto { CompanyId = _settingProvider.PrebuiltCompany, Name = "Another service"});

            Assert.AreEqual(32, serviceId.Length);

            _serviceService.Delete(serviceId);
        }

        [TestMethod]
        public void RegisterUserEnsureEnvironmentNotTheSecondTest()
        {
            var email = EmailHelper.Generate();

            var userId = _userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            email = EmailHelper.Generate();
            var user2Id = _userService.Register(new RegisterDto() { Name = "new user", Email = email }, "password", null);

            Assert.AreEqual(userId, _settingProvider.UserForPrebuiltEntity);
            Assert.AreNotEqual(user2Id, _settingProvider.UserForPrebuiltEntity);
            Assert.IsNotNull(_settingProvider.PrebuiltCompany);
            Assert.IsNull(_settingProvider.PrebuiltService);
        }

        private void Initialize()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var authenticationContext = new TestAuthenticationContext();

            var userOperations = environmentFactory.MgmtUserOperations;
            var serviceOperations = environmentFactory.MgmtServiceOperations;
            var companyOperations = environmentFactory.MgmtCompanyOperations;
            var settingOperations = environmentFactory.MgmtSettingOperations;

            settingOperations.Update(new Setting(Setting.ServiceProfile, ServiceProfile.SingleCompany.ToString()));

            _settingProvider = new SettingProvider(settingOperations);

            _companyService = new CompanyService(companyOperations, authenticationContext, null, new CapabilityProvider(_settingProvider));
            _serviceService = new ServiceService(serviceOperations, companyOperations, authenticationContext, null,
                new CapabilityProvider(_settingProvider));
            var environmentPrebuilder = new EnvironmentPrebuilder(authenticationContext, _settingProvider, _companyService, null);

            _userService = new UserService(userOperations, authenticationContext, _settingProvider, environmentPrebuilder);
        }

        class TestAuthenticationContext : IAuthenticationContext
        {
            private string _userId;

            public IPrincipal GenerateContextUser(string userId)
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
