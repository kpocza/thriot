using System.Configuration;

namespace IoT.Framework.DataAccess
{
    public class LocalConnectionParametersResolver : IConnectionParametersResolver
    {
        public string ManagementConnectionName
        {
            get { return "ManagementConnection"; }
        }

        public string ManagementConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[ManagementConnectionName].ConnectionString; }
        }

        public string ManagementConnectionProvider
        {
            get { return ConfigurationManager.ConnectionStrings[ManagementConnectionName].ProviderName; }
        }
    }
}