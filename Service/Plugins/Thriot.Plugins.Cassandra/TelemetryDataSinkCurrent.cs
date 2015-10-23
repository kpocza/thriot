using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var boundStatement = insertStatement.Bind(message.DeviceId, message.Time, Encoding.UTF8.GetBytes(message.Payload));

            _session.Execute(boundStatement);
        }

        public IEnumerable<TelemetryData> GetCurrentData(IEnumerable<string> deviceIds)
        {
            var deviceIdList = string.Join(",", deviceIds.Select(deviceId => $"'{deviceId}'"));

            var selectQuery =
                $"SELECT \"DeviceId\", \"Time\", \"Data\" FROM \"{TableName}\" WHERE \"DeviceId\" IN ({deviceIdList})";

            var list = new List<TelemetryData>();

            var rowSet = _session.Execute(selectQuery);
            
            foreach (var row in rowSet)
            {
                list.Add(new TelemetryData((string)row["DeviceId"], Encoding.UTF8.GetString((byte[])row["Data"]), ((DateTimeOffset)row["Time"]).DateTime));
            }

            return list;
        }
    }
}
