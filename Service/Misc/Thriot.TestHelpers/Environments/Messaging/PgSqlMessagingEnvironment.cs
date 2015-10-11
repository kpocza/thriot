using Thriot.Messaging.Services.Client;

namespace Thriot.TestHelpers.Environments.Messaging
{
    public class PgSqlMessagingEnvironment : IMessagingEnvironment
    {
        public IMessagingServiceClient MessagingServiceClient => InprocMessagingServiceClient.PgSqlInstance;
    }
}
