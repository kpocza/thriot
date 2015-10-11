namespace Thriot.TestHelpers
{
    public class PgSqlEnvironmentFactory : IEnvironmentFactory
    {
        public IManagementEnvironment ManagementEnvironment => new Environments.Management.PgSqlManagementEnvironment();

        public IMessagingEnvironment MessagingEnvironment => new Environments.Messaging.PgSqlMessagingEnvironment();

        public ITelemetryEnvironment TelemetryEnvironment => new Environments.Telemetry.PgSqlTelemetryEnvironment();

        public IQueueEnvironment QueueEnvironment => new Environments.Queue.PgSqlQueueEnvironment();
    }
} 