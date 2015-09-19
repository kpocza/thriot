using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
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
            AssemblyResolver.Initialize();
            DtoMapper.Setup();

#if INTEGRATIONTEST
            SettingInitializer.Init();
#endif
        }
    }
}
