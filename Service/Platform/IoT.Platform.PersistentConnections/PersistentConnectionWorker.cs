using System;
using System.Collections.Generic;
using System.Threading;
using Thriot.Framework.Logging;
using Thriot.Platform.Model.Messaging;

namespace Thriot.Platform.PersistentConnections
{
    public abstract class PersistentConnectionWorker
    {
        protected readonly PusherRegistry _pusherRegistry;
        protected readonly IMessagingOperations _outgoingMessageReader;
        private readonly TimeSpan _batchWaitTimeLimit;
        private readonly int _batchWaitSizeLimit;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly AutoResetEvent _taskEndWaitEvent;
        private readonly AutoResetEvent _taskStartWaitEvent;
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        protected PersistentConnectionWorker(PusherRegistry pusherRegistry, IMessagingOperations outgoingMessageReader, TimeSpan batchWaitTimeLimit, int batchWaitSizeLimit)
        {
            _pusherRegistry = pusherRegistry;
            _outgoingMessageReader = outgoingMessageReader;
            _batchWaitTimeLimit = batchWaitTimeLimit;
            _batchWaitSizeLimit = batchWaitSizeLimit;
            _cancellationTokenSource = new CancellationTokenSource();
            _taskEndWaitEvent = new AutoResetEvent(false);
            _taskStartWaitEvent = new AutoResetEvent(false);
        }

        public void Start()
        {
            new Thread(() => Body(_cancellationTokenSource.Token)).Start();
            _taskStartWaitEvent.WaitOne();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _taskEndWaitEvent.WaitOne((int)_batchWaitTimeLimit.TotalMilliseconds * 5);        
        }

        protected abstract IPersistentConnection GetNextConnection();

        protected abstract IDictionary<long, OutgoingMessageToStoreWithState> ProcessManyItems(IEnumerable<long> ids);

        protected abstract void PostProcessConnections(IDictionary<long, IPersistentConnection> allConnections, IDictionary<long, OutgoingMessageToStoreWithState> firedConnections);

        private void Body(CancellationToken cancellationToken)
        {
            _taskStartWaitEvent.Set();

            while (!cancellationToken.IsCancellationRequested)
            {
                var items = new Dictionary<long, IPersistentConnection>();

                var lastProcessTime = DateTime.UtcNow;
                while ((DateTime.UtcNow - lastProcessTime) < _batchWaitTimeLimit && items.Count < _batchWaitSizeLimit)
                {
                    var connection = GetNextConnection();
                    if (connection != null)
                    {
                        items[connection.NumericDeviceId] = connection;
                    }
                }
                if (items.Keys.Count == 0)
                {
                    Thread.Sleep(10);
                    continue;
                }

                IDictionary<long, OutgoingMessageToStoreWithState> results = null;
                try
                {
                    results = ProcessManyItems(items.Keys);
                }
                catch(Exception ex)
                {
                    Logger.Error("Process many items error. {0}", ex.ToString());
                    continue;
                }

                PostProcessConnections(items, results);

                if (results.Count > 0)
                {
                    foreach (var item in items)
                    {
                        var result = results[item.Key];

                        if (result.HasMessage)
                        {
                            try
                            {
                                item.Value.SendMessage(result);
                            }
                            catch(Exception ex)
                            {
                                Logger.Error("Send error. Device: {0}. {1}", item.Value.DeviceId, ex.ToString());
                            }
                        }
                    }
                    Thread.Sleep(1);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }

            _taskEndWaitEvent.Set();
        }
    }
}
