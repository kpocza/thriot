using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.DataAccess;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Azure
{
    public class TelemetryDataSinkTimeSeries : TelemetryDataSinkBase, ITelemetryDataSinkTimeSeries
    {
        public TelemetryDataSinkTimeSeries()
        {
        }

        public TelemetryDataSinkTimeSeries(IDynamicConnectionStringResolver dynamicConnectionStringResolver)
            : base(dynamicConnectionStringResolver)
        {
        }

        public override void Record(TelemetryData message)
        {
            var timeSeriesRepository = new GenericRepository<TimeSeriesTableEntity>(TableEntityOperation, TableName);

            var partitionKey = message.Time.ToString("yyyy-MM-dd") + "-" + message.DeviceId;
            var rowKey = message.Time.Ticks.ToString(CultureInfo.InvariantCulture);
            var partitionKeyRowKeyPair = new PartionKeyRowKeyPair(partitionKey, rowKey);

            var timeSeriesTableEntity = new TimeSeriesTableEntity(partitionKeyRowKeyPair, message.Payload);
            timeSeriesRepository.Create(timeSeriesTableEntity);
        }

        public IEnumerable<TelemetryData> GetTimeSeries(IEnumerable<string> deviceIds, DateTime date)
        {
            var list = new List<TelemetryData>();
            var timeSeriesRepository = new GenericRepository<TimeSeriesTableEntity>(TableEntityOperation, TableName);

            foreach (var deviceId in deviceIds)
            {
                var partitionKey = date.Date.ToString("yyyy-MM-dd") + "-" + deviceId;

                var tableData = timeSeriesRepository.QueryPartition(partitionKey);

                list.AddRange(tableData.Select(t => new TelemetryData(deviceId, t.Payload, new DateTime(long.Parse(t.RowKey)))));
            }

            return list;
        }
    }
}
