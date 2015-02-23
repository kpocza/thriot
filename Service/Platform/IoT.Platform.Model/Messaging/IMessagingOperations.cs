using System.Collections.Generic;

namespace IoT.Platform.Model.Messaging
{
    public interface IMessagingOperations
    {
        OutgoingState Record(OutgoingMessageToStore message);

        OutgoingMessageToStoreWithState ReceiveAndForget(long deviceId);

        IDictionary<long, OutgoingMessageToStoreWithState> ReceiveAndForgetMany(IEnumerable<long> deviceIds);

        OutgoingMessageToStoreWithState Peek(long deviceId);

        IDictionary<long, OutgoingMessageToStoreWithState> PeekMany(IEnumerable<long> deviceIds);

        OutgoingState Commit(long deviceId);
    }
}
