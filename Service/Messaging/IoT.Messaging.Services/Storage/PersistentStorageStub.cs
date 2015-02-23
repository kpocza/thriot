using System;
using System.Collections.Generic;

namespace IoT.Messaging.Services.Storage
{
    public class PersistentStorageStub : IPersistentStorage
    {
        private const int DefaultQueueSize = 100;
        private static long _maxId;
        private static readonly object _lock;

        private static readonly IDictionary<long, DeviceMessagingMeta> _messagingMetas;
        private static readonly IDictionary<long, DeviceQueueEntry[]> _queueEntries;

        static PersistentStorageStub()
        {
            _messagingMetas = new Dictionary<long, DeviceMessagingMeta>();
            _queueEntries = new Dictionary<long, DeviceQueueEntry[]>();
            _lock = new object();
            _maxId = 1;
        }

        public long InitializeDevice(string deviceId)
        {
            lock (_lock)
            {
                long id = _maxId;
                _maxId++;

                _messagingMetas.Add(id, new DeviceMessagingMeta
                {
                    Id = id,
                    EnqueueIndex = 0,
                    DequeueIndex = 0,
                    Peek = false,
                    QueueSize = DefaultQueueSize,
                    Version = 0
                });

                var queueEntries = new DeviceQueueEntry[DefaultQueueSize];
                for (int i = 0; i < DefaultQueueSize; i++)
                {
                    queueEntries[i] = new DeviceQueueEntry
                    {
                        Id = id,
                        Index = i,
                        Payload = null,
                        Timestamp = DateTime.UtcNow
                    };
                }

                _queueEntries.Add(id, queueEntries);

                return id;
            }
        }

        public IReadOnlyCollection<EnqueueResult> Enqueue(IEnumerable<EnqueueItem> items)
        {
            lock (_lock)
            {
                var result = new List<EnqueueResult>();
                foreach (var item in items)
                {
                    var meta = _messagingMetas[item.DeviceId];
                    int enqueueIdx = meta.EnqueueIndex % meta.QueueSize;

                    var queueEntry = _queueEntries[item.DeviceId][enqueueIdx];
                    queueEntry.Payload = item.Payload;
                    queueEntry.Timestamp = item.Timestamp;

                    var resultItem = new EnqueueResult
                    {
                        Id = item.DeviceId,
                        MessageId = meta.EnqueueIndex,
                    };

                    meta.Version++;
                    meta.EnqueueIndex++;
                    if (meta.DequeueIndex + meta.QueueSize <= meta.EnqueueIndex)
                    {
                        meta.DequeueIndex++;
                        meta.Peek = false;
                    }

                    resultItem.DequeueIndex = meta.DequeueIndex;
                    resultItem.EnqueueIndex = meta.EnqueueIndex;
                    resultItem.Peek = meta.Peek;
                    resultItem.Version = meta.Version;

                    result.Add(resultItem);
                }

                return result;
            }
        }

        public DequeueResults Dequeue(IEnumerable<DeviceIdWithOpHint> deviceIds)
        {
            var dequeueResults = new DequeueResults();
            var messages = new List<DequeueResult>();
            var unknwoEtnries = new List<DequeueResult>(); 
            
            lock (_lock)
            {
                foreach (var deviceId in deviceIds)
                {
                    var meta = _messagingMetas[deviceId.DeviceId];
                    if (meta.DequeueIndex < meta.EnqueueIndex)
                    {
                        int dequeueIdx = meta.DequeueIndex % meta.QueueSize;
                        var queueEntry = _queueEntries[deviceId.DeviceId][dequeueIdx];

                        var dequeueItem = new DequeueResult
                        {
                            Id = deviceId.DeviceId,
                            MessageId = meta.DequeueIndex,
                            Peek = false,
                            EnqueueIndex = meta.EnqueueIndex
                        };
                        meta.DequeueIndex++;
                        meta.Version++;

                        dequeueItem.DequeueIndex = meta.DequeueIndex;
                        dequeueItem.Version = meta.Version;

                        if (dequeueItem.MessageId != deviceId.Index)
                        {
                            dequeueItem.Payload = queueEntry.Payload;
                            dequeueItem.Timestamp = queueEntry.Timestamp;
                        }

                        messages.Add(dequeueItem);
                    }
                    else
                    {
                        if (meta.DequeueIndex == meta.EnqueueIndex)
                        {
                            unknwoEtnries.Add(new DequeueResult
                            {
                                Id = meta.Id,
                                DequeueIndex = meta.DequeueIndex,
                                EnqueueIndex = meta.EnqueueIndex,
                                Peek = meta.Peek,
                                Version = meta.Version,
                                Payload = null
                            });
                        }
                    }
                }

                dequeueResults.Messages = messages;
                dequeueResults.UnknownEntries = unknwoEtnries;

                return dequeueResults;
            }
        }

        public DequeueResults Peek(IEnumerable<DeviceIdWithOpHint> deviceIds)
        {
            var dequeueResults = new DequeueResults();
            var messages = new List<DequeueResult>();
            var unknwoEtnries = new List<DequeueResult>();
            
            lock (_lock)
            {
                foreach (var deviceId in deviceIds)
                {
                    var meta = _messagingMetas[deviceId.DeviceId];
                    if (meta.DequeueIndex < meta.EnqueueIndex)
                    {
                        int dequeueIdx = meta.DequeueIndex % meta.QueueSize;
                        var queueEntry = _queueEntries[deviceId.DeviceId][dequeueIdx];

                        var dequeueItem = new DequeueResult
                        {
                            Id = deviceId.DeviceId,
                            MessageId = meta.DequeueIndex,
                            DequeueIndex = meta.DequeueIndex,
                            EnqueueIndex = meta.EnqueueIndex,
                        };
                        meta.Peek = true;
                        meta.Version++;

                        dequeueItem.Peek = meta.Peek;
                        dequeueItem.Version = meta.Version;

                        if (dequeueItem.MessageId != deviceId.Index)
                        {
                            dequeueItem.Payload = queueEntry.Payload;
                            dequeueItem.Timestamp = queueEntry.Timestamp;
                        }

                        messages.Add(dequeueItem);
                    }
                    else
                    {
                        if (meta.DequeueIndex == meta.EnqueueIndex)
                        {
                            unknwoEtnries.Add(new DequeueResult
                            {
                                Id = meta.Id,
                                DequeueIndex = meta.DequeueIndex,
                                EnqueueIndex = meta.EnqueueIndex,
                                Peek = meta.Peek,
                                Version = meta.Version,
                                Payload = null
                            });
                        }
                    }
                }

                dequeueResults.Messages = messages;
                dequeueResults.UnknownEntries = unknwoEtnries;

                return dequeueResults;
            }
        }

        public IReadOnlyCollection<DeviceEntry> Commit(IEnumerable<long> deviceIds)
        {
            lock (_lock)
            {
                var result = new List<DeviceEntry>();

                foreach (var deviceId in deviceIds)
                {
                    var meta = _messagingMetas[deviceId];

                    if (meta.DequeueIndex < meta.EnqueueIndex && meta.Peek)
                    {
                        meta.DequeueIndex++;
                        meta.Peek = false;
                        meta.Version++;

                        var dequeueResult = new DeviceEntry
                        {
                            Id = deviceId,
                            DequeueIndex = meta.DequeueIndex,
                            EnqueueIndex = meta.EnqueueIndex,
                            Peek = meta.Peek,
                            Version = meta.Version
                        };

                        result.Add(dequeueResult);
                    }
                }

                return result;
            }
        }

        class DeviceMessagingMeta
        {
            public long Id { get; set; }

            public int DequeueIndex { get; set; }

            public int EnqueueIndex { get; set; }

            public bool Peek { get; set; }

            public int QueueSize { get; set; }

            public int Version { get; set; }
        }

        class DeviceQueueEntry
        {
            public long Id { get; set; }

            public int Index { get; set; }

            public byte[] Payload { get; set; }

            public DateTime Timestamp { get; set; }
        }
    }
}
