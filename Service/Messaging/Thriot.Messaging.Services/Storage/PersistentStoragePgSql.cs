using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace Thriot.Messaging.Services.Storage
{
    public class PersistentStoragePgSql : IPersistentStorage
    {
        private readonly string _connectionString;

        public PersistentStoragePgSql(IConnectionStringResolver connectionStringResolver)
        {
            _connectionString = connectionStringResolver.ConnectionString;
        }

        public long InitializeDevice(string deviceId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("RegisterDevice", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Uid", deviceId);

                    return (long)command.ExecuteScalar();
                }
            }
        }

        public IReadOnlyCollection<EnqueueResult> Enqueue(IEnumerable<EnqueueItem> items)
        {
            if(!items.Any())
                return new List<EnqueueResult>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("Enqueue", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var jsonSource = items.Select(
                        item =>
                            new EnqueueData
                            {
                                deviceid = item.DeviceId,
                                payload = item.Payload,
                                timestamp = item.Timestamp
                            }).ToList();
                    
                    var messages = new NpgsqlParameter("MessagesJson", NpgsqlDbType.Json);
                    messages.Value = JsonConvert.SerializeObject(jsonSource);
                    command.Parameters.Add(messages);

                    using (var reader = command.ExecuteReader())
                    {

                        var colDeviceId = reader.GetOrdinal("DeviceId");
                        var colDequeueIndex = reader.GetOrdinal("DequeueIndex");
                        var colEnqueueIndex = reader.GetOrdinal("EnqueueIndex");
                        var colPeek = reader.GetOrdinal("Peek");
                        var colVersion = reader.GetOrdinal("Version");
                        var colMessageId = reader.GetOrdinal("MessageId");

                        var enqueueResults = new List<EnqueueResult>();

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

                        return enqueueResults;
                    }
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

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("Commit", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var commitItemsJsonSource = deviceIds.Select(item => new CommitData {deviceid = item}).ToList();
                    var messages = new NpgsqlParameter("CommitItemsJson", NpgsqlDbType.Json);
                    messages.Value = JsonConvert.SerializeObject(commitItemsJsonSource);
                    command.Parameters.Add(messages);

                    var deviceEntries = new List<DeviceEntry>();

                    using (var reader = command.ExecuteReader())
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

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var dequeueItemsJsonSource =
                        deviceIds.Select(item => new RetrieveData {deviceid = item.DeviceId, index = item.Index ?? -1}).ToList();

                    var dequeueItems = new NpgsqlParameter("DequeueItemsJson", NpgsqlDbType.Json);
                    dequeueItems.Value = JsonConvert.SerializeObject(dequeueItemsJsonSource);
                    command.Parameters.Add(dequeueItems);

                    var dequeueResults = new DequeueResults();
                    var messages = new List<DequeueResult>();
                    var unknownEntries = new List<DeviceEntry>();

                    using (var reader = command.ExecuteReader())
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
                            if ((bool) reader[colIsMessage])
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
                                    dequeueResult.Payload = Convert.FromBase64String(Encoding.UTF8.GetString((byte[]) reader[colPayload]));
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

        class EnqueueData
        {
            public long deviceid;
            public byte[] payload;
            public DateTime timestamp;
        }

        class RetrieveData
        {
            public long deviceid;
            public int index;
        }

        class CommitData
        {
            public long deviceid;
        }
    }
}
