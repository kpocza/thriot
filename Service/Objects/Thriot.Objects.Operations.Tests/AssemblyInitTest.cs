using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Management.Services;
using Thriot.TestHelpers;

namespace Thriot.Objects.Operations.Tests
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
