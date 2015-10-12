using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.TestHelpers;

namespace Thriot.Management.Operations.Tests
{
    [TestClass]
    public class AssemblyInit
    {
        public static void AssemblyInitFunction(TestContext context)
        {
            EnvironmentFactoryFactory.Initialize(context.Properties);
        }
    }
}
