using System;
using System.Collections.Generic;
using System.Linq;
using Thriot.Framework.Batching;
using Thriot.Framework.Logging;
using Thriot.Platform.Model.Messaging;
using Thriot.Messaging.Services.Client;

namespace Thriot.Platform.Services.Messaging
{
    internal abstract class BatchWorkerReceive : IBatchWorker<long, OutgoingMessageToStoreWithState>
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public IDictionary<Guid, OutgoingMessageToStoreWithState> Process(IEnumerable<BatchItem<long>> deviceIds)
        {
            try
            {
                var successfullItems =
                    Receive(new DeviceListDtoClient {DeviceIds = deviceIds.Select(d => d.Parameter).ToList()});

                var receiveResults = new Dictionary<Guid, OutgoingMessageToStoreWithState>();

                foreach (var item in deviceIds)
                {
                    var message = successfullItems.Messages.SingleOrDefault(m => m.DeviceId == item.Parameter);

                    receiveResults.Add(item.Id,
                        message != null
                            ? new OutgoingMessageToStoreWithState(
                                new OutgoingMessageToStore(message.DeviceId, message.Payload, message.MessageId,
                                    message.TimeStamp, message.SenderDeviceId), OutgoingState.Ok)
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

        protected abstract DequeueMessagesDtoClient Receive(DeviceListDtoClient deviceList);

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