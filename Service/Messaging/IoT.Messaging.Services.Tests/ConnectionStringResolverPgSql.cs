using IoT.Messaging.Services.Storage;

namespace IoT.Messaging.Services.Tests
{
    public class ConnectionStringResolverPgSql : IConnectionStringResolver
    {
        public string ConnectionString
        {
            get { return @"Server=127.0.0.1;Port=5432;Database=IoTMessaging;User Id=postgres;Password=postgres;"; }
        }
    }
}
