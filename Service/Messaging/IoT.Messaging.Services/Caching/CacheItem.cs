using System;

namespace Thriot.Messaging.Services.Caching
{
    public class CacheItem : CacheIndex
    {
        public CacheItem(long deviceId, int index, byte[] payload, DateTime timestamp) : base(deviceId, index)
        {
            Payload = payload;
            Timestamp = timestamp;
        }

        public CacheItem(CacheIndex cacheIndex, byte[] payload, DateTime timestamp) : this(cacheIndex.DeviceId, cacheIndex.Index, payload, timestamp)
        {
        }

        public byte[] Payload { get; private set; }

        public DateTime Timestamp { get; private set; }
    }
}
