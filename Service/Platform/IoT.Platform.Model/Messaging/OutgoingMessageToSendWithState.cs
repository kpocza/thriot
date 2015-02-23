namespace IoT.Platform.Model.Messaging
{
    public class OutgoingMessageToSendWithState
    {
        public OutgoingMessageToSend Message { get; private set; }
        public OutgoingState State { get; private set; }

        public OutgoingMessageToSendWithState(OutgoingMessageToSend message, OutgoingState state)
        {
            Message = message;
            State = state;
        }
    }
}
