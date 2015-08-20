using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.InMemoryQueue
{
    public class InMemorySerialQueueReceiveAdapter : SerialQueueReceiveAdapter
    {
        protected override IEnumerable<QueueItem> DequeueItemsCore(int maxDequeueCount, int expirationMinutes)
        {
            return InMemoryQueue.Instance.Dequeue(maxDequeueCount, expirationMinutes);
        }

        protected override void CommitItemsCore(IEnumerable<QueueItem> items)
        {
            InMemoryQueue.Instance.Commit(items);
        }
    }
}
