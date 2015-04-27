using System;

namespace Thriot.Platform.Model
{
    public interface IBatchParameters
    {
        TimeSpan MessagingRecordCollectionTime { get; }

        int MessagingRecordCollectionBatch { get; }

        int MessagingRecordFlushBatch { get; }

        TimeSpan MessagingRecordOperationTimeout { get; }

        TimeSpan MessagingRecordWorkerStopTimeout { get; }


        TimeSpan MessagingReceiveAndForgetCollectionTime { get; }

        int MessagingReceiveAndForgetCollectionBatch { get; }

        int MessagingReceiveAndForgetFlushBatch { get; }

        TimeSpan MessagingReceiveAndForgetOperationTimeout { get; }

        TimeSpan MessagingReceiveAndForgetWorkerStopTimeout { get; }


        TimeSpan MessagingPeekCollectionTime { get; }

        int MessagingPeekCollectionBatch { get; }

        int MessagingPeekFlushBatch { get; }

        TimeSpan MessagingPeekOperationTimeout { get; }

        TimeSpan MessagingPeekWorkerStopTimeout { get; }


        TimeSpan MessagingCommitCollectionTime { get; }

        int MessagingCommitCollectionBatch { get; }

        int MessagingCommitFlushBatch { get; }

        TimeSpan MessagingCommitOperationTimeout { get; }

        TimeSpan MessagingCommitWorkerStopTimeout { get; }


        TimeSpan PersistentConnectionMessageReceiveAndForgetCollectionTime { get; }

        int PersistentConnectionMessageReceiveAndForgetCollectionBatch { get; }


        TimeSpan PersistentConnectionMessagePeekCollectionTime { get; }

        int PersistentConnectionMessagePeekCollectionBatch { get; }
    }
}
