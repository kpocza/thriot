namespace Thriot.Messaging.Services.Client
{
    public interface IMessagingServiceClient
    {
        void Setup(string serviceUrl, string apiKey);

        long Initialize(string deviceId);

        DeviceListDtoClient Enqueue(EnqueueMessagesDtoClient enqueueMessages);

        DequeueMessagesDtoClient Dequeue(DeviceListDtoClient deviceList);

        DequeueMessagesDtoClient Peek(DeviceListDtoClient deviceList);

        DeviceListDtoClient Commit(DeviceListDtoClient deviceList);
    }
}
