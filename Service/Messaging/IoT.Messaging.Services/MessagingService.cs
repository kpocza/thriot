using System;
using System.Collections.Generic;
using System.Linq;
using IoT.Messaging.Dto;
using IoT.Messaging.Services.Caching;
using IoT.Messaging.Services.Storage;

namespace IoT.Messaging.Services
{
    public class MessagingService
    {
        private readonly IMessageCache _messageCache;
        private readonly IPersistentStorage _persistentStorage;

        public MessagingService(IMessageCache messageCache, IPersistentStorage persistentStorage)
        {
            _messageCache = messageCache;
            _persistentStorage = persistentStorage;
        }

        public long Initialize(string deviceId)
        {
            return _persistentStorage.InitializeDevice(deviceId);
        }

        public DeviceListDto Enqueue(EnqueueMessagesDto messages)
        {
            EnsureSingleDevice(messages.Messages.Select(m => m.DeviceId));
            
            var enqueueItems = ItemConverters.ToEnqueueItemList(messages);

            var deviceEntries = _persistentStorage.Enqueue(enqueueItems);

            var cacheItems = ItemConverters.ToCacheItems(deviceEntries, enqueueItems);
            _messageCache.Put(cacheItems);

            DeviceEntryRegistry.Instance.Merge(deviceEntries);

            return ItemConverters.ToDeviceListDto(deviceEntries);
        }

        public DequeueMessagesDto Dequeue(DeviceListDto devices)
        {
            return RetrieveItems(devices, _persistentStorage.Dequeue);
        }

        public DequeueMessagesDto Peek(DeviceListDto devices)
        {
            return RetrieveItems(devices, _persistentStorage.Peek);
        }

        public DeviceListDto Commit(DeviceListDto devices)
        {
            EnsureSingleDevice(devices.DeviceIds);

            var commitHint = DeviceEntryRegistry.Instance.GetCommitHint(devices.DeviceIds);

            var commitEntries = _persistentStorage.Commit(commitHint);

            DeviceEntryRegistry.Instance.Merge(commitEntries);

            return ItemConverters.ToDeviceList(commitEntries);
        }

        private DequeueMessagesDto RetrieveItems(DeviceListDto devices, Func<IEnumerable<DeviceIdWithOpHint>, DequeueResults> retrieveFunc)
        {
            EnsureSingleDevice(devices.DeviceIds);

            var dequeueHint = DeviceEntryRegistry.Instance.GetDequeueHint(devices.DeviceIds);

            var cacheItems = _messageCache.Get(dequeueHint.ToCacheIndices());

            var deviceIdWithOpHintList = ItemConverters.ToDeviceIdWithOpHint(dequeueHint);

            foreach (var hint in deviceIdWithOpHintList)
            {
                if (hint.Index.HasValue && cacheItems.MissingDevices.Contains(hint.DeviceId))
                {
                    hint.Index = null;
                }
            }

            var dequeueEntries = retrieveFunc(deviceIdWithOpHintList);

            DeviceEntryRegistry.Instance.Merge(dequeueEntries.Messages);
            DeviceEntryRegistry.Instance.Merge(dequeueEntries.UnknownEntries);

            _messageCache.Remove(cacheItems.CacheItems.Keys);

            return ItemConverters.GetDequeueMessagesDto(dequeueEntries.Messages, cacheItems);
        }

        private void EnsureSingleDevice(IEnumerable<long> deviceIds)
        {
            var deviceIdSet = new HashSet<long>();

            foreach (var deviceId in deviceIds)
            {
                if (deviceIdSet.Contains(deviceId))
                    throw new InvalidOperationException("Only one message can be recorded by device");
                deviceIdSet.Add(deviceId);
            }
        }
    }
}
