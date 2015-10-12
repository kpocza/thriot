using System;
using System.Collections;
using System.Linq;

namespace Thriot.TestHelpers
{
    public static class EnvironmentFactoryFactory
    {
        private static string _management;
        private static string _messaging;
        private static string _telemetry;
        private static string _queue;

        public static void Initialize(IDictionary properties)
        {
            _management = (string)properties["Management"];
            _messaging = (string)properties["Messaging"];
            _telemetry = (string)properties["Telemetry"];
            _queue = (string)properties["Queue"];
        }

        public static EnvironmentFactory Create()
        {
            return new EnvironmentFactory
            (
                GetValue<IManagementEnvironment>("Management", _management),
                GetValue<IMessagingEnvironment>("Messaging", _messaging),
                GetValue<ITelemetryEnvironment>("Telemetry", _telemetry),
                GetValue<IQueueEnvironment>("Queue", _queue)
            );
        }

        private static T GetValue<T>(string module, string name)
        {
            if (name == null)
                name = "InMemory";

            var type = typeof (EnvironmentFactoryFactory).Assembly.GetTypes()
                .Single(t => t.FullName == $"Thriot.TestHelpers.Environments.{module}.{name}{module}Environment");

            return (T) Activator.CreateInstance(type);
        }
    }
}
