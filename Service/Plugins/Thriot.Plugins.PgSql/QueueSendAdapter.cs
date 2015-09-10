using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Newtonsoft.Json;
using Thriot.Plugins.Core;
using Npgsql;
using NpgsqlTypes;

namespace Thriot.Plugins.PgSql
{
    public class QueueSendAdapter : IQueueSendAdapter
    {
        private string _connectionString;

        public void Setup(IDictionary<string, string> parameters)
        {
            _connectionString = parameters["ConnectionString"];
        }

        public void Send(TelemetryData telemetryData)
        {
            var jsonSource = new[]
            {
                new EnqueueItemType
                {
                    deviceid = telemetryData.DeviceId,
                    payload = Encoding.UTF8.GetBytes(telemetryData.Payload),
                    recordedat = telemetryData.Time
                }
            };

            using (var sqlConnection = new NpgsqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new NpgsqlCommand("enqueue", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    var enqueueItems = new NpgsqlParameter("enqueueitemsjson", NpgsqlDbType.Json);
                    enqueueItems.Value = JsonConvert.SerializeObject(jsonSource);
                    sqlCommand.Parameters.Add(enqueueItems);

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void Clear()
        {
            using (var sqlConnection = new NpgsqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new NpgsqlCommand("clear", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        class EnqueueItemType
        {
            public string deviceid;
            public byte[] payload;
            public DateTime recordedat;
        }

    }
}