namespace Thriot.TestHelpers
{
    public class SqlEnvironmentFactory : IEnvironmentFactory
    {
        public IManagementEnvironment ManagementEnvironment => new Environments.Management.SqlManagementEnvironment();

        public IMessagingEnvironment MessagingEnvironment => new Environments.Messaging.SqlMessagingEnvironment();

        public ITelemetryEnvironment TelemetryEnvironment => new Environments.Telemetry.SqlTelemetryEnvironment();

        public IQueueEnvironment QueueEnvironment => new Environments.Queue.SqlQueueEnvironment();

    }
} 