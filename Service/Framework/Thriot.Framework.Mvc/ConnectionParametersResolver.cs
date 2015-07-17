using Microsoft.Framework.Configuration;
using Thriot.Framework.DataAccess;

namespace Thriot.Framework.Mvc
{
    public class ConnectionParametersResolver : IConnectionParametersResolver
    {
        private readonly IConfiguration _configuration;

        public ConnectionParametersResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ManagementConnectionName
        {
            get { return "ManagementConnection"; }
        }

        public string ManagementConnectionString
        {
            get { return _configuration.Get($"ConnectionString:{ManagementConnectionName}:ConnectionString"); }
        }

        public string ManagementConnectionProvider
        {
            get { return _configuration.Get($"ConnectionString:{ManagementConnectionName}:ProviderName"); }
        }
    }
}