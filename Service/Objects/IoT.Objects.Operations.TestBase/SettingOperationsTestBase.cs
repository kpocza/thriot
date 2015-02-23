using IoT.Framework;
using IoT.Objects.Model;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Objects.Operations.TestBase
{
    public abstract class SettingOperationsTestBase
    {
        public virtual void GetSettingTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var setting = environmentFactory.ObjSettingOperations.Get(Setting.TelemetrySetupServiceApiKey);

            Assert.AreEqual(32, setting.Value.Length);
        }
    }
}

