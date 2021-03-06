﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Management.Services;
using Thriot.TestHelpers;

namespace Thriot.Platform.Services.Messaging.Tests
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
