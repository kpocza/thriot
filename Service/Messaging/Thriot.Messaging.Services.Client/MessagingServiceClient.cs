using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Thriot.Messaging.Services.Client
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

        public DeviceListDtoClient Enqueue(EnqueueMessagesDtoClient enqueueMessages)
        {
            return PostRequest<DeviceListDtoClient>("enqueue", enqueueMessages);
        }

        public DequeueMessagesDtoClient Dequeue(DeviceListDtoClient deviceList)
        {
            return PostRequest<DequeueMessagesDtoClient>("dequeue", deviceList);
        }

        public DequeueMessagesDtoClient Peek(DeviceListDtoClient deviceList)
        {
            return PostRequest<DequeueMessagesDtoClient>("peek", deviceList);
        }

        public DeviceListDtoClient Commit(DeviceListDtoClient deviceList)
        {
            return PostRequest<DeviceListDtoClient>("commit", deviceList);
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