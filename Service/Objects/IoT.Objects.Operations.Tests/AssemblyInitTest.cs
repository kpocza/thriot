using IoT.Management.Services;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Objects.Operations.Tests
{
    [TestClass]
    public class AssemblyInitTest
    {
        [AssemblyInitialize]
        public static void AssemblyInitFunction(TestContext context)
        {
            DtoMapper.Setup();

            SettingInitializer.Init();
        }
    }
}
