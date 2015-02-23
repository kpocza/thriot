using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Platform.Services.Telemetry.Tests
{
    [TestClass]
    public class AssemblyInit
    {
        [AssemblyInitialize]
        public static void AssemblyInitFunction(TestContext context)
        {
            Management.Services.DtoMapper.Setup();
            DtoMapper.Setup();

            SettingInitializer.Init();
        }
    }
}
