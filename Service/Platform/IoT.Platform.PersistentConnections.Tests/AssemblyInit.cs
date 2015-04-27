﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Management.Services;
using Thriot.TestHelpers;

namespace Thriot.Platform.PersistentConnections.Tests
{
    [TestClass]
    public class AssemblyInit
    {
        [AssemblyInitialize]
        public static void AssemblyInitFunction(TestContext context)
        {
            DtoMapper.Setup();

            SettingInitializer.Init();
        }
    }
}
