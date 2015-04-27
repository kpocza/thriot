using Thriot.ServiceClient.Messaging;

namespace Thriot.Platform.Services.Messaging
{
    internal class BatchWorkerPeek : BatchWorkerReceive
    {
        private readonly IMessagingService _messagingService;

        public BatchWorkerPeek(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        protected override DequeueMessagesDto Receive(DeviceListDto deviceList)
        {
            return _messagingService.Peek(deviceList);
        }
    }
}