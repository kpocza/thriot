using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Thriot.ServiceClient.Messaging
{
    public class MessagingServiceClient : IMessagingServiceClient
    {
        private const string MessagingServiceApiKey = "X-MessagingServiceApiKey";
        private static string _serviceUrl;
        private static string _apiKey;

        public void Setup(string serviceUrl, string apiKey)
        {
            _apiKey = apiKey;
            _serviceUrl = serviceUrl;
        }

        public long Initialize(string deviceId)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers.Add(MessagingServiceApiKey, _apiKey);

                var str = wc.DownloadString(_serviceUrl + "/initialize/" + deviceId);
                return JsonConvert.DeserializeObject<long>(str);
            }
        }

        public DeviceListDto Enqueue(EnqueueMessagesDto enqueueMessages)
        {
            return PostRequest<DeviceListDto>("enqueue", enqueueMessages);
        }

        public DequeueMessagesDto Dequeue(DeviceListDto deviceList)
        {
            return PostRequest<DequeueMessagesDto>("dequeue", deviceList);
        }

        public DequeueMessagesDto Peek(DeviceListDto deviceList)
        {
            return PostRequest<DequeueMessagesDto>("peek", deviceList);
        }

        public DeviceListDto Commit(DeviceListDto deviceList)
        {
            return PostRequest<DeviceListDto>("commit", deviceList);
        }

        private T PostRequest<T>(string operationUrl, object request)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers.Add(MessagingServiceApiKey, _apiKey);

                var str = wc.UploadString(_serviceUrl + "/" + operationUrl, JsonConvert.SerializeObject(request));
                return JsonConvert.DeserializeObject<T>(str);
            }
        }
    }
}