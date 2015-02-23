using System.Configuration;

namespace IoT.Framework.DataAccess
{
    public interface IConnectionParametersResolver
    {
        string ManagementConnectionName { get; }

        string ManagementConnectionString { get; }

        string ManagementConnectionProvider { get; }
    }
}
