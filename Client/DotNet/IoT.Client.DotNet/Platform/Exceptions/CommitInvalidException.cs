using System;

namespace IoT.Client.DotNet.Platform.Exceptions
{
    public class CommitInvalidException : Exception
    {
        public string Response { get; private set; }

        public CommitInvalidException(string response)
        {
            Response = response;
        }
    }
}
