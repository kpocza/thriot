using System;

namespace Thriot.Messaging.Services.Storage
{
    public class DequeueResult : DeviceEntry
    {
        public byte[] Payload { get; set; }

        public DateTime Timestamp { get; set; }

        public string SenderDeviceId { get; set; }

        public int MessageId { get; set; }
    }
}
