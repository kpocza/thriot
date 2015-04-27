using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Thriot.Messaging.Services.Storage
{
    public class PersistentStorage : IPersistentStorage
    {
        private readonly string _connectionString;

        public PersistentStorage(IConnectionStringResolver connectionStringResolver)
        {
            _connectionString = connectionStringResolver.ConnectionString;
        }

        public long InitializeDevice(string deviceId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand("RegisterDevice", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@Uid", deviceId));
                    var outParam = new SqlParameter("@DeviceId", SqlDbType.BigInt);
                    outParam.Direction = ParameterDirection.Output;
                    sqlCommand.Parameters.Add(outParam);

                    sqlCommand.ExecuteNonQuery();

                    return (long)outParam.Value;
                }
            }
        }

        public IReadOnlyCollection<EnqueueResult> Enqueue(IEnumerable<EnqueueItem> items)
        {
            if(!items.Any())
                return new List<EnqueueResult>();

            var dataTable = new DataTable("EnqueueItemTable");
            dataTable.Columns.Add("DeviceId", typeof(long));
            dataTable.Columns.Add("Payload", typeof(byte[]));
            dataTable.Columns.Add("Timestamp", typeof(DateTime));

            foreach (var item in items)
            {
                var dataRow = dataTable.NewRow();
                dataRow[0] = item.DeviceId;
                dataRow[1] = item.Payload;
                dataRow[2] = item.Timestamp;
                dataTable.Rows.Add(dataRow);
            }

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand("Enqueue", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@Messages", dataTable));

                    var enqueueResults = new List<EnqueueResult>();

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        var colDeviceId = reader.GetOrdinal("DeviceId");
                        var colDequeueIndex = reader.GetOrdinal("DequeueIndex");
                        var colEnqueueIndex = reader.GetOrdinal("EnqueueIndex");
                        var colPeek = reader.GetOrdinal("Peek");
                        var colVersion = reader.GetOrdinal("Version");
                        var colMessageId = reader.GetOrdinal("MessageId");

                        while (reader.Read())
                        {
                            var enqueueResult = new EnqueueResult
                            {
                                Id = (long) reader[colDeviceId],
                                DequeueIndex = (int) reader[colDequeueIndex],
                                EnqueueIndex = (int) reader[colEnqueueIndex],
                                Peek = (bool) reader[colPeek],
                                Version = (int) reader[colVersion],
                                MessageId = (int) reader[colMessageId]
                            };

                            enqueueResults.Add(enqueueResult);
                        }
                    }

                    return enqueueResults;
                }
            }
        }

        public DequeueResults Dequeue(IEnumerable<DeviceIdWithOpHint> deviceIds)
        {
            return Retrieve("Dequeue", deviceIds);
        }

        public DequeueResults Peek(IEnumerable<DeviceIdWithOpHint> deviceIds)
        {
            return Retrieve("Peek", deviceIds);
        }

        public IReadOnlyCollection<DeviceEntry> Commit(IEnumerable<long> deviceIds)
        {
            if(!deviceIds.Any())
                return new List<DeviceEntry>();

            var dataTable = new DataTable("DeviceIdTable");
            dataTable.Columns.Add("DeviceId", typeof(long));

            foreach (var deviceId in deviceIds)
            {
                var dataRow = dataTable.NewRow();
                dataRow[0] = deviceId;
                dataTable.Rows.Add(dataRow);
            }

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand("Commit", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@CommitItems", dataTable));

                    var deviceEntries = new List<DeviceEntry>();

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        var colDeviceId = reader.GetOrdinal("DeviceId");
                        var colDequeueIndex = reader.GetOrdinal("DequeueIndex");
                        var colEnqueueIndex = reader.GetOrdinal("EnqueueIndex");
                        var colPeek = reader.GetOrdinal("Peek");
                        var colVersion = reader.GetOrdinal("Version");

                        while (reader.Read())
                        {
                            var deviceEntry = new DeviceEntry
                            {
                                Id = (long) reader[colDeviceId],
                                DequeueIndex = (int) reader[colDequeueIndex],
                                EnqueueIndex = (int) reader[colEnqueueIndex],
                                Peek = (bool) reader[colPeek],
                                Version = (int) reader[colVersion]
                            };

                            deviceEntries.Add(deviceEntry);
                        }
                    }

                    return deviceEntries;
                }
            }
        }

        private DequeueResults Retrieve(string spName, IEnumerable<DeviceIdWithOpHint> deviceIds)
        {
            if (!deviceIds.Any())
                return new DequeueResults { Messages = new List<DequeueResult>(), UnknownEntries = new List<DeviceEntry>()};
            
            var dataTable = new DataTable("DeviceIdWithIndexTable");
            dataTable.Columns.Add("DeviceId", typeof(long));
            dataTable.Columns.Add("Index", typeof(int));

            foreach (var item in deviceIds)
            {
                var dataRow = dataTable.NewRow();
                dataRow[0] = item.DeviceId;
                dataRow[1] = item.Index ?? -1; // just to avoid NULL check problems in SQL
                dataTable.Rows.Add(dataRow);
            }

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand(spName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@DequeueItems", dataTable));

                    var dequeueResults = new DequeueResults();
                    var messages = new List<DequeueResult>();
                    var unknownEntries = new List<DeviceEntry>();

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        var colIsMessage = reader.GetOrdinal("IsMessage");
                        var colDeviceId = reader.GetOrdinal("DeviceId");
                        var colDequeueIndex = reader.GetOrdinal("DequeueIndex");
                        var colEnqueueIndex = reader.GetOrdinal("EnqueueIndex");
                        var colPeek = reader.GetOrdinal("Peek");
                        var colVersion = reader.GetOrdinal("Version");
                        var colMessageId = reader.GetOrdinal("MessageId");
                        var colPayload = reader.GetOrdinal("Payload");
                        var colTimestamp = reader.GetOrdinal("Timestamp");

                        while (reader.Read())
                        {
                            if ((int) reader[colIsMessage] == 1)
                            {
                                var dequeueResult = new DequeueResult
                                {
                                    Id = (long) reader[colDeviceId],
                                    DequeueIndex = (int) reader[colDequeueIndex],
                                    EnqueueIndex = (int) reader[colEnqueueIndex],
                                    Peek = (bool) reader[colPeek],
                                    Version = (int) reader[colVersion],
                                    MessageId = (int) reader[colMessageId]
                                };
                                if (!reader.IsDBNull(colPayload))
                                {
                                    dequeueResult.Payload = (byte[]) reader[colPayload];
                                    dequeueResult.Timestamp = (DateTime) reader[colTimestamp];
                                }

                                messages.Add(dequeueResult);
                            }
                            else
                            {
                                var deviceEntry = new DeviceEntry
                                {
                                    Id = (long) reader[colDeviceId],
                                    DequeueIndex = (int) reader[colDequeueIndex],
                                    EnqueueIndex = (int) reader[colEnqueueIndex],
                                    Peek = (bool) reader[colPeek],
                                    Version = (int) reader[colVersion]
                                };

                                unknownEntries.Add(deviceEntry);
                            }
                        }
                    }

                    dequeueResults.Messages = messages;
                    dequeueResults.UnknownEntries = unknownEntries;

                    return dequeueResults;
                }
            }
        }
    }
}
