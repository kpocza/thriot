using System.Configuration;

namespace Thriot.Framework.DataAccess
{
    public interface IConnectionParametersResolver
    {
        string ManagementConnectionName { get; }

        string ManagementConnectionString { get; }

        string ManagementConnectionProvider { get; }
    }
}
