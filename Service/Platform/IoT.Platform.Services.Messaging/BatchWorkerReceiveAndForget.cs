using IoT.ServiceClient.Messaging;

namespace IoT.Platform.Services.Messaging
{
    internal class BatchWorkerReceiveAndForget : BatchWorkerReceive
    {
        private readonly IMessagingService _messagingService;

        public BatchWorkerReceiveAndForget(IMessagingService messagingService)
        {
            _messagingService = messagingService;
            
        }

        protected override DequeueMessagesDto Receive(DeviceListDto deviceList)
        {
            return _messagingService.Dequeue(deviceList);
        }
    }
}