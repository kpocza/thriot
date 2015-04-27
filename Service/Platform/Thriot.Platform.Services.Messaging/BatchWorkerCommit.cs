using System;
using System.Collections.Generic;
using System.Linq;
using Thriot.Framework.Batching;
using Thriot.Framework.Logging;
using Thriot.Platform.Model.Messaging;
using Thriot.ServiceClient.Messaging;

namespace Thriot.Platform.Services.Messaging
{
    internal class BatchWorkerCommit : IBatchWorker<long, OutgoingState>
    {
        private readonly IMessagingService _messagingService;
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public BatchWorkerCommit(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public IDictionary<Guid, OutgoingState> Process(IEnumerable<BatchItem<long>> deviceIds)
        {
            try
            {
                _messagingService.Commit(new DeviceListDto
                {
                    DeviceIds = deviceIds.Select(d => d.Parameter).ToList()
                });

                var commitResults = new Dictionary<Guid, OutgoingState>();

                foreach (var item in deviceIds)
                {
                    commitResults.Add(item.Id, OutgoingState.Ok);
                }

                return commitResults;
            }
            catch(Exception ex)
            {
                Logger.Error("Devices: {0}. {1}", string.Join(",", deviceIds.Select(d => d.Parameter)) , ex.ToString());

                return deviceIds.ToDictionary(p => p.Id, p => OutgoingState.Fail);
            }
        }

        public bool IsItemThrottled(IEnumerable<WorkItem<long, OutgoingState>> existingItems, long newItem)
        {
            return existingItems.Any(v => v.Parameter == newItem);
        }

        public void Throttle(WorkItem<long, OutgoingState> newItem)
        {
            newItem.Result = OutgoingState.Throttled;
        }

        public void ReportCancellation(WorkItem<long, OutgoingState> newItem)
        {
            newItem.Result = OutgoingState.Fail;
        }
    }
}