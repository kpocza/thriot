using System;
using System.Text;
using Thriot.Framework;

namespace Thriot.Platform.Model.Messaging
{
    public class OutgoingMessageToStoreWithState
    {
        public OutgoingMessageToStore Message { get; private set; }
        public OutgoingState State { get; private set; }

        public OutgoingMessageToStoreWithState(OutgoingMessageToStore message, OutgoingState state)
        {
            Message = message;
            State = state;
        }

        public bool HasMessage => State == OutgoingState.Ok && Message != null;

        public string ToStringMessage()
        {
            var timestamp = Message.Time.ToUnixTime();
            var message =
                $"pushedmessage {Message.MessageId} {timestamp} {Message.SenderDeviceId} {Encoding.UTF8.GetString(Message.Payload)}";

            return message;
        }
    }
}
