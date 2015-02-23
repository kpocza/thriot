using IoT.Messaging.Services.Storage;

namespace IoT.Messaging.PerformanceTest
{
    internal class ConnectionStringResolver : IConnectionStringResolver
    {
        public string ConnectionString
        {
            get { return @"Server=.\SQLEXPRESS;Database=IoTMessaging;Trusted_Connection=True;"; }
        }
    }
}