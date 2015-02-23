using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IoT.Client.DotNet
{
    public class RestConnection : IRestConnection
    {
        private string _baseUrl;
        private IDictionary<string, string> _headers;

        public void Setup(string baseUrl, IDictionary<string, string> headers)
        {
            _baseUrl = baseUrl;
            _headers = headers;
        }

        public string Get(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                SetHeaders(wc);

                return wc.DownloadString(_baseUrl + "/" + url);
            }
        }

        public string Post(string url, string content)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                SetHeaders(wc);

                return wc.UploadString(_baseUrl + "/" + url, content);
            }
        }

        public string Put(string url, string content)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                SetHeaders(wc);

                return wc.UploadString(_baseUrl + "/" + url, "PUT", content);
            }
        }

        public void Delete(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                SetHeaders(wc);

                wc.UploadString(_baseUrl + "/" + url, "DELETE", "");
            }
        }

        private void SetHeaders(WebClient wc)
        {
            if (_headers == null)
                return;

            foreach (var header in _headers)
            {
                wc.Headers[header.Key] = header.Value;
            }
        }
    }
}