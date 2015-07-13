using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.PgSql
{
    public class TelemetryDataSinkCurrent : TelemetryDataSinkBase, ITelemetryDataSinkCurrent
    {
        protected override string CreateTableStatement
        {
            get
            {
                return string.Join(Environment.NewLine,
                    string.Format("CREATE TABLE IF NOT EXISTS \"{0}\"(", TableName),
                        "\"DeviceId\" varchar(32) NOT NULL PRIMARY KEY,",
                        "\"Time\" timestamp NOT NULL,",
                        "\"Payload\" varchar(1024) NOT NULL)");
            }
        }

        public override void Record(TelemetryData message)
        {
            using (var sqlConnection = new NpgsqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                int rowsAffected = 0;
                using (
                    var update =
                        new NpgsqlCommand(
                            string.Format(
                                "UPDATE \"{0}\" SET \"Time\" = @time, \"Payload\" = @payload WHERE \"DeviceId\" = @deviceId",
                                TableName), sqlConnection))
                {
                    update.Parameters.AddWithValue("deviceId", message.DeviceId);
                    update.Parameters.AddWithValue("time", message.Time);
                    update.Parameters.AddWithValue("payload", message.Payload);

                    rowsAffected = update.ExecuteNonQuery();
                }

                if (rowsAffected != 1)
                {
                    using (
                        var insert =
                            new NpgsqlCommand(
                                string.Format(
                                    "INSERT INTO \"{0}\"(\"DeviceId\", \"Time\", \"Payload\") VALUES(@deviceId, @time, @payload)",
                                    TableName), sqlConnection))
                    {
                        insert.Parameters.AddWithValue("deviceId", message.DeviceId);
                        insert.Parameters.AddWithValue("time", message.Time);
                        insert.Parameters.AddWithValue("payload", message.Payload);

                        rowsAffected = insert.ExecuteNonQuery();
                        if(rowsAffected!= 1)
                            throw new InvalidOperationException("Unable to add data");
                    }
                }
            }
        }

        public IEnumerable<TelemetryData> GetCurrentData(IEnumerable<string> deviceIds)
        {
            var list = new List<TelemetryData>();
            var allDeviceIds = new List<string>(deviceIds);
            allDeviceIds.Sort();

            using (var sqlConnection = new NpgsqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                for (var idx = 0; idx < allDeviceIds.Count; idx += 500)
                {
                    var queryNow = allDeviceIds.Skip(idx).Take(500);
                    var concatenatedDeviceIds = string.Join(",", queryNow.Select(q => "'" + q + "'"));
                    using (var select = new NpgsqlCommand(string.Format("SELECT \"DeviceId\", \"Time\", \"Payload\" FROM \"{0}\" WHERE \"DeviceId\" IN ({1})", TableName, concatenatedDeviceIds), sqlConnection))
                    {
                        var dataReader = select.ExecuteReader();

                        while (dataReader.Read())
                        {
                            list.Add(new TelemetryData((string) dataReader[0], (string) dataReader[2],
                                (DateTime) dataReader[1]));
                        }
                    }
                }
            }

            return list;
        }
    }
}
