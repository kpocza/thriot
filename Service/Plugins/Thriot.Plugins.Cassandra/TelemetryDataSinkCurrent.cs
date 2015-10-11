using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public IEnumerable<TelemetryData> GetCurrentData(IEnumerable<string> deviceIds)
        {
            throw new NotImplementedException();
        }
    }
}
