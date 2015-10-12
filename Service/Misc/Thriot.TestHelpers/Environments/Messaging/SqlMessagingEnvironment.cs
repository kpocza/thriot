using Thriot.Messaging.Services.Client;

namespace Thriot.TestHelpers.Environments.Messaging
{
    public class SqlMessagingEnvironment : IMessagingEnvironment
    {
        public IMessagingServiceClient MessagingServiceClient => InprocMessagingServiceClient.SqlInstance;

        public string ConnectionStringResolverType => "Thriot.Messaging.Services.Tests.ConnectionStringResolver, Thriot.Messaging.Services.Tests";

        public string PersistentStorageType => "Thriot.Messaging.Services.Storage.PersistentStorage, Thriot.Messaging.Services";
    }
}
