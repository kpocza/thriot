using System;

namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// This exception will be thrown when the registration is performed on a Thriot installation where activation is needed after registration.
    /// When this exception is thrown the user was registered successfully by the <see cref="UserManagementClient.Register"/> method but cannot be logged in due to the activation requirement.
    /// PLease check your email and click the activation link.
    /// </summary>
    public class ActivationRequiredException : Exception
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="message">Message to be passed to the exception</param>
        public ActivationRequiredException(string message) : base(message)
        {
            
        }
    }
}
