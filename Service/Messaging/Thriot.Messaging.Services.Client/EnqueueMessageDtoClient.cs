using System;

namespace Thriot.Messaging.Services.Client
{
    public class EnqueueMessageDtoClient
    {
        public long DeviceId { get; set; }

        public byte[] Payload { get; set; }
        
        public DateTime TimeStamp { get; set; }

        public string SenderDeviceId { get; set; }
    }
}
