using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Thriot.Framework.Logging;

namespace Thriot.Plugins.Core
{
    public abstract class SerialQueueReceiveAdapter : IQueueReceiveAdapter
    {
        private readonly Thread _thread;
        private readonly CancellationTokenSource _cts;
        private Action<TelemetryData> _itemProcessorAction;

        private const int MaxDequeueCount = 32;
        private const int ExpirationMinutes = 3;
        private const int AutoCommitTimeMinutes = 2;
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        protected SerialQueueReceiveAdapter()
        {
            _cts = new CancellationTokenSource();
            _thread = new Thread(Body);
        }

        public void Start(Action<TelemetryData> itemProcessorAction)
        {
            _itemProcessorAction = itemProcessorAction;
            _thread.Start();
        }

        public void Stop()
        {
            _cts.Cancel();
            _thread.Join();
        }

        private void Body()
        {
            while (!_cts.IsCancellationRequested)
            {
                var items = DequeueItems(MaxDequeueCount, ExpirationMinutes);

                if (!items.Any())
                {
                    Thread.Sleep(50);
                    continue;
                }

                var startProcessingAt = DateTime.UtcNow;
                var selected = new List<QueueItem>();

                foreach (var item in items)
                {
                    selected.Add(item);
                    ProcessItem(item.TelemetryData);

                    if (IsAutoCommitTimeReached(startProcessingAt))
                    {
                        CommitItems(selected);
                        selected.Clear();
                    }
                }

                if (selected.Any())
                {
                    CommitItems(selected);
                }
            }
        }

        private IEnumerable<QueueItem> DequeueItems(int maxDequeueCount, int expirationMinutes)
        {
            try
            {
                return DequeueItemsCore(maxDequeueCount, expirationMinutes);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return new List<QueueItem>();
            }
        }

        private void CommitItems(IEnumerable<QueueItem> selected)
        {
            try
            {
                CommitItemsCore(selected);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void ProcessItem(TelemetryData telemetryData)
        {
            try
            {
                _itemProcessorAction(telemetryData);
            }
            catch(Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private static bool IsAutoCommitTimeReached(DateTime startProcessingAt)
        {
            return DateTime.UtcNow - startProcessingAt >= TimeSpan.FromMinutes(AutoCommitTimeMinutes);
        }

        protected abstract IEnumerable<QueueItem> DequeueItemsCore(int maxDequeueCount, int expirationMinutes);

        protected abstract void CommitItemsCore(IEnumerable<QueueItem> items);
    }
}
