using System;

namespace IoT.Client.DotNet.Platform.Exceptions
{
    public class LoginInvalidException : Exception
    {
        public string Response { get; private set; }

        public LoginInvalidException(string response)
        {
            Response = response;
        }
    }
}
