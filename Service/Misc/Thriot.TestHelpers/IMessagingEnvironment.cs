using Thriot.Messaging.Services.Client;

namespace Thriot.TestHelpers
{
    public interface IMessagingEnvironment
    {
        IMessagingServiceClient MessagingServiceClient { get; }

        string ConnectionStringResolverType { get; }

        string PersistentStorageType { get; }
    }
}
