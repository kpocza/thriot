using Thriot.ServiceClient.Messaging;

namespace Thriot.Platform.Services.Messaging
{
    internal class BatchWorkerReceiveAndForget : BatchWorkerReceive
    {
        private readonly IMessagingServiceClient _messagingServiceClient;

        public BatchWorkerReceiveAndForget(IMessagingServiceClient messagingServiceClient)
        {
            _messagingServiceClient = messagingServiceClient;
            
        }

        protected override DequeueMessagesDto Receive(DeviceListDto deviceList)
        {
            return _messagingServiceClient.Dequeue(deviceList);
        }
    }
}