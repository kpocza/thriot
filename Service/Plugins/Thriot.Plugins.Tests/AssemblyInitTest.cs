using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Management.Services;
using Thriot.TestHelpers;

namespace Thriot.Plugins.Tests
{
    [TestClass]
    public class AssemblyInitTest
    {
        [AssemblyInitialize]
        public static void AssemblyInitFunction(TestContext context)
        {
            EnvironmentFactoryFactory.Initialize(context.Properties);

            AssemblyResolver.Initialize();
            DtoMapper.Setup();

            var environmentFactory = EnvironmentFactoryFactory.Create();
            if (environmentFactory.TelemetryEnvironment.TelemetryDataSinkCurrent != null)
            {
                SettingInitializer.Init();
            }
        }
    }
}
