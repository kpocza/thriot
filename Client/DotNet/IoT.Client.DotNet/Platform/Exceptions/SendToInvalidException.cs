using System;

namespace IoT.Client.DotNet.Platform.Exceptions
{
    public class SendToInvalidException : Exception
    {
        public string Response { get; private set; }

        public SendToInvalidException(string response)
        {
            Response = response;
        }
    }
}
