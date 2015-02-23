﻿using IoT.Framework;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IoT.Management.Services.Tests
{
    [TestClass]
    public class SettingProviderTest
    {
        [TestCleanup]
        public void CleanUp()
        {
            SettingInitializer.RemoveExtraEntries();
        }

        [TestMethod]
        public void GetServiceProfileTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var serviceProfile = settingProvider.ServiceProfile;

            Assert.AreEqual(ServiceProfile.ServiceProvider, serviceProfile);
        }

        [TestMethod]
        public void GetEmailActivationTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var emailActivation = settingProvider.EmailActivation;

            Assert.IsFalse(emailActivation);
        }

        [TestMethod]
        public void GetPrebuiltCompanyTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            Assert.IsNull(settingProvider.PrebuiltCompany);

            settingProvider.PrebuiltCompany = "aaa";

            Assert.AreEqual("aaa", settingProvider.PrebuiltCompany);
        }

        [TestMethod]
        public void GetPrebuiltServiceTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            Assert.IsNull(settingProvider.PrebuiltService);

            settingProvider.PrebuiltService = "aaa";

            Assert.AreEqual("aaa", settingProvider.PrebuiltService);
        }

        [TestMethod]
        public void GetUserForPrebuiltEntityTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            Assert.IsNull(settingProvider.UserForPrebuiltEntity);

            settingProvider.UserForPrebuiltEntity = "aaa";

            Assert.AreEqual("aaa", settingProvider.UserForPrebuiltEntity);
        }

        [TestMethod]
        public void CacheTest()
        {
            var settingOperations = Substitute.For<ISettingOperations>();
            settingOperations.Get(Setting.ServiceProfile)
                .Returns(new Setting(Setting.ServiceProfile,ServiceProfile.ServiceProvider.ToString()));

            var settingProvider = new SettingProvider(settingOperations);

            var serviceProfile = settingProvider.ServiceProfile;
            Assert.AreEqual(ServiceProfile.ServiceProvider, serviceProfile);

            settingOperations.Received(1).Get(Setting.ServiceProfile);

            serviceProfile = settingProvider.ServiceProfile;
            Assert.AreEqual(ServiceProfile.ServiceProvider, serviceProfile);

            settingOperations.Received(1).Get(Setting.ServiceProfile);
        }
    }
}
