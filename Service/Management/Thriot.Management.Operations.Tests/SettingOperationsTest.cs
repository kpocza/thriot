using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Management.Model;
using Thriot.TestHelpers;

namespace Thriot.Management.Operations.Tests
{
    [TestClass]
    public class SettingOperationsTest
    {
        [TestMethod]
        public void UpdateSettingTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var settingOperations = environmentFactory.ManagementEnvironment.MgmtSettingOperations;

            var settingId = new SettingId(Identity.Next(), Identity.Next());

            settingOperations.Create(new Setting(settingId, "value"));

            var newSetting = settingOperations.Get(settingId);

            Assert.AreEqual(settingId.ToString(), newSetting.Id.ToString());
            Assert.AreEqual("value", newSetting.Value);

            newSetting.Value += "mod";
            settingOperations.Update(newSetting);

            var modSetting = settingOperations.Get(settingId);

            Assert.AreEqual(settingId.ToString(), modSetting.Id.ToString());
            Assert.AreEqual("valuemod", modSetting.Value);
        }
    }
}

