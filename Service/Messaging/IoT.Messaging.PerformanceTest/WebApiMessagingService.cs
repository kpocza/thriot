using Thriot.ServiceClient.Messaging;

namespace Thriot.Messaging.PerformanceTest
{
    public class WebApiMessagingService : IMessagingService
    {
        readonly MessagingService _messagingService = new MessagingService();

        public void Setup(string serviceUrl, string apiKey)
        {
            _messagingService.Setup(serviceUrl, apiKey);
        }

        public long Initialize(string deviceId)
        {
            return _messagingService.Initialize(deviceId);
        }

        public DeviceListDto Enqueue(EnqueueMessagesDto enqueueMessages)
        {
            return _messagingService.Enqueue(enqueueMessages);
        }

        public DequeueMessagesDto Dequeue(DeviceListDto deviceList)
        {
            return _messagingService.Dequeue(deviceList);
        }

        public DequeueMessagesDto Peek(DeviceListDto deviceList)
        {
            return _messagingService.Peek(deviceList);
        }

        public DeviceListDto Commit(DeviceListDto deviceList)
        {
            return _messagingService.Commit(deviceList);
        }
    }
}
