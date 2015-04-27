using System.Linq;
using Thriot.Messaging.Services.Storage;
using Thriot.ServiceClient.Messaging;

namespace Thriot.Messaging.PerformanceTest
{
    public class PureDatabaseCalls : IMessagingService
    {
        private readonly IPersistentStorage _persistentStorage = new PersistentStorage(new ConnectionStringResolver());

        public void Setup(string serviceUrl, string apiKey)
        {
        }

        public long Initialize(string deviceId)
        {
            return _persistentStorage.InitializeDevice(deviceId);
        }

        public DeviceListDto Enqueue(EnqueueMessagesDto enqueueMessages)
        {
            return new DeviceListDto
            {
                DeviceIds =
                    _persistentStorage.Enqueue(enqueueMessages.Messages.ConvertAll(m => new EnqueueItem
                    {
                        DeviceId = m.DeviceId,
                        Payload = m.Payload,
                        Timestamp = m.TimeStamp
                    })).Select(r => r.Id).ToList()
            };
        }

        public DequeueMessagesDto Dequeue(DeviceListDto deviceList)
        {
            return new DequeueMessagesDto
            {
                Messages =
                    _persistentStorage.Dequeue(
                        deviceList.DeviceIds.ConvertAll(id => new DeviceIdWithOpHint {DeviceId = id, Index = -1})).Messages
                        .Select(d => new DequeueMessageDto
                        {
                            DeviceId = d.Id,
                            MessageId = d.MessageId,
                            Payload = d.Payload,
                            TimeStamp = d.Timestamp
                        }).ToList()
            };
        }

        public DequeueMessagesDto Peek(DeviceListDto deviceList)
        {
            return new DequeueMessagesDto
            {
                Messages =
                    _persistentStorage.Peek(
                        deviceList.DeviceIds.ConvertAll(id => new DeviceIdWithOpHint { DeviceId = id, Index = -1 })).Messages
                        .Select(d => new DequeueMessageDto
                        {
                            DeviceId = d.Id,
                            MessageId = d.MessageId,
                            Payload = d.Payload,
                            TimeStamp = d.Timestamp
                        }).ToList()
            };
        }

        public DeviceListDto Commit(DeviceListDto deviceList)
        {
            return new DeviceListDto
            {
                DeviceIds = _persistentStorage.Commit(deviceList.DeviceIds).Select(d => d.Id).ToList()
            };
        }
    }
}
