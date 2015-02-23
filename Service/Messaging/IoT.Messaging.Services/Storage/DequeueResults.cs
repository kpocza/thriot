using System.Collections.Generic;

namespace IoT.Messaging.Services.Storage
{
    public class DequeueResults
    {
        public IReadOnlyCollection<DequeueResult> Messages { get; set; }

        public IReadOnlyCollection<DeviceEntry> UnknownEntries { get; set; }
    }
}
