using System.Configuration;
using IoT.Messaging.Services.Storage;

namespace IoT.Messaging.WebApi
{
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["MessagingConnection"].ConnectionString; }
        }
    }
}