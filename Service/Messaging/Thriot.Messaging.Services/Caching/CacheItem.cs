using System;

namespace Thriot.Messaging.Services.Caching
{
    public class CacheItem : CacheIndex
    {
        public CacheItem(long deviceId, int index, byte[] payload, DateTime timestamp, string senderDeviceId) : base(deviceId, index)
        {
            Payload = payload;
            Timestamp = timestamp;
            SenderDeviceId = senderDeviceId;
        }

        public CacheItem(CacheIndex cacheIndex, byte[] payload, DateTime timestamp, string senderDeviceId) : this(cacheIndex.DeviceId, cacheIndex.Index, payload, timestamp, senderDeviceId)
        {
        }

        public byte[] Payload { get; private set; }

        public DateTime Timestamp { get; private set; }

        public string SenderDeviceId { get; private set; }
    }
}
