using Microsoft.Framework.Configuration;

namespace Thriot.Framework.DataAccess
{
    public class ConnectionParametersResolver : IConnectionParametersResolver
    {
        private readonly IConfiguration _configuration;

        public ConnectionParametersResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ManagementConnectionName => "ManagementConnection";

        public string ManagementConnectionString => _configuration.Get($"ConnectionString:{ManagementConnectionName}:ConnectionString");

        public string ManagementConnectionProvider => _configuration.Get($"ConnectionString:{ManagementConnectionName}:ProviderName");
    }
}