using System;

namespace Thriot.Messaging.Services.Storage
{
    public class EnqueueItem
    {
        public long DeviceId { get; set; }
     
        public byte[] Payload { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}
