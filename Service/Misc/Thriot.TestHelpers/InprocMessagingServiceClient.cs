using Thriot.Messaging.Services;
using Thriot.Messaging.Services.Client;
using Thriot.Messaging.Services.Dto;
using Thriot.Messaging.Services.Caching;
using Thriot.Messaging.Services.Storage;

namespace Thriot.TestHelpers
{
    public class InprocMessagingServiceClient : IMessagingServiceClient
    {
        private static InprocMessagingServiceClient _client;

        public static IMessagingServiceClient SqlInstance
        {
            get
            {
                if (_client == null)
                {
                    var client = new InprocMessagingServiceClient();
                    client.SetupSql();
                    _client = client;
                }
                return _client;
            }
        }

        public static IMessagingServiceClient PgSqlInstance
        {
            get
            {
                if (_client == null)
                {
                    var client = new InprocMessagingServiceClient();
                    client.SetupPgSql();
                    _client = client;
                }
                return _client;
            }
        }

        private MessagingService _messagingService;

        public class SqlConnectionStringResolver : IConnectionStringResolver
        {
            public string ConnectionString => @"Server=.\SQLEXPRESS;Database=ThriotMessaging;Trusted_Connection=True;";
        }

        public class PgSqlConnectionStringResolver : IConnectionStringResolver
        {
            public string ConnectionString => @"Server=127.0.0.1;Port=5432;Database=ThriotMessaging;User Id=thriotmessaging;Password=thriotmessaging;";
        }

        private InprocMessagingServiceClient()
        {
        }

        private void SetupSql()
        {
            _messagingService = new MessagingService(new MessageCache(), new PersistentStorage(new SqlConnectionStringResolver()));
        }

        private void SetupPgSql()
        {
            _messagingService = new MessagingService(new MessageCache(), new PersistentStoragePgSql(new PgSqlConnectionStringResolver()));
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
