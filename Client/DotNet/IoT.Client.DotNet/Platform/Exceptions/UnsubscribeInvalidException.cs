using System;

namespace IoT.Client.DotNet.Platform.Exceptions
{
    public class UbsubscribeInvalidException : Exception
    {
        public string Response { get; private set; }

        public UbsubscribeInvalidException(string response)
        {
            Response = response;
        }
    }
}
