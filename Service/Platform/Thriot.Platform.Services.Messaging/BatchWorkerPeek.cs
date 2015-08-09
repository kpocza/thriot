using Thriot.ServiceClient.Messaging;

namespace Thriot.Platform.Services.Messaging
{
    internal class BatchWorkerPeek : BatchWorkerReceive
    {
        private readonly IMessagingServiceClient _messagingServiceClient;

        public BatchWorkerPeek(IMessagingServiceClient messagingServiceClient)
        {
            _messagingServiceClient = messagingServiceClient;
        }

        protected override DequeueMessagesDto Receive(DeviceListDto deviceList)
        {
            return _messagingServiceClient.Peek(deviceList);
        }
    }
}