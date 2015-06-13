using System;
using Thriot.Framework;
using Thriot.Platform.Model.Messaging;

namespace Thriot.Platform.WebApi.Models
{
    public class OutgoingMessageDto
    {
        public OutgoingMessageDto(OutgoingMessageToSend outgoingMessage)
        {
            Payload = outgoingMessage.Payload;
            Timestamp = outgoingMessage.Time.ToUnixTime();
            MessageId = outgoingMessage.MessageId;
            SenderDeviceId = outgoingMessage.SenderDeviceId;
        }

        public string Payload { get; private set; }

        public long Timestamp { get; private set; }

        public int MessageId { get; private set; }

        public string SenderDeviceId { get; private set; }
    }
}