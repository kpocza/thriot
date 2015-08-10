using Thriot.Messaging.Services.Client;

namespace Thriot.Messaging.PerformanceTest
{
    public class WebApiMessagingServiceClient : IMessagingServiceClient
    {
        readonly MessagingServiceClient _messagingServiceClient = new MessagingServiceClient();

        public void Setup(string serviceUrl, string apiKey)
        {
            _messagingServiceClient.Setup(serviceUrl, apiKey);
        }

        public long Initialize(string deviceId)
        {
            return _messagingServiceClient.Initialize(deviceId);
        }

        public DeviceListDtoClient Enqueue(EnqueueMessagesDtoClient enqueueMessages)
        {
            return _messagingServiceClient.Enqueue(enqueueMessages);
        }

        public DequeueMessagesDtoClient Dequeue(DeviceListDtoClient deviceList)
        {
            return _messagingServiceClient.Dequeue(deviceList);
        }

        public DequeueMessagesDtoClient Peek(DeviceListDtoClient deviceList)
        {
            return _messagingServiceClient.Peek(deviceList);
        }

        public DeviceListDtoClient Commit(DeviceListDtoClient deviceList)
        {
            return _messagingServiceClient.Commit(deviceList);
        }
    }
}
