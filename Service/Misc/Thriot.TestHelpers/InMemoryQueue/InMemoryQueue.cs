using System;
using System.Collections.Generic;
using System.Threading;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.InMemoryQueue
{
    public class InMemoryQueue
    {
        private long _nextId;
        private LinkedList<InMemoryQueueItem> _queue;
        private readonly object _lock = new object();

        public static readonly InMemoryQueue Instance = new InMemoryQueue();

        public InMemoryQueue()
        {
            Clear();
        }

        public void Enqueue(TelemetryData telemetryData)
        {
            lock (_lock)
            {
                _queue.AddLast(new InMemoryQueueItem(_nextId, telemetryData));
                _nextId++;
            }
        }

        public IEnumerable<QueueItem> Dequeue(int maxDequeueCount, int expirationMinutes)
        {
            var list = new List<QueueItem>();
            lock (_lock)
            {
                var expirationMaxDate = DateTime.UtcNow.AddMinutes(-expirationMinutes);
                var dequeueAt = DateTime.UtcNow;

                int count = 0;
                foreach (var item in _queue)
                {
                    if (!item.DequeueAt.HasValue || item.DequeueAt.Value < expirationMaxDate)
                    {
                        item.DequeueAt = dequeueAt;
                        list.Add(item);
                        count++;

                        if (count > maxDequeueCount)
                            break;
                    }
                }
            }

            return list;
        }

        public void Commit(IEnumerable<QueueItem> items)
        {
            foreach (var item in items)
            {
                _queue.Remove((InMemoryQueueItem) item);
            }
        }

        public void Clear()
        {
            _nextId = 1;
            _queue = new LinkedList<InMemoryQueueItem>();
        }

        private class InMemoryQueueItem : QueueItem
        {
            public DateTime? DequeueAt { get; set; }

            public InMemoryQueueItem(long id, TelemetryData telemetryData) : base(id, telemetryData)
            {
            }
        }
    }
}
