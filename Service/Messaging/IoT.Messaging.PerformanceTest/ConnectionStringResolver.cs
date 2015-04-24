using IoT.Messaging.Services.Storage;

namespace IoT.Messaging.PerformanceTest
{
    internal class ConnectionStringResolver : IConnectionStringResolver
    {
        public string ConnectionString
        {
            get { return @"Server=.\SQLEXPRESS;Database=ThriotMessaging;Trusted_Connection=True;"; }
        }
    }
}