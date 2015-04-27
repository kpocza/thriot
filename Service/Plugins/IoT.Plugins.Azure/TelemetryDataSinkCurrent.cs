using System.Collections.Generic;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.DataAccess;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Azure
{
    public class TelemetryDataSinkCurrent : TelemetryDataSinkBase, ITelemetryDataSinkCurrent
    {
        public TelemetryDataSinkCurrent()
        {
        }

        public TelemetryDataSinkCurrent(IDynamicConnectionStringResolver dynamicConnectionStringResolver) : base(dynamicConnectionStringResolver)
        {
        }

        public override void Record(TelemetryData message)
        {
            var currentDataRepository = new GenericRepository<CurrentDataTableEntity>(TableEntityOperation, TableName);

            var partitionKeyRowKeyPair = PartionKeyRowKeyPair.CreateFromIdentity(message.DeviceId);

            var currentDataTableEntity = new CurrentDataTableEntity(partitionKeyRowKeyPair, message.Time, message.Payload);
            currentDataRepository.Upsert(currentDataTableEntity);
        }

        public IEnumerable<TelemetryData> GetCurrentData(IEnumerable<string> deviceIds)
        {
            var list = new List<TelemetryData>();
            var currentDataRepository = new GenericRepository<CurrentDataTableEntity>(TableEntityOperation, TableName);

            foreach (var deviceId in deviceIds)
            {
                var partitionKeyRowKeyPair = PartionKeyRowKeyPair.CreateFromIdentity(deviceId);
                var currentDataTableEntity = currentDataRepository.Get(partitionKeyRowKeyPair);

                if (currentDataTableEntity != null)
                {
                    list.Add(new TelemetryData(deviceId, currentDataTableEntity.Payload, currentDataTableEntity.Time));
                }
            }

            return list;
        }
    }
}
