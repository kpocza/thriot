using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Management.Services;
using Thriot.TestHelpers;

namespace Thriot.Plugins.Sql.Tests
{
    [TestClass]
    public class AssemblyInitTest
    {
        [AssemblyInitialize]
        public static void AssemblyInitFunction(TestContext context)
        {
            DtoMapper.Setup();

#if INTEGRATIONTEST
            SettingInitializer.Init();
#endif
        }
    }
}
