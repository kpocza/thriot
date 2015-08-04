using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Npgsql;
using Thriot.Framework.DataAccess;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.PgSql
{
    public class TelemetryDataSinkTimeSeries : TelemetryDataSinkBase, ITelemetryDataSinkTimeSeries
    {
        protected override string CreateTableStatement
        {
            get
            {
                return string.Join(Environment.NewLine,
                    string.Format("CREATE TABLE IF NOT EXISTS \"{0}\"(", TableName),
                        "\"Date\" date NOT NULL,",
                        "\"DeviceId\" varchar(32) NOT NULL,",
                        "\"Time\" timestamp NOT NULL,",
                        "\"Payload\" varchar(1024) NOT NULL,",
                        string.Format("PRIMARY KEY (\"Date\", \"DeviceId\", \"Time\"))", TableName));
            }
        }

        public override void Record(TelemetryData message)
        {
            using (var sqlConnection = new NpgsqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (
                    var insert =
                        new NpgsqlCommand(
                            string.Format(
                                "INSERT INTO \"{0}\"(\"Date\", \"DeviceId\", \"Time\", \"Payload\") VALUES(@date, @deviceId, @time, @payload)",
                                TableName), sqlConnection))
                {
                    insert.Parameters.AddWithValue("date", message.Time.Date);
                    insert.Parameters.AddWithValue("deviceId", message.DeviceId);
                    insert.Parameters.AddWithValue("time", message.Time);
                    insert.Parameters.AddWithValue("payload", message.Payload);

                    try
                    {
                        var rowsAffected = insert.ExecuteNonQuery();
                        if (rowsAffected != 1)
                            throw new InvalidOperationException("Unable to add data");
                    }
                    catch (NpgsqlException)
                    {
                        throw new StorageAccessException(HttpStatusCode.Conflict);
                    }
                }
            }
        }

        public IEnumerable<TelemetryData> GetTimeSeries(IEnumerable<string> deviceIds, DateTime date)
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
                    var datePart = date.Date;
                    var dateString = $"{datePart.ToString("yyyy")}.{datePart.ToString("MM")}.{datePart.ToString("dd")}";
                    using (var select = new NpgsqlCommand(
                        $"SELECT \"DeviceId\", \"Time\", \"Payload\" FROM \"{TableName}\" WHERE \"Date\" = '{dateString}' AND \"DeviceId\" IN ({concatenatedDeviceIds})", sqlConnection))
                    {
                        select.Parameters.AddWithValue("date", date.Date);

                        var dataReader = select.ExecuteReader();

                        while (dataReader.Read())
                        {
                            list.Add(new TelemetryData((string)dataReader[0], (string)dataReader[2],
                                // WORKAROUND: timezone handling in npgsql
                                ((DateTime)dataReader[1]).ToUniversalTime()));
                        }
                    }
                }
            }

            return list;
        }
    }
}
