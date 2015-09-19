using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.TestHelpers;

namespace Thriot.Platform.Services.Telemetry.Tests
{
    [TestClass]
    public class AssemblyInit
    {
        [AssemblyInitialize]
        public static void AssemblyInitFunction(TestContext context)
        {
            AssemblyResolver.Initialize();

            Management.Services.DtoMapper.Setup();
            DtoMapper.Setup();

            SettingInitializer.Init();
        }
    }
}
