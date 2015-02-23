using System;
using System.Collections.Generic;
using System.Linq;
using IoT.Framework.Batching;
using IoT.Framework.Logging;
using IoT.Platform.Model.Messaging;
using IoT.ServiceClient.Messaging;

namespace IoT.Platform.Services.Messaging
{
    internal class BatchWorkerRecord : IBatchWorker<OutgoingMessageToStore, OutgoingState>
    {
        private readonly IMessagingService _messagingService;
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public BatchWorkerRecord(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public IDictionary<Guid, OutgoingState> Process(IEnumerable<BatchItem<OutgoingMessageToStore>> parameters)
        {
            try
            {
                var successfullDevices = _messagingService.Enqueue(new EnqueueMessagesDto
                {
                    Messages = parameters.Select(p => new EnqueueMessageDto
                    {
                        DeviceId = p.Parameter.DeviceId,
                        Payload = p.Parameter.Payload,
                        TimeStamp = p.Parameter.Time
                    }).ToList()
                });

                var enqueueResults = new Dictionary<Guid, OutgoingState>();

                foreach (var item in parameters)
                {
                    var successfull = successfullDevices.DeviceIds.Contains(item.Parameter.DeviceId);

                    enqueueResults.Add(item.Id, successfull ? OutgoingState.Ok : OutgoingState.Fail);
                }

                return enqueueResults;
            }
            catch(Exception ex)
            {
                Logger.Error("Devices: {0}. {1}", string.Join(",", parameters.Select(d => d.Parameter.DeviceId)), ex.ToString());

                return parameters.ToDictionary(p => p.Id, p => OutgoingState.Fail);
            }
        }

        public bool IsItemThrottled(IEnumerable<WorkItem<OutgoingMessageToStore, OutgoingState>> existingItems, OutgoingMessageToStore newItem)
        {
            return existingItems.Any(item => item.Parameter.DeviceId == newItem.DeviceId);
        }

        public void Throttle(WorkItem<OutgoingMessageToStore, OutgoingState> newItem)
        {
            newItem.Result = OutgoingState.Throttled;
        }

        public void ReportCancellation(WorkItem<OutgoingMessageToStore, OutgoingState> newItem)
        {
            newItem.Result = OutgoingState.Fail;
        }
    }
}