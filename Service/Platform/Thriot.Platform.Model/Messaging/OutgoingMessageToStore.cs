using System;

namespace Thriot.Platform.Model.Messaging
{
    public class OutgoingMessageToStore
    {
        public OutgoingMessageToStore(long deviceId, byte[] payload, int messageId, DateTime time, string senderDeviceId)
        {
            DeviceId = deviceId;
            Payload = payload;
            MessageId = messageId;
            Time = time;
            SenderDeviceId = senderDeviceId;
        }

        public long DeviceId { get; private set; }

        public byte[] Payload { get; private set; }
        
        public int MessageId { get; private set; }

        public DateTime Time { get; private set; }

        public string SenderDeviceId { get; private set; }
    }
}