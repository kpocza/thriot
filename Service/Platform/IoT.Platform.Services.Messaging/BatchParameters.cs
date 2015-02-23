using System;
using IoT.Platform.Model;

namespace IoT.Platform.Services.Messaging
{
    public class BatchParameters : IBatchParameters
    {
        public TimeSpan MessagingRecordCollectionTime
        {
            get { return TimeSpan.FromMilliseconds(100); }
        }

        public int MessagingRecordCollectionBatch
        {
            get { return 100; }
        }

        public int MessagingRecordFlushBatch
        {
            get { return 1024; }
        }

        public TimeSpan MessagingRecordOperationTimeout
        {
            get { return TimeSpan.FromMilliseconds(2000); }
        }

        public TimeSpan MessagingRecordWorkerStopTimeout
        {
            get { return TimeSpan.FromMilliseconds(2000); }
        }


        public TimeSpan MessagingReceiveAndForgetCollectionTime
        {
            get { return TimeSpan.FromMilliseconds(100); }
        }

        public int MessagingReceiveAndForgetCollectionBatch
        {
            get { return 500; }
        }

        public int MessagingReceiveAndForgetFlushBatch
        {
            get { return 1024; }
        }

        public TimeSpan MessagingReceiveAndForgetOperationTimeout
        {
            get { return TimeSpan.FromMilliseconds(2000); }
        }

        public TimeSpan MessagingReceiveAndForgetWorkerStopTimeout
        {
            get { return TimeSpan.FromMilliseconds(2000); }
        }


        public TimeSpan MessagingPeekCollectionTime
        {
            get { return TimeSpan.FromMilliseconds(100); }
        }

        public int MessagingPeekCollectionBatch
        {
            get { return 500; }
        }

        public int MessagingPeekFlushBatch
        {
            get { return 1024; }
        }

        public TimeSpan MessagingPeekOperationTimeout
        {
            get { return TimeSpan.FromMilliseconds(2000); }
        }

        public TimeSpan MessagingPeekWorkerStopTimeout
        {
            get { return TimeSpan.FromMilliseconds(2000); }
        }


        public TimeSpan MessagingCommitCollectionTime
        {
            get { return TimeSpan.FromMilliseconds(100); }
        }

        public int MessagingCommitCollectionBatch
        {
            get { return 500; }
        }

        public int MessagingCommitFlushBatch
        {
            get { return 1024; }
        }

        public TimeSpan MessagingCommitOperationTimeout
        {
            get { return TimeSpan.FromMilliseconds(2000); }
        }

        public TimeSpan MessagingCommitWorkerStopTimeout
        {
            get { return TimeSpan.FromMilliseconds(2000); }
        }

        public TimeSpan PersistentConnectionMessageReceiveAndForgetCollectionTime
        {
            get { return TimeSpan.FromMilliseconds(100); }
        }

        public int PersistentConnectionMessageReceiveAndForgetCollectionBatch
        {
            get { return 100; }
        }

        
        public TimeSpan PersistentConnectionMessagePeekCollectionTime
        {
            get { return TimeSpan.FromMilliseconds(100); }
        }

        public int PersistentConnectionMessagePeekCollectionBatch
        {
            get { return 100; }
        }
    }
}
