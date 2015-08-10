using System;
using System.Collections.Generic;
using System.Linq;
using Thriot.Framework.Batching;
using Thriot.Framework.Logging;
using Thriot.Platform.Model.Messaging;
using Thriot.Messaging.Services.Client;

namespace Thriot.Platform.Services.Messaging
{
    internal class BatchWorkerRecord : IBatchWorker<OutgoingMessageToStore, OutgoingState>
    {
        private readonly IMessagingServiceClient _messagingServiceClient;
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public BatchWorkerRecord(IMessagingServiceClient messagingServiceClient)
        {
            _messagingServiceClient = messagingServiceClient;
        }

        public IDictionary<Guid, OutgoingState> Process(IEnumerable<BatchItem<OutgoingMessageToStore>> parameters)
        {
            try
            {
                var successfullDevices = _messagingServiceClient.Enqueue(new EnqueueMessagesDtoClient
                {
                    Messages = parameters.Select(p => new EnqueueMessageDtoClient
                    {
                        DeviceId = p.Parameter.DeviceId,
                        Payload = p.Parameter.Payload,
                        TimeStamp = p.Parameter.Time,
                        SenderDeviceId = p.Parameter.SenderDeviceId
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