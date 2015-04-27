using System.Collections.Generic;

namespace Thriot.Messaging.Services.Storage
{
    public interface IPersistentStorage
    {
        long InitializeDevice(string deviceId);

        IReadOnlyCollection<EnqueueResult> Enqueue(IEnumerable<EnqueueItem> items);

        DequeueResults Dequeue(IEnumerable<DeviceIdWithOpHint> deviceIds);

        DequeueResults Peek(IEnumerable<DeviceIdWithOpHint> deviceIds);

        IReadOnlyCollection<DeviceEntry> Commit(IEnumerable<long> deviceIds);
    }
}
