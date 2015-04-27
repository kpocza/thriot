using System.Configuration;
using Thriot.Messaging.Services.Storage;

namespace Thriot.Messaging.WebApi
{
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["MessagingConnection"].ConnectionString; }
        }
    }
}