using System;

namespace IoT.Client.DotNet.Platform
{
    public class PushedMessage
    {
        public int MessageId { get; private set; }

        public DateTime Timestamp { get; private set; }

        public string Payload { get; private set; }

        public PushedMessage(int messageId, long timestamp, string payload)
        {
            MessageId = messageId;
            Timestamp = new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(timestamp);
            Payload = payload;
        }
    }
}
