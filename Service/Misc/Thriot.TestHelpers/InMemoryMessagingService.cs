using Thriot.Messaging.Services;
using Thriot.Messaging.Services.Client;
using Thriot.Messaging.Services.Dto;
using Thriot.Messaging.Services.Caching;
using Thriot.Messaging.Services.Storage;

namespace Thriot.TestHelpers
{
    public class InMemoryMessagingService : IMessagingServiceClient
    {
        public static readonly IMessagingServiceClient Instance = new InMemoryMessagingService();
        private readonly MessagingService _messagingService;

        private InMemoryMessagingService()
        {
            _messagingService = new MessagingService(new MessageCache(), new PersistentStorageStub());
        }

        public void Setup(string serviceUrl, string apiKey)
        {
        }

        public long Initialize(string deviceId)
        {
            return _messagingService.Initialize(deviceId);
        }

        public DeviceListDtoClient Enqueue(EnqueueMessagesDtoClient enqueueMessages)
        {
            return new DeviceListDtoClient
            {
                DeviceIds =
                    _messagingService.Enqueue(new EnqueueMessagesDto
                    {
                        Messages = enqueueMessages.Messages.ConvertAll(m => new EnqueueMessageDto
                        {
                            DeviceId = m.DeviceId,
                            Payload = m.Payload,
                            TimeStamp = m.TimeStamp,
                            SenderDeviceId = m.SenderDeviceId
                        })
                    }).DeviceIds
            };
        }

        public DequeueMessagesDtoClient Dequeue(DeviceListDtoClient deviceList)
        {
            return new DequeueMessagesDtoClient
            {
                Messages =
                    _messagingService.Dequeue(new DeviceListDto {DeviceIds = deviceList.DeviceIds})
                        .Messages.ConvertAll(m => new DequeueMessageDtoClient
                        {
                            DeviceId = m.DeviceId,
                            MessageId = m.MessageId,
                            Payload = m.Payload,
                            TimeStamp = m.TimeStamp,
                            SenderDeviceId = m.SenderDeviceId
                        })
            };
        }

        public DequeueMessagesDtoClient Peek(DeviceListDtoClient deviceList)
        {
            return new DequeueMessagesDtoClient
            {
                Messages =
                    _messagingService.Peek(new DeviceListDto { DeviceIds = deviceList.DeviceIds })
                        .Messages.ConvertAll(m => new DequeueMessageDtoClient
                        {
                            DeviceId = m.DeviceId,
                            MessageId = m.MessageId,
                            Payload = m.Payload,
                            TimeStamp = m.TimeStamp,
                            SenderDeviceId = m.SenderDeviceId
                        })
            };
        }

        public DeviceListDtoClient Commit(DeviceListDtoClient deviceList)
        {
            return new DeviceListDtoClient
            {
                DeviceIds =
                    _messagingService.Commit(new DeviceListDto {DeviceIds = deviceList.DeviceIds})
                        .DeviceIds
            };
        }
    }
}
