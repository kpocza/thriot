using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Objects.Model;
using Thriot.TestHelpers;

namespace Thriot.Objects.Operations.Tests
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

