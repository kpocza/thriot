namespace Thriot.TestHelpers
{
    public class AzureEnvironmentFactory : IEnvironmentFactory
    {
        public IManagementEnvironment ManagementEnvironment => new Environments.Management.AzureManagementEnvironment();

        public IMessagingEnvironment MessagingEnvironment => new Environments.Messaging.SqlMessagingEnvironment();

        public ITelemetryEnvironment TelemetryEnvironment => new Environments.Telemetry.AzureTelemetryEnvironment();

        public IQueueEnvironment QueueEnvironment => new Environments.Queue.AzureQueueEnvironment();
    }
}