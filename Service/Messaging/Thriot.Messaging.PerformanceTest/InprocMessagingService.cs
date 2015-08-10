using Thriot.Messaging.Services;
using Thriot.Messaging.Services.Client;
using Thriot.Messaging.Services.Dto;
using Thriot.Messaging.Services.Caching;
using Thriot.Messaging.Services.Storage;

namespace Thriot.Messaging.PerformanceTest
{
    public class InprocMessagingServiceClient : IMessagingServiceClient
    {
        private readonly MessagingService _messagingService;

        public InprocMessagingServiceClient()
        {
            _messagingService = new MessagingService(new MessageCache(), new PersistentStorage(new ConnectionStringResolver()));
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
                            TimeStamp = m.TimeStamp
                        })
                    }).DeviceIds
            };
        }

        public DequeueMessagesDtoClient Dequeue(DeviceListDtoClient deviceList)
        {
            return new DequeueMessagesDtoClient
            {
                Messages =
                    _messagingService.Dequeue(new DeviceListDto { DeviceIds = deviceList.DeviceIds })
                        .Messages.ConvertAll(m => new DequeueMessageDtoClient
                        {
                            DeviceId = m.DeviceId,
                            MessageId = m.MessageId,
                            Payload = m.Payload,
                            TimeStamp = m.TimeStamp
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
                            TimeStamp = m.TimeStamp
                        })
            };
        }

        public DeviceListDtoClient Commit(DeviceListDtoClient deviceList)
        {
            return new DeviceListDtoClient
            {
                DeviceIds =
                    _messagingService.Commit(new DeviceListDto { DeviceIds = deviceList.DeviceIds })
                        .DeviceIds
            };
        }
    }
}
