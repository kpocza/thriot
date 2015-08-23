using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Sql
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

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand("[dbo].[Dequeue]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.AddWithValue("@Count", maxDequeueCount);
                    sqlCommand.Parameters.AddWithValue("@ExpiredMins", expirationMinutes);

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
                                    Encoding.UTF8.GetString((byte[])reader[colData]), 
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
            var dataTable = new DataTable("CommitItemType");
            dataTable.Columns.Add("Id", typeof(long));

            foreach (var item in items)
            {
                var dataRow = dataTable.NewRow();
                dataRow[0] = item.Id;
                dataTable.Rows.Add(dataRow);
            }

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand("[dbo].[Commit]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@CommitItems", dataTable));

                    var numRows = sqlCommand.ExecuteNonQuery();

                    if (numRows != items.Count())
                        throw new Exception("Error on commit");
                }
            }
        }
    }
}
