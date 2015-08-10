using Thriot.Messaging.Services.Client;

namespace Thriot.Platform.Services.Messaging
{
    internal class BatchWorkerReceiveAndForget : BatchWorkerReceive
    {
        private readonly IMessagingServiceClient _messagingServiceClient;

        public BatchWorkerReceiveAndForget(IMessagingServiceClient messagingServiceClient)
        {
            _messagingServiceClient = messagingServiceClient;
            
        }

        protected override DequeueMessagesDtoClient Receive(DeviceListDtoClient deviceList)
        {
            return _messagingServiceClient.Dequeue(deviceList);
        }
    }
}