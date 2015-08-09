using Thriot.ServiceClient.Messaging;

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

        public DeviceListDto Enqueue(EnqueueMessagesDto enqueueMessages)
        {
            return _messagingServiceClient.Enqueue(enqueueMessages);
        }

        public DequeueMessagesDto Dequeue(DeviceListDto deviceList)
        {
            return _messagingServiceClient.Dequeue(deviceList);
        }

        public DequeueMessagesDto Peek(DeviceListDto deviceList)
        {
            return _messagingServiceClient.Peek(deviceList);
        }

        public DeviceListDto Commit(DeviceListDto deviceList)
        {
            return _messagingServiceClient.Commit(deviceList);
        }
    }
}
