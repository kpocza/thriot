using System.Collections.Generic;
using System.Linq;
using Thriot.Messaging.Dto;
using Thriot.Messaging.Services.Caching;
using Thriot.Messaging.Services.Storage;

namespace Thriot.Messaging.Services
{
    internal static class ItemConverters
    {
        internal static IReadOnlyCollection<EnqueueItem> ToEnqueueItemList(EnqueueMessagesDto messages)
        {
            var enqueueItems = new List<EnqueueItem>();

            foreach (var msg in messages.Messages)
            {
                enqueueItems.Add(new EnqueueItem
                {
                    DeviceId = msg.DeviceId,
                    Payload = msg.Payload,
                    Timestamp = msg.TimeStamp,
                    SenderDeviceId = msg.SenderDeviceId
                });
            }
            return enqueueItems;
        }

        internal static IReadOnlyCollection<CacheItem> ToCacheItems(IReadOnlyCollection<EnqueueResult> enqueueResults, IEnumerable<EnqueueItem> enqueueItems)
        {
            var cacheItems = new List<CacheItem>();

            foreach (var enqueueResult in enqueueResults)
            {
                var enqueueItem = enqueueItems.Single(item => item.DeviceId == enqueueResult.Id);

                cacheItems.Add(new CacheItem(enqueueResult.Id, enqueueResult.MessageId, enqueueItem.Payload, enqueueItem.Timestamp, enqueueItem.SenderDeviceId));
            }

            return cacheItems;
        }

        public static DeviceListDto ToDeviceListDto(IEnumerable<EnqueueResult> enqueueResults)
        {
            var deviceListDto = new DeviceListDto() {DeviceIds = new List<long>()};
            foreach (var enqueueResult in enqueueResults)
            {
                deviceListDto.DeviceIds.Add(enqueueResult.Id);
            }
            return deviceListDto;
        }

        public static IReadOnlyCollection<DeviceIdWithOpHint> ToDeviceIdWithOpHint(DequeueHint dequeueHint)
        {
            var result = new List<DeviceIdWithOpHint>();

            foreach (var deviceEntry in dequeueHint.NewMessages)
            {
                result.Add(new DeviceIdWithOpHint
                {
                    DeviceId = deviceEntry.Id, 
                    Index = deviceEntry.DequeueIndex
                });
            }

            foreach (var unknownDevice in dequeueHint.UnknownDevices)
            {
                result.Add(new DeviceIdWithOpHint
                {
                    DeviceId = unknownDevice, 
                    Index = null
                });
            }

            return result;
        }

        public static DequeueMessagesDto GetDequeueMessagesDto(IReadOnlyCollection<DequeueResult> dequeueEntries, CacheGetResult cacheItems)
        {
            var dequeueMessages = new DequeueMessagesDto();
            dequeueMessages.Messages = new List<DequeueMessageDto>();
            foreach (var dequeueEntry in dequeueEntries)
            {
                var dequeueMessageDto = new DequeueMessageDto
                {
                    DeviceId = dequeueEntry.Id,
                    MessageId = dequeueEntry.MessageId,
                };

                bool addItem = false;
                if (dequeueEntry.Payload != null)
                {
                    dequeueMessageDto.Payload = dequeueEntry.Payload;
                    dequeueMessageDto.TimeStamp = dequeueEntry.Timestamp;
                    dequeueMessageDto.SenderDeviceId = dequeueEntry.SenderDeviceId;
                    addItem = true;
                }
                else
                {
                    var key = new CacheIndex(dequeueEntry.Id, dequeueEntry.MessageId);
                    CacheItem cacheItem;
                    if (cacheItems.CacheItems.TryGetValue(key, out cacheItem))
                    {
                        dequeueMessageDto.Payload = cacheItem.Payload;
                        dequeueMessageDto.TimeStamp = cacheItem.Timestamp;
                        dequeueMessageDto.SenderDeviceId = cacheItem.SenderDeviceId;
                        addItem = true;
                    }
                }

                if (addItem)
                    dequeueMessages.Messages.Add(dequeueMessageDto);
            }

            return dequeueMessages;
        }

        public static IReadOnlyCollection<DeviceIdWithOpHint> ToDeviceIdWithOpHint(IEnumerable<DeviceEntry> commitHint)
        {
            var result = new List<DeviceIdWithOpHint>();

            foreach (var deviceEntry in commitHint)
            {
                result.Add(new DeviceIdWithOpHint
                {
                    DeviceId = deviceEntry.Id,
                    Index = deviceEntry.DequeueIndex
                });
            }

            return result;
        }


        public static DeviceListDto ToDeviceList(IReadOnlyCollection<DeviceEntry> commitEntries)
        {
            var deviceList = new DeviceListDto();
            deviceList.DeviceIds = commitEntries.Select(d => d.Id).ToList();

            return deviceList;
        }
    }
}
