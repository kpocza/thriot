using Thriot.Framework.Batching;
using Thriot.Platform.Model;
using Thriot.Platform.Model.Messaging;
using Thriot.ServiceClient.Messaging;

namespace Thriot.Platform.Services.Messaging
{
    public static class MessagingWorkers
    {
        public static BatchRunner<OutgoingMessageToStore, OutgoingState> BatchRunnerRecord { get; private set; }
        public static BatchRunner<long, OutgoingMessageToStoreWithState> BatchRunnerReceiveAndForget { get; private set; }
        public static BatchRunner<long, OutgoingMessageToStoreWithState> BatchRunnerPeek { get; private set; }
        public static BatchRunner<long, OutgoingState> BatchRunnerCommit { get; private set; }

        public static void Start(IBatchParameters batchParameters, IMessagingService messagingService)
        {
            var batchWorkerRecord = new BatchWorkerRecord(messagingService);
            var batchWorkerReceiveAndForget = new BatchWorkerReceiveAndForget(messagingService);
            var batchWorkerPeek = new BatchWorkerPeek(messagingService);
            var batchWorkerCommit = new BatchWorkerCommit(messagingService);

            BatchRunnerReceiveAndForget = new BatchRunner<long, OutgoingMessageToStoreWithState>(
                    batchParameters.MessagingReceiveAndForgetCollectionTime,
                    batchParameters.MessagingReceiveAndForgetCollectionBatch,
                    batchParameters.MessagingReceiveAndForgetFlushBatch, 
                    batchParameters.MessagingReceiveAndForgetOperationTimeout,
                    batchParameters.MessagingReceiveAndForgetWorkerStopTimeout,
                    batchWorkerReceiveAndForget);

            BatchRunnerPeek = new BatchRunner<long, OutgoingMessageToStoreWithState>(
                    batchParameters.MessagingPeekCollectionTime,
                    batchParameters.MessagingPeekCollectionBatch,
                    batchParameters.MessagingPeekFlushBatch,
                    batchParameters.MessagingPeekOperationTimeout,
                    batchParameters.MessagingPeekWorkerStopTimeout,
                    batchWorkerPeek);

            BatchRunnerCommit = new BatchRunner<long, OutgoingState>(
                    batchParameters.MessagingCommitCollectionTime,
                    batchParameters.MessagingCommitCollectionBatch,
                    batchParameters.MessagingCommitFlushBatch,
                    batchParameters.MessagingCommitOperationTimeout,
                    batchParameters.MessagingCommitWorkerStopTimeout,
                    batchWorkerCommit);

            BatchRunnerRecord =
                new BatchRunner<OutgoingMessageToStore, OutgoingState>(
                    batchParameters.MessagingRecordCollectionTime,
                    batchParameters.MessagingRecordCollectionBatch,
                    batchParameters.MessagingRecordFlushBatch,
                    batchParameters.MessagingRecordOperationTimeout,
                    batchParameters.MessagingRecordWorkerStopTimeout,
                    batchWorkerRecord);

            BatchRunnerReceiveAndForget.Start();
            BatchRunnerPeek.Start();
            BatchRunnerCommit.Start();
            BatchRunnerRecord.Start();
        }

        public static void Stop()
        {
            BatchRunnerRecord.Stop();
            BatchRunnerReceiveAndForget.Stop();
            BatchRunnerPeek.Stop();
            BatchRunnerCommit.Stop();
        }
    }
}