using System.Configuration;

namespace IoT.Framework.DataAccess
{
    public interface IDynamicConnectionStringResolver
    {
        ConnectionStringSettings Resolve(string connectionName);
    }
}