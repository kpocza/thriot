﻿using IoT.Management.Services;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Plugins.Sql.Tests
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
