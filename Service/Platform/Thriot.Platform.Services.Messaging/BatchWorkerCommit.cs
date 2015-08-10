using System;
using System.Collections.Generic;
using System.Linq;
using Thriot.Framework.Batching;
using Thriot.Framework.Logging;
using Thriot.Platform.Model.Messaging;
using Thriot.Messaging.Services.Client;

namespace Thriot.Platform.Services.Messaging
{
    internal class BatchWorkerCommit : IBatchWorker<long, OutgoingState>
    {
        private readonly IMessagingServiceClient _messagingServiceClient;
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public BatchWorkerCommit(IMessagingServiceClient messagingServiceClient)
        {
            _messagingServiceClient = messagingServiceClient;
        }

        public IDictionary<Guid, OutgoingState> Process(IEnumerable<BatchItem<long>> deviceIds)
        {
            try
            {
                _messagingServiceClient.Commit(new DeviceListDtoClient
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