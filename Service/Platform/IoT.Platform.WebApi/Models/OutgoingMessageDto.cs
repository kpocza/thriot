using System;
using IoT.Framework;
using IoT.Platform.Model.Messaging;

namespace IoT.Platform.WebApi.Models
{
    public class OutgoingMessageDto
    {
        public OutgoingMessageDto(OutgoingMessageToSend outgoingMessage)
        {
            Payload = outgoingMessage.Payload;
            Timestamp = outgoingMessage.Time.ToUnixTime();
            MessageId = outgoingMessage.MessageId;
        }

        public string Payload { get; private set; }

        public long Timestamp { get; private set; }

        public int MessageId { get; private set; }
    }
}