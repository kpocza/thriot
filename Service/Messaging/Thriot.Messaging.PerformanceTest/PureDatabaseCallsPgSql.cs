using System.Linq;
using Thriot.Messaging.Services.Storage;
using Thriot.Messaging.Services.Client;

namespace Thriot.Messaging.PerformanceTest
{
    public class PureDatabaseCallsPgSqlClient : IMessagingServiceClient
    {
        private readonly IPersistentStorage _persistentStorage = new PersistentStoragePgSql(new ConnectionStringResolverPgSql());

        public void Setup(string serviceUrl, string apiKey)
        {
        }

        public long Initialize(string deviceId)
        {
            return _persistentStorage.InitializeDevice(deviceId);
        }

        public DeviceListDtoClient Enqueue(EnqueueMessagesDtoClient enqueueMessages)
        {
            return new DeviceListDtoClient
            {
                DeviceIds =
                    _persistentStorage.Enqueue(enqueueMessages.Messages.ConvertAll(m => new EnqueueItem
                    {
                        DeviceId = m.DeviceId,
                        Payload = m.Payload,
                        Timestamp = m.TimeStamp,
                        SenderDeviceId = m.SenderDeviceId
                    })).Select(r => r.Id).ToList()
            };
        }

        public DequeueMessagesDtoClient Dequeue(DeviceListDtoClient deviceList)
        {
            return new DequeueMessagesDtoClient
            {
                Messages =
                    _persistentStorage.Dequeue(
                        deviceList.DeviceIds.ConvertAll(id => new DeviceIdWithOpHint {DeviceId = id, Index = -1})).Messages
                        .Select(d => new DequeueMessageDtoClient
                        {
                            DeviceId = d.Id,
                            MessageId = d.MessageId,
                            Payload = d.Payload,
                            TimeStamp = d.Timestamp,
                            SenderDeviceId = d.SenderDeviceId
                        }).ToList()
            };
        }

        public DequeueMessagesDtoClient Peek(DeviceListDtoClient deviceList)
        {
            return new DequeueMessagesDtoClient
            {
                Messages =
                    _persistentStorage.Peek(
                        deviceList.DeviceIds.ConvertAll(id => new DeviceIdWithOpHint { DeviceId = id, Index = -1 })).Messages
                        .Select(d => new DequeueMessageDtoClient
                        {
                            DeviceId = d.Id,
                            MessageId = d.MessageId,
                            Payload = d.Payload,
                            TimeStamp = d.Timestamp,
                            SenderDeviceId = d.SenderDeviceId
                        }).ToList()
            };
        }

        public DeviceListDtoClient Commit(DeviceListDtoClient deviceList)
        {
            return new DeviceListDtoClient
            {
                DeviceIds = _persistentStorage.Commit(deviceList.DeviceIds).Select(d => d.Id).ToList()
            };
        }
    }
}
