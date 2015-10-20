using System;
using System.Collections.Generic;
using Cassandra;
using Cassandra.Data.Linq;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Cassandra
{
    public class TelemetryDataSinkCurrent : TelemetryDataSinkBase, ITelemetryDataSinkCurrent
    {
        protected override void CreateTable()
        {
            _session.Execute($"CREATE TABLE IF NOT EXISTS \"{TableName}\" (\"DeviceId\" text PRIMARY KEY, \"Time\" timestamp, \"Data\" blob)");
        }

        public override void Record(TelemetryData message)
        {
            var insertStatement = _session.Prepare($"INSERT INTO \"{TableName}\" (\"DeviceId\", \"Time\", \"Data\") VALUES(?, ?, ?)");
            var boundStatement = insertStatement.Bind(message.DeviceId, message.Time, message.Payload);

            _session.Execute(boundStatement);
        }

        public IEnumerable<TelemetryData> GetCurrentData(IEnumerable<string> deviceIds)
        {
            throw new NotImplementedException();
        }
    }
}
