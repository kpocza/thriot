using System.Configuration;

namespace Thriot.Framework.DataAccess
{
    public interface IDynamicConnectionStringResolver
    {
        ConnectionStringSettings Resolve(string connectionName);
    }
}