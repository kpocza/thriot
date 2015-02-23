using System;
using System.Collections.Generic;
using System.Linq;
using IoT.Framework.Batching;
using IoT.Framework.Logging;
using IoT.Platform.Model.Messaging;
using IoT.ServiceClient.Messaging;

namespace IoT.Platform.Services.Messaging
{
    internal abstract class BatchWorkerReceive : IBatchWorker<long, OutgoingMessageToStoreWithState>
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public IDictionary<Guid, OutgoingMessageToStoreWithState> Process(IEnumerable<BatchItem<long>> deviceIds)
        {
            try
            {
                var successfullItems =
                    Receive(new DeviceListDto {DeviceIds = deviceIds.Select(d => d.Parameter).ToList()});

                var receiveResults = new Dictionary<Guid, OutgoingMessageToStoreWithState>();

                foreach (var item in deviceIds)
                {
                    var message = successfullItems.Messages.SingleOrDefault(m => m.DeviceId == item.Parameter);

                    receiveResults.Add(item.Id,
                        message != null
                            ? new OutgoingMessageToStoreWithState(
                                new OutgoingMessageToStore(message.DeviceId, message.Payload, message.MessageId,
                                    message.TimeStamp), OutgoingState.Ok)
                            : new OutgoingMessageToStoreWithState(null, OutgoingState.Ok));
                }

                return receiveResults;
            }
            catch(Exception ex)
            {
                Logger.Error("Devices: {0}. {1}", string.Join(",", deviceIds.Select(d => d.Parameter)), ex.ToString());

                return deviceIds.ToDictionary(p => p.Id, p => new OutgoingMessageToStoreWithState(null, OutgoingState.Fail));
            }
        }

        protected abstract DequeueMessagesDto Receive(DeviceListDto deviceList);

        public bool IsItemThrottled(IEnumerable<WorkItem<long, OutgoingMessageToStoreWithState>> existingItems, long newItem)
        {
            return existingItems.Any(v => v.Parameter == newItem);
        }

        public void Throttle(WorkItem<long, OutgoingMessageToStoreWithState> newItem)
        {
            newItem.Result = new OutgoingMessageToStoreWithState(null, OutgoingState.Throttled);
        }

        public void ReportCancellation(WorkItem<long, OutgoingMessageToStoreWithState> newItem)
        {
            newItem.Result = new OutgoingMessageToStoreWithState(null, OutgoingState.Fail);
        }
    }
}