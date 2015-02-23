using IoT.Messaging.Services.Caching;
using IoT.Messaging.Services.Storage;
using IoT.ServiceClient.Messaging;
using DequeueMessageDto = IoT.ServiceClient.Messaging.DequeueMessageDto;
using DequeueMessagesDto = IoT.ServiceClient.Messaging.DequeueMessagesDto;
using DeviceListDto = IoT.ServiceClient.Messaging.DeviceListDto;
using EnqueueMessagesDto = IoT.ServiceClient.Messaging.EnqueueMessagesDto;
using MSvc = IoT.Messaging.Services;

namespace IoT.UnitTestHelpers
{
    public class InMemoryMessagingService : IMessagingService
    {
        public static readonly IMessagingService Instance = new InMemoryMessagingService();
        private readonly MSvc.MessagingService _messagingService;

        private InMemoryMessagingService()
        {
            _messagingService = new MSvc.MessagingService(new MessageCache(), new PersistentStorageStub());
        }

        public void Setup(string serviceUrl, string apiKey)
        {
        }

        public long Initialize(string deviceId)
        {
            return _messagingService.Initialize(deviceId);
        }

        public DeviceListDto Enqueue(EnqueueMessagesDto enqueueMessages)
        {
            return new DeviceListDto
            {
                DeviceIds =
                    _messagingService.Enqueue(new Messaging.Dto.EnqueueMessagesDto
                    {
                        Messages = enqueueMessages.Messages.ConvertAll(m => new Messaging.Dto.EnqueueMessageDto
                        {
                            DeviceId = m.DeviceId,
                            Payload = m.Payload,
                            TimeStamp = m.TimeStamp
                        })
                    }).DeviceIds
            };
        }

        public DequeueMessagesDto Dequeue(DeviceListDto deviceList)
        {
            return new DequeueMessagesDto
            {
                Messages =
                    _messagingService.Dequeue(new Messaging.Dto.DeviceListDto {DeviceIds = deviceList.DeviceIds})
                        .Messages.ConvertAll(m => new DequeueMessageDto
                        {
                            DeviceId = m.DeviceId,
                            MessageId = m.MessageId,
                            Payload = m.Payload,
                            TimeStamp = m.TimeStamp
                        })
            };
        }

        public DequeueMessagesDto Peek(DeviceListDto deviceList)
        {
            return new DequeueMessagesDto
            {
                Messages =
                    _messagingService.Peek(new Messaging.Dto.DeviceListDto { DeviceIds = deviceList.DeviceIds })
                        .Messages.ConvertAll(m => new DequeueMessageDto
                        {
                            DeviceId = m.DeviceId,
                            MessageId = m.MessageId,
                            Payload = m.Payload,
                            TimeStamp = m.TimeStamp
                        })
            };
        }

        public DeviceListDto Commit(DeviceListDto deviceList)
        {
            return new DeviceListDto
            {
                DeviceIds =
                    _messagingService.Commit(new Messaging.Dto.DeviceListDto {DeviceIds = deviceList.DeviceIds})
                        .DeviceIds
            };
        }
    }
}
