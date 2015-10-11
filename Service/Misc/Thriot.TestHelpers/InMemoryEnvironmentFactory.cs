namespace Thriot.TestHelpers
{
    public class InMemoryEnvironmentFactory : IEnvironmentFactory
    {
        public IManagementEnvironment ManagementEnvironment => new Environments.Management.InMemoryManagementEnvironment();

        public IMessagingEnvironment MessagingEnvironment => new Environments.Messaging.InMemoryMessagingEnvironment();

        public ITelemetryEnvironment TelemetryEnvironment => new Environments.Telemetry.InMemoryTelemetryEnvironment();

        public IQueueEnvironment QueueEnvironment => new Environments.Queue.InMemoryQueueEnvironment();
    }
}