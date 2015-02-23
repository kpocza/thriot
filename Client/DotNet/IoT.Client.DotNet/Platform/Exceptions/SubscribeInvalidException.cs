using System;

namespace IoT.Client.DotNet.Platform.Exceptions
{
    public class SubscribeInvalidException : Exception
    {
        public string Response { get; private set; }

        public SubscribeInvalidException(string response)
        {
            Response = response;
        }
    }
}
