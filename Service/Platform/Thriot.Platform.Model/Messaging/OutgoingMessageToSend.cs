using System;

namespace Thriot.Platform.Model.Messaging
{
    public class OutgoingMessageToSend
    {
        public OutgoingMessageToSend(string deviceId, string payload, DateTime time, int messageId, string senderDeviceId)
        {
            DeviceId = deviceId;
            Payload = payload;
            Time = time;
            MessageId = messageId;
            SenderDeviceId = senderDeviceId;
        }

        public string DeviceId { get; private set; }

        public string Payload { get; private set; }

        public DateTime Time { get; private set; }

        public int MessageId { get; private set; }

        public string SenderDeviceId { get; private set; }
    }
}
