using Thriot.Messaging.Services.Client;

namespace Thriot.TestHelpers.Environments.Messaging
{
    public class InMemoryMessagingEnvironment : IMessagingEnvironment
    {
        public IMessagingServiceClient MessagingServiceClient => InMemoryMessagingService.Instance;
    }
}
