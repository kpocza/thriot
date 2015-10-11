namespace Thriot.TestHelpers
{
    public interface IEnvironmentFactory
    {
        IManagementEnvironment ManagementEnvironment { get; }

        IMessagingEnvironment MessagingEnvironment { get; }

        ITelemetryEnvironment TelemetryEnvironment { get; }

        IQueueEnvironment QueueEnvironment { get; }
    }
}
