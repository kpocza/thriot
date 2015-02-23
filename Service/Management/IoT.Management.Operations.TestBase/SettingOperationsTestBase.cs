using IoT.Framework;
using IoT.Management.Model;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Management.Operations.TestBase
{
    public abstract class SettingOperationsTestBase
    {
        public virtual void UpdateSettingTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var settingOperations = environmentFactory.MgmtSettingOperations;

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

