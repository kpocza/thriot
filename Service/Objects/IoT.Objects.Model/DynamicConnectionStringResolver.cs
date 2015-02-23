using System.Collections.Concurrent;
using System.Configuration;
using IoT.Framework.DataAccess;
using IoT.Objects.Model.Operations;

namespace IoT.Objects.Model
{
    public class DynamicConnectionStringResolver : IDynamicConnectionStringResolver
    {
        private readonly ISettingOperations _settingOperations;
        private readonly ConcurrentDictionary<string, Setting> _connectionStringSettingsList;

        public DynamicConnectionStringResolver(ISettingOperations settingOperations)
        {
            _settingOperations = settingOperations;
            _connectionStringSettingsList = new ConcurrentDictionary<string, Setting>();
        }

        public ConnectionStringSettings Resolve(string connectionName)
        {
            var setting = _connectionStringSettingsList.GetOrAdd(connectionName, connName => _settingOperations.Get(SettingId.GetConnection(connName)));

            return new ConnectionStringSettings(connectionName, setting.Value);
        }
    }
}
