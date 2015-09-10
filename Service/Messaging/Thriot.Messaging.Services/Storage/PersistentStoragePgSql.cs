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

                using (var command = new NpgsqlCommand("registerdevice", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("uid", deviceId);

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

                using (var command = new NpgsqlCommand("enqueue", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var jsonSource = items.Select(
                        item =>
                            new EnqueueData
                            {
                                deviceid = item.DeviceId,
                                payload = item.Payload,
                                timestamp = item.Timestamp,
                                senderuid = item.SenderDeviceId
                            }).ToList();
                    
                    var messages = new NpgsqlParameter("messagesjson", NpgsqlDbType.Json);
                    messages.Value = JsonConvert.SerializeObject(jsonSource);
                    command.Parameters.Add(messages);

                    using (var reader = command.ExecuteReader())
                    {

                        var colDeviceId = reader.GetOrdinal("deviceid");
                        var colDequeueIndex = reader.GetOrdinal("dequeueindex");
                        var colEnqueueIndex = reader.GetOrdinal("enqueueindex");
                        var colPeek = reader.GetOrdinal("peek");
                        var colVersion = reader.GetOrdinal("version");
                        var colMessageId = reader.GetOrdinal("messageid");

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
            return Retrieve("dequeue", deviceIds);
        }

        public DequeueResults Peek(IEnumerable<DeviceIdWithOpHint> deviceIds)
        {
            return Retrieve("peek", deviceIds);
        }

        public IReadOnlyCollection<DeviceEntry> Commit(IEnumerable<long> deviceIds)
        {
            if(!deviceIds.Any())
                return new List<DeviceEntry>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("commit", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var commitItemsJsonSource = deviceIds.Select(item => new CommitData {deviceid = item}).ToList();
                    var messages = new NpgsqlParameter("commititemsjson", NpgsqlDbType.Json);
                    messages.Value = JsonConvert.SerializeObject(commitItemsJsonSource);
                    command.Parameters.Add(messages);

                    var deviceEntries = new List<DeviceEntry>();

                    using (var reader = command.ExecuteReader())
                    {
                        var colDeviceId = reader.GetOrdinal("deviceid");
                        var colDequeueIndex = reader.GetOrdinal("dequeueindex");
                        var colEnqueueIndex = reader.GetOrdinal("enqueueindex");
                        var colPeek = reader.GetOrdinal("peek");
                        var colVersion = reader.GetOrdinal("version");

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

                    var dequeueItems = new NpgsqlParameter("dequeueitemsjson", NpgsqlDbType.Json);
                    dequeueItems.Value = JsonConvert.SerializeObject(dequeueItemsJsonSource);
                    command.Parameters.Add(dequeueItems);

                    var dequeueResults = new DequeueResults();
                    var messages = new List<DequeueResult>();
                    var unknownEntries = new List<DeviceEntry>();

                    using (var reader = command.ExecuteReader())
                    {
                        var colIsMessage = reader.GetOrdinal("ismessage");
                        var colDeviceId = reader.GetOrdinal("deviceid");
                        var colDequeueIndex = reader.GetOrdinal("dequeueindex");
                        var colEnqueueIndex = reader.GetOrdinal("enqueueindex");
                        var colPeek = reader.GetOrdinal("peek");
                        var colVersion = reader.GetOrdinal("version");
                        var colMessageId = reader.GetOrdinal("messageid");
                        var colPayload = reader.GetOrdinal("payload");
                        var colTimestamp = reader.GetOrdinal("timestamp");
                        var colSenderUid = reader.GetOrdinal("senderuid");

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
                                    dequeueResult.SenderDeviceId = (string)reader[colSenderUid];
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
            public string senderuid;
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
