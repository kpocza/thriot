using Thriot.Messaging.Services.Storage;

namespace Thriot.Messaging.PerformanceTest
{
    internal class ConnectionStringResolver : IConnectionStringResolver
    {
        public string ConnectionString
        {
            get { return @"Server=.\SQLEXPRESS;Database=ThriotMessaging;Trusted_Connection=True;"; }
        }
    }
}