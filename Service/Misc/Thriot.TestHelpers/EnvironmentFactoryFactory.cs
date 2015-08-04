using System;
using System.Configuration;

namespace Thriot.TestHelpers
{
    public static class EnvironmentFactoryFactory
    {
        public static IEnvironmentFactory Create()
        {
            return (IEnvironmentFactory)Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["IEnvironmentFactory"]));
        }
    }
}
