﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.TestHelpers;

namespace Thriot.Plugins.Sql.Tests
{
    [TestClass]
    public class AssemblyInitTest
    {
        [AssemblyInitialize]
        public static void AssemblyInitFunction(TestContext context)
        {
            EnvironmentFactoryFactory.Initialize(context.Properties);
        }
    }
}
