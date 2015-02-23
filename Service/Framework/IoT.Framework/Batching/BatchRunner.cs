using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IoT.Framework.Logging;

namespace IoT.Framework.Batching
{
    public class BatchRunner<TParameter, TResult>
    {
        private readonly TimeSpan _batchCollectionTimeLimit;
        private readonly int _batchCollectionSizeLimit;
        private readonly int _batchFlushSizeLimit;
        private readonly TimeSpan _batchOperationTimeOut;
        private readonly TimeSpan _batchWorkerStopTimeout;
        private readonly IBatchWorker<TParameter, TResult> _batchWorker;

        protected Dictionary<Guid, WorkItem<TParameter, TResult>> _workItems;
        private readonly object _workerLock;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly AutoResetEvent _taskEndWaitEvent;
        private readonly AutoResetEvent _taskStartWaitEvent;

        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public BatchRunner(TimeSpan batchCollectionTimeLimit, int batchCollectionSizeLimit, int batchFlushSizeLimit, TimeSpan batchOperationTimeOut, TimeSpan batchWorkerStopTimeout, IBatchWorker<TParameter, TResult> batchWorker)
        {
            _batchCollectionTimeLimit = batchCollectionTimeLimit;
            _batchCollectionSizeLimit = batchCollectionSizeLimit;
            _batchFlushSizeLimit = batchFlushSizeLimit;
            _batchOperationTimeOut = batchOperationTimeOut;
            _batchWorkerStopTimeout = batchWorkerStopTimeout;
            _batchWorker = batchWorker;

            _workerLock = new object();
            _workItems = new Dictionary<Guid, WorkItem<TParameter, TResult>>();
            _cancellationTokenSource = new CancellationTokenSource();
            _taskStartWaitEvent = new AutoResetEvent(false);
            _taskEndWaitEvent = new AutoResetEvent(false);
        }

        public TResult EnqueueAndWaitItem(TParameter parameter)
        {
            var @event = new ManualResetEvent(false);
            var workItem = new WorkItem<TParameter, TResult>(@event, parameter);

            EnqueueItem(workItem);

            @event.WaitOne(WaitTime);

            return workItem.Result;
        }

        public IList<WorkItem<TParameter, TResult>> EnqueueAndWaitManyItems(IEnumerable<TParameter> parameters)
        {
            var workItems = new List<WorkItem<TParameter, TResult>>();
            var events = new List<WaitHandle>();
            foreach (TParameter parameter in parameters)
            {
                var @event = new ManualResetEvent(false);
                var workItem = new WorkItem<TParameter, TResult>(@event, parameter);

                EnqueueItem(workItem);

                events.Add(@event);
                workItems.Add(workItem);
            }

            MultiWaitHandler.WaitAll(events.ToArray(), WaitTime);

            return workItems;
        }

        public void EnqueueItem(WorkItem<TParameter, TResult> workItem)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                _batchWorker.ReportCancellation(workItem);
                workItem.Event.Set();
                return;
            }

            lock (_workerLock)
            {
                if (_batchWorker.IsItemThrottled(_workItems.Values, workItem.Parameter))
                {
                    _batchWorker.Throttle(workItem);
                    workItem.Event.Set();
                    return;
                }
                _workItems.Add(Guid.NewGuid(), workItem);
            }
        }

        public void Start()
        {
            new Thread(() => Body(_cancellationTokenSource.Token)).Start();
            _taskStartWaitEvent.WaitOne();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _taskEndWaitEvent.WaitOne((int)_batchWorkerStopTimeout.TotalMilliseconds);
        }

        public TimeSpan WaitTime
        {
            get { return TimeSpan.FromMilliseconds((int)_batchOperationTimeOut.TotalMilliseconds); }
        }

        private void Body(CancellationToken cancellationToken)
        {
            _taskStartWaitEvent.Set();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var lastProcessTime = DateTime.UtcNow;
                    while ((DateTime.UtcNow - lastProcessTime) < _batchCollectionTimeLimit && QueueCount() < _batchCollectionSizeLimit)
                    {
                        Thread.Sleep(2);
                    }

                    var items = GrabQueue();

                    if (items.Count > 0)
                    {
                        var batchItems = items.Select(item => new BatchItem<TParameter>(item.Key, item.Value.Parameter)).ToList();

                        foreach (var smallBatch in batchItems.Buffer(_batchFlushSizeLimit))
                        {
                            var processedItems = _batchWorker.Process(smallBatch);

                            if (processedItems.Count != smallBatch.Count)
                                throw new Exception(
                                    "THe Process method of batchworker should say something for all entities");

                            ReportResult(processedItems, items);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Batch processing error. {0}", ex.ToString());
                    throw;
                }
            }

            _taskEndWaitEvent.Set();
        }

        private int QueueCount()
        {
            lock (_workerLock)
            {
                return _workItems.Count;
            }
        }

        private IDictionary<Guid, WorkItem<TParameter, TResult>> GrabQueue()
        {
            lock (_workerLock)
            {
                var grabbedWorkItems = _workItems;

                _workItems = new Dictionary<Guid, WorkItem<TParameter, TResult>>();

                return grabbedWorkItems;
            }
        }

        private static void ReportResult(IDictionary<Guid, TResult> processedItems, IDictionary<Guid, WorkItem<TParameter, TResult>> items)
        {
            foreach (var processedItem in processedItems)
            {
                var item = items[processedItem.Key];
                item.Result = processedItem.Value;
                item.Event.Set();
                items.Remove(processedItem.Key);
            }
        }
    }
}
