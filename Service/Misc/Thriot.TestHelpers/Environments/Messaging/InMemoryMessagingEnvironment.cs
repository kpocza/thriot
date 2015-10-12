using Thriot.Messaging.Services.Client;

namespace Thriot.TestHelpers.Environments.Messaging
{
    public class InMemoryMessagingEnvironment : IMessagingEnvironment
    {
        public IMessagingServiceClient MessagingServiceClient => InMemoryMessagingService.Instance;

        public string ConnectionStringResolverType => null;

        public string PersistentStorageType => "Thriot.Messaging.Services.Storage.PersistentStorageStub, Thriot.Messaging.Services";
    }
}
