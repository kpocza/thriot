using Thriot.Messaging.Services.Storage;

namespace Thriot.Messaging.Services.Tests
{
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        public string ConnectionString
        {
            get { return @"Server=.\SQLEXPRESS;Database=ThriotMessaging;Trusted_Connection=True;"; }
        }
    }
}
