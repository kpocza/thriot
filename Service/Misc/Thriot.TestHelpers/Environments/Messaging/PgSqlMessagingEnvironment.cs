using Thriot.Messaging.Services.Client;

namespace Thriot.TestHelpers.Environments.Messaging
{
    public class PgSqlMessagingEnvironment : IMessagingEnvironment
    {
        public IMessagingServiceClient MessagingServiceClient => InprocMessagingServiceClient.PgSqlInstance;

        public string ConnectionStringResolverType => "Thriot.Messaging.Services.Tests.ConnectionStringResolverPgSql, Thriot.Messaging.Services.Tests";

        public string PersistentStorageType => "Thriot.Messaging.Services.Storage.PersistentStoragePgSql, Thriot.Messaging.Services";
    }
}
