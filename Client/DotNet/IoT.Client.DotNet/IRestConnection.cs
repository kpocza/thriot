using System.Collections.Generic;

namespace Thriot.Client.DotNet
{
    public interface IRestConnection
    {
        void Setup(string baseUrl, IDictionary<string, string> headers);

        string Get(string url);

        string Post(string url, string content);

        string Put(string url, string content);

        void Delete(string url);
    }
}
