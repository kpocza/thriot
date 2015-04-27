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

        public bool HasMessage
        {
            get { return State == OutgoingState.Ok && Message != null; }
        }

        public string ToStringMessage()
        {
            var timestamp = Message.Time.ToUnixTime();
            var message = String.Format("pushedmessage {0} {1} {2}", Message.MessageId, timestamp, Encoding.UTF8.GetString(Message.Payload));

            return message;
        }
    }
}
