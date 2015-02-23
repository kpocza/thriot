using System;

namespace IoT.Messaging.Dto
{
    public class DequeueMessageDto
    {
        public long DeviceId { get; set; }

        public byte[] Payload { get; set; }

        public int MessageId { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
