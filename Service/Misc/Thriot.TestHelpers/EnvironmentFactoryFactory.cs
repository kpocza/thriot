using Thriot.Framework;

namespace Thriot.TestHelpers
{
    public static class EnvironmentFactoryFactory
    {
        public static IEnvironmentFactory Create()
        {
            return SingleContainer.Instance.Resolve<IEnvironmentFactory>();
        }
    }
}
