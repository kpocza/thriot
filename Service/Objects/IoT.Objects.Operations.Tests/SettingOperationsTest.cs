using IoT.Framework;
using IoT.Objects.Model;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Objects.Operations.Tests
{
    [TestClass]
    public class SettingOperationsTest
    {
        [TestMethod]
        public void GetSettingTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var setting = environmentFactory.ObjSettingOperations.Get(Setting.TelemetrySetupServiceApiKey);

            Assert.AreEqual(32, setting.Value.Length);
        }
    }
}

