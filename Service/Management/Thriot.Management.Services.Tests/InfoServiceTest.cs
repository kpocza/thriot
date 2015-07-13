using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Framework;
using Thriot.TestHelpers;

namespace Thriot.Management.Services.Tests
{
    [TestClass]
    public class InfoServiceTest
    {
        [TestMethod]
        public void GetInfoTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns("12345");

            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);
            var capabilityProvider = new CapabilityProvider(settingProvider);

            var infoService = new InfoService(authenticationContext, settingProvider, capabilityProvider);

            var info = infoService.Get();

            Assert.AreEqual("ServiceProvider", info.ServiceProfile);
            Assert.IsNull(info.PrebuiltCompany);
            Assert.IsNull(info.PrebuiltService);
            Assert.IsTrue(info.CanCreateCompany);
            Assert.IsTrue(info.CanDeleteCompany);
            Assert.IsTrue(info.CanCreateService);
            Assert.IsTrue(info.CanDeleteService);
        }

        [TestMethod]
        public void GetUrlInfoTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var authenticationContext = Substitute.For<IAuthenticationContext>();
            authenticationContext.GetContextUser().Returns("12345");

            var settingProvider = new SettingProvider(environmentFactory.MgmtSettingOperations);

            var infoService = new InfoService(authenticationContext, settingProvider, null);

            var info = infoService.GetUrlInfo();

            Assert.IsNotNull(info.WebsiteUrl);
            Assert.IsNotNull(info.ManagementApiUrl);
            Assert.IsNotNull(info.PlatformApiUrl);
            Assert.IsNotNull(info.PlatformWsUrl);
            Assert.IsNotNull(info.ReportingApiUrl);
        }
    }
}
