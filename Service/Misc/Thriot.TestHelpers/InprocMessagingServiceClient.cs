using Thriot.Messaging.Services;
using Thriot.Messaging.Services.Client;
using Thriot.Messaging.Services.Dto;
using Thriot.Messaging.Services.Caching;
using Thriot.Messaging.Services.Storage;

namespace Thriot.TestHelpers
{
    public class InprocMessagingServiceClient : IMessagingServiceClient
    {
        public static readonly IMessagingServiceClient Instance = new InprocMessagingServiceClient();
        private readonly MessagingService _messagingService;

        public class ConnectionStringResolver : IConnectionStringResolver
        {
            public string ConnectionString
            {
                get { return @"Server=.\SQLEXPRESS;Database=ThriotMessaging;Trusted_Connection=True;"; }
            }
        }

        private InprocMessagingServiceClient()
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
