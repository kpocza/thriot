namespace Thriot.ServiceClient.Messaging
{
    public interface IMessagingServiceClient
    {
        void Setup(string serviceUrl, string apiKey);

        long Initialize(string deviceId);

        DeviceListDto Enqueue(EnqueueMessagesDto enqueueMessages);

        DequeueMessagesDto Dequeue(DeviceListDto deviceList);

        DequeueMessagesDto Peek(DeviceListDto deviceList);

        DeviceListDto Commit(DeviceListDto deviceList);
    }
}
