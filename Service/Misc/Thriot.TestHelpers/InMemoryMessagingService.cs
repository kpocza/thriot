using Thriot.Messaging.Services;
using Thriot.Messaging.Services.Caching;
using Thriot.Messaging.Services.Storage;
using Thriot.ServiceClient.Messaging;
using DequeueMessageDto = Thriot.ServiceClient.Messaging.DequeueMessageDto;
using DequeueMessagesDto = Thriot.ServiceClient.Messaging.DequeueMessagesDto;
using DeviceListDto = Thriot.ServiceClient.Messaging.DeviceListDto;
using EnqueueMessagesDto = Thriot.ServiceClient.Messaging.EnqueueMessagesDto;
using MessagingService = Thriot.Messaging.Services.MessagingService;
using MSvc = Thriot.Messaging.Services;

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

        public ServiceClient.Messaging.DeviceListDto Enqueue(ServiceClient.Messaging.EnqueueMessagesDto enqueueMessages)
        {
            return new ServiceClient.Messaging.DeviceListDto
            {
                DeviceIds =
                    _messagingService.Enqueue(new Messaging.Services.Dto.EnqueueMessagesDto
                    {
                        Messages = enqueueMessages.Messages.ConvertAll(m => new Messaging.Services.Dto.EnqueueMessageDto
                        {
                            DeviceId = m.DeviceId,
                            Payload = m.Payload,
                            TimeStamp = m.TimeStamp,
                            SenderDeviceId = m.SenderDeviceId
                        })
                    }).DeviceIds
            };
        }

        public ServiceClient.Messaging.DequeueMessagesDto Dequeue(ServiceClient.Messaging.DeviceListDto deviceList)
        {
            return new ServiceClient.Messaging.DequeueMessagesDto
            {
                Messages =
                    _messagingService.Dequeue(new Messaging.Services.Dto.DeviceListDto {DeviceIds = deviceList.DeviceIds})
                        .Messages.ConvertAll(m => new ServiceClient.Messaging.DequeueMessageDto
                        {
                            DeviceId = m.DeviceId,
                            MessageId = m.MessageId,
                            Payload = m.Payload,
                            TimeStamp = m.TimeStamp,
                            SenderDeviceId = m.SenderDeviceId
                        })
            };
        }

        public ServiceClient.Messaging.DequeueMessagesDto Peek(ServiceClient.Messaging.DeviceListDto deviceList)
        {
            return new ServiceClient.Messaging.DequeueMessagesDto
            {
                Messages =
                    _messagingService.Peek(new Messaging.Services.Dto.DeviceListDto { DeviceIds = deviceList.DeviceIds })
                        .Messages.ConvertAll(m => new ServiceClient.Messaging.DequeueMessageDto
                        {
                            DeviceId = m.DeviceId,
                            MessageId = m.MessageId,
                            Payload = m.Payload,
                            TimeStamp = m.TimeStamp,
                            SenderDeviceId = m.SenderDeviceId
                        })
            };
        }

        public ServiceClient.Messaging.DeviceListDto Commit(ServiceClient.Messaging.DeviceListDto deviceList)
        {
            return new ServiceClient.Messaging.DeviceListDto
            {
                DeviceIds =
                    _messagingService.Commit(new Messaging.Services.Dto.DeviceListDto {DeviceIds = deviceList.DeviceIds})
                        .DeviceIds
            };
        }
    }
}
