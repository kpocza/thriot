using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using IoT.Framework.DataAccess;
using IoT.Plugins.Core;

namespace IoT.Plugins.Sql
{
    public class TelemetryDataSinkTimeSeries : TelemetryDataSinkBase, ITelemetryDataSinkTimeSeries
    {
        public TelemetryDataSinkTimeSeries()
        {
        }

        public TelemetryDataSinkTimeSeries(IDynamicConnectionStringResolver dynamicConnectionStringResolver)
            : base(dynamicConnectionStringResolver)
        {
        }

        protected override string CreateTableStatement
        {
            get
            {
                return string.Join(Environment.NewLine,
                    string.Format("CREATE TABLE [{0}](", TableName),
                        "[Date] date NOT NULL,",
                        "[DeviceId] varchar(32) NOT NULL,",
                        "[Time] datetime2 NOT NULL,",
                        "[Payload] nvarchar(1024) NOT NULL,",
                        string.Format("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([Date], [DeviceId], [Time]))", TableName));
            }
        }

        public override void Record(TelemetryData message)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (
                    var insert =
                        new SqlCommand(
                            string.Format(
                                "INSERT INTO [{0}]([Date], [DeviceId], [Time], [Payload]) VALUES(@date, @deviceId, @time, @payload)",
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
                    catch (SqlException)
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

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                for (var idx = 0; idx < allDeviceIds.Count; idx += 500)
                {
                    var queryNow = allDeviceIds.Skip(idx).Take(500);
                    var concatenatedDeviceIds = string.Join(",", queryNow.Select(q => "'" + q + "'"));
                    using (var select = new SqlCommand(string.Format("SELECT [DeviceId], [Time], [Payload] FROM [{0}](nolock) WHERE [Date] = @date AND DeviceId IN ({1})", TableName, concatenatedDeviceIds), sqlConnection))
                    {
                        select.Parameters.AddWithValue("date", date.Date);

                        var dataReader = select.ExecuteReader();

                        while (dataReader.Read())
                        {
                            list.Add(new TelemetryData((string)dataReader[0], (string)dataReader[2],
                                (DateTime)dataReader[1]));
                        }
                    }
                }
            }

            return list;
        }
    }
}
