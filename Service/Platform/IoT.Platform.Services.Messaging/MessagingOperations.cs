using System.Collections.Generic;
using IoT.Platform.Model.Messaging;

namespace IoT.Platform.Services.Messaging
{
    public class MessagingOperations : IMessagingOperations
    {
        public OutgoingState Record(OutgoingMessageToStore message)
        {
            return MessagingWorkers.BatchRunnerRecord.EnqueueAndWaitItem(message);
        }

        public OutgoingMessageToStoreWithState ReceiveAndForget(long deviceId)
        {
            var result = MessagingWorkers.BatchRunnerReceiveAndForget.EnqueueAndWaitItem(deviceId);

            return result ?? new OutgoingMessageToStoreWithState(null, OutgoingState.Fail);
        }

        public IDictionary<long, OutgoingMessageToStoreWithState> ReceiveAndForgetMany(IEnumerable<long> deviceIds)
        {
            var list = new Dictionary<long, OutgoingMessageToStoreWithState>();

            var results = MessagingWorkers.BatchRunnerReceiveAndForget.EnqueueAndWaitManyItems(deviceIds);

            foreach (var result in results)
            {
                list.Add(result.Parameter,
                    result.Result ?? new OutgoingMessageToStoreWithState(null, OutgoingState.Fail));
            }

            return list;
        }

        public OutgoingMessageToStoreWithState Peek(long deviceId)
        {
            var result = MessagingWorkers.BatchRunnerPeek.EnqueueAndWaitItem(deviceId);

            return result ?? new OutgoingMessageToStoreWithState(null, OutgoingState.Fail);
        }

        public IDictionary<long, OutgoingMessageToStoreWithState> PeekMany(IEnumerable<long> deviceIds)
        {
            var list = new Dictionary<long, OutgoingMessageToStoreWithState>();

            var results = MessagingWorkers.BatchRunnerPeek.EnqueueAndWaitManyItems(deviceIds);

            foreach (var result in results)
            {
                list.Add(result.Parameter,
                    result.Result ?? new OutgoingMessageToStoreWithState(null, OutgoingState.Fail));
            }

            return list;
        }

        public OutgoingState Commit(long deviceId)
        {
            return MessagingWorkers.BatchRunnerCommit.EnqueueAndWaitItem(deviceId);
        }
    }
}
