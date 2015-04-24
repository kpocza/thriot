using IoT.Messaging.Services.Storage;

namespace IoT.Messaging.Services.Tests
{
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        public string ConnectionString
        {
            get { return @"Server=.\SQLEXPRESS;Database=ThriotMessaging;Trusted_Connection=True;"; }
        }
    }
}
