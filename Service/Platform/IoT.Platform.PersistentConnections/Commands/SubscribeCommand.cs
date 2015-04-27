namespace Thriot.Platform.PersistentConnections.Commands
{
    public class SubscribeCommand : Command
    {
        public SubscriptionType SubscriptionType { get; private set; }

        public SubscribeCommand(string parameters)
        {
            if (parameters == "receiveandforget")
            {
                SubscriptionType = SubscriptionType.ReceiveAndForget;
                IsValid = true;
            }
            else if (parameters == "peekandcommit")
            {
                SubscriptionType = SubscriptionType.PeekAndCommit;
                IsValid = true;
            }
        }
    }
}
