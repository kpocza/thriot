using System;

namespace IoT.Messaging.Dto
{
    public class EnqueueMessageDto
    {
        public long DeviceId { get; set; }

        public byte[] Payload { get; set; }
        
        public DateTime TimeStamp { get; set; }
    }
}
