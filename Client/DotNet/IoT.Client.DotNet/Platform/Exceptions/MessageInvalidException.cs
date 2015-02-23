using System;

namespace IoT.Client.DotNet.Platform.Exceptions
{
    public class MessageInvalidException : Exception
    {
        public string Response { get; private set; }

        public MessageInvalidException(string response)
        {
            Response = response;
        }
    }
}
