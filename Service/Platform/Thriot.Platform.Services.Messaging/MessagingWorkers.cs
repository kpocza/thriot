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

        public static void Start(IBatchParameters batchParameters, IMessagingServiceClient messagingServiceClient)
        {
            var batchWorkerRecord = new BatchWorkerRecord(messagingServiceClient);
            var batchWorkerReceiveAndForget = new BatchWorkerReceiveAndForget(messagingServiceClient);
            var batchWorkerPeek = new BatchWorkerPeek(messagingServiceClient);
            var batchWorkerCommit = new BatchWorkerCommit(messagingServiceClient);

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