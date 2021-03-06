﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.TestHelpers;

namespace Thriot.Management.Services.Tests
{
    [TestClass]
    public class AssemblyInit
    {
        [AssemblyInitialize]
        public static void AssemblyInitFunction(TestContext context)
        {
            EnvironmentFactoryFactory.Initialize(context.Properties);

            DtoMapper.Setup();

            SettingInitializer.Init();
        }
    }
}
