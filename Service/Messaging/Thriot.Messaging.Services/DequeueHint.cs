using System.Collections.Generic;
using Thriot.Messaging.Services.Caching;

namespace Thriot.Messaging.Services
{
    public class DequeueHint
    {
        public DequeueHint()
        {
            NewMessages = new List<DeviceEntry>();
            UnknownDevices = new List<long>();
        }

        public ICollection<DeviceEntry> NewMessages { get; set; }

        public ICollection<long> UnknownDevices { get; set; }

        public IEnumerable<CacheIndex> ToCacheIndices()
        {
            var result = new List<CacheIndex>();

            foreach (var deviceEntry in NewMessages)
            {
                result.Add(new CacheIndex(deviceEntry.Id, deviceEntry.DequeueIndex));
            }
            return result;
        }
    }
}
