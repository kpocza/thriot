using System;
using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Cassandra
{
    public class TelemetryDataSinkTimeSeries : TelemetryDataSinkBase, ITelemetryDataSinkTimeSeries
    {
        protected override void CreateTable()
        {
            _session.Execute($"CREATE TABLE IF NOT EXISTS \"{TableName}\" (\"Day\" text, \"DeviceId\" text, \"Time\" timestamp, \"Data\" blob, PRIMARY KEY ((\"Day\", \"DeviceId\"), \"Time\"))");
        }

        public override void Record(TelemetryData message)
        {
            var insertStatement = _session.Prepare($"INSERT INTO \"{TableName}\" (\"Day\", \"DeviceId\", \"Time\", \"Data\") VALUES(?, ?, ?, ?)");
            var boundStatement = insertStatement.Bind(message.Time.Date.ToString("yyyyMMdd"), message.DeviceId, message.Time, message.Payload);

            _session.Execute(boundStatement);
        }

        public IEnumerable<TelemetryData> GetTimeSeries(IEnumerable<string> deviceIds, DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
