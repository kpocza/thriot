using System;
using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Cassandra
{
    public class TelemetryDataSinkTimeSeries : TelemetryDataSinkBase, ITelemetryDataSinkTimeSeries
    {
        protected override void CreateTable()
        {
            _session.Execute($"CREATE TABLE IF NOT EXISTS \"{TableName}\" (\"Day\" date, \"DeviceId\" text, \"Time\" timestamp, \"Data\" blob, PRIMARY KEY ((\"Day\", \"DeviceId\"), \"Time\"))");
        }

        public override void Record(TelemetryData message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TelemetryData> GetTimeSeries(IEnumerable<string> deviceIds, DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
