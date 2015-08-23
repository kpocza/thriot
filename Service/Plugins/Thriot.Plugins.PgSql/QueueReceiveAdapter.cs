using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Thriot.Plugins.Core;
using Npgsql;
using NpgsqlTypes;

namespace Thriot.Plugins.PgSql
{
    public class QueueReceiveAdapter : SerialQueueReceiveAdapter
    {
        private string _connectionString;

        public override void Setup(IDictionary<string, string> parameters)
        {
            _connectionString = parameters["ConnectionString"];
        }

        protected override IEnumerable<QueueItem> DequeueItemsCore(int maxDequeueCount, int expirationMinutes)
        {
            var items = new List<QueueItem>();

            using (var sqlConnection = new NpgsqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new NpgsqlCommand("Dequeue", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.AddWithValue("Count", maxDequeueCount);
                    sqlCommand.Parameters.AddWithValue("ExpiredMins", expirationMinutes);

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        var colId = reader.GetOrdinal("Id");
                        var colDeviceId = reader.GetOrdinal("DeviceId");
                        var colData = reader.GetOrdinal("Payload");
                        var colRecordedAt = reader.GetOrdinal("RecordedAt");

                        while (reader.Read())
                        {
                            var queueItem = new QueueItem(
                                (long)reader[colId], 
                                new TelemetryData(
                                    (string)reader[colDeviceId],
                                    Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.UTF8.GetString((byte[])reader[colData]))), 
                                    (DateTime)reader[colRecordedAt]));

                            items.Add(queueItem);
                        }
                    }
                }
            }

            return items;
        }

        protected override void CommitItemsCore(IEnumerable<QueueItem> items)
        {
            var commitItemsJsonSource = items.Select(item => new CommitItemType { id = item.Id }).ToList();


            using (var sqlConnection = new NpgsqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new NpgsqlCommand("Commit", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    var messages = new NpgsqlParameter("CommitItemsJson", NpgsqlDbType.Json);
                    messages.Value = JsonConvert.SerializeObject(commitItemsJsonSource);
                    sqlCommand.Parameters.Add(messages);

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        class CommitItemType
        {
            public long id;
        }
    }
}
