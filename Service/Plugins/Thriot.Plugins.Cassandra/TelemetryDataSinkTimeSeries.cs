using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var deviceIdList = string.Join(",", deviceIds.Select(deviceId => $"'{deviceId}'"));

            var selectQuery =
                $"SELECT \"DeviceId\", \"Time\", \"Data\" FROM \"{TableName}\" WHERE \"Day\"='{date.Date.ToString("yyyyMMdd")}' AND \"DeviceId\" IN ({deviceIdList})";

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
