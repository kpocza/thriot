using Thriot.Messaging.Services.Client;

namespace Thriot.Platform.Services.Messaging
{
    internal class BatchWorkerPeek : BatchWorkerReceive
    {
        private readonly IMessagingServiceClient _messagingServiceClient;

        public BatchWorkerPeek(IMessagingServiceClient messagingServiceClient)
        {
            _messagingServiceClient = messagingServiceClient;
        }

        protected override DequeueMessagesDtoClient Receive(DeviceListDtoClient deviceList)
        {
            return _messagingServiceClient.Peek(deviceList);
        }
    }
}