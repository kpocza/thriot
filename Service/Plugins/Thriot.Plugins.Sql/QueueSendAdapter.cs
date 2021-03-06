﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Sql
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
            var dataTable = new DataTable("EnqueueItemType");
            dataTable.Columns.Add("DeviceId", typeof (string));
            dataTable.Columns.Add("Payload", typeof (byte[]));
            dataTable.Columns.Add("RecordedAt", typeof (DateTime));

            var dataRow = dataTable.NewRow();
            dataRow[0] = telemetryData.DeviceId;
            dataRow[1] = Encoding.UTF8.GetBytes(telemetryData.Payload);
            dataRow[2] = telemetryData.Time;
            dataTable.Rows.Add(dataRow);

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand("[dbo].[Enqueue]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@EnqueueItems", dataTable));

                    var numRows = sqlCommand.ExecuteNonQuery();

                    if (numRows != 1)
                        throw new Exception("Error on enqueue");
                }
            }
        }

        public void Clear()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand("[dbo].[Clear]", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}