using Thriot.Messaging.Services.Storage;

namespace Thriot.Messaging.Services.Tests
{
    public class ConnectionStringResolverPgSql : IConnectionStringResolver
    {
        public string ConnectionString
        {
            get { return @"Server=127.0.0.1;Port=5432;Database=ThriotMessaging;User Id=thriotmessaging;Password=thriotmessaging;"; }
        }
    }
}
