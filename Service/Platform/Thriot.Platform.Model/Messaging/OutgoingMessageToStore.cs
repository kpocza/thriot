using System;

namespace Thriot.Platform.Model.Messaging
{
    public class OutgoingMessageToStore
    {
        public OutgoingMessageToStore(long deviceId, byte[] payload, int messageId, DateTime time)
        {
            DeviceId = deviceId;
            Payload = payload;
            MessageId = messageId;
            Time = time;
        }

        public long DeviceId { get; private set; }

        public byte[] Payload { get; private set; }
        
        public int MessageId { get; private set; }

        public DateTime Time { get; private set; }
    }
}