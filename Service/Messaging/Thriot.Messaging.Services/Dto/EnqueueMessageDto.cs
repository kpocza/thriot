using System;

namespace Thriot.Messaging.Services.Dto
{
    public class EnqueueMessageDto
    {
        public long DeviceId { get; set; }

        public byte[] Payload { get; set; }
        
        public DateTime TimeStamp { get; set; }

        public string SenderDeviceId { get; set; }
    }
}
