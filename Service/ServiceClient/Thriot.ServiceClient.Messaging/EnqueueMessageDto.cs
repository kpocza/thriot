using System;

namespace Thriot.ServiceClient.Messaging
{
    public class EnqueueMessageDto
    {
        public long DeviceId { get; set; }

        public byte[] Payload { get; set; }
        
        public DateTime TimeStamp { get; set; }
    }
}
