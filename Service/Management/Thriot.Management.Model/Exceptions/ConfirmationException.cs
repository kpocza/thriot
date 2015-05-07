using System;

namespace Thriot.Management.Model.Exceptions
{
    public class ConfirmationException : Exception
    {
        public ConfirmationException(string message)
            : base(message)
        {
        }
    }
}
