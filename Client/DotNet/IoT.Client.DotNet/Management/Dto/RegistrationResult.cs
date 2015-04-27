namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Entity indicating the result of registration
    /// </summary>
    public class RegistrationResult
    {
        /// <summary>
        /// true only if the user has to click on the activation link sent by email before using the system
        /// </summary>
        public bool NeedsActivation { get; set; }

        /// <summary>
        /// Authentication token to be used with Basic authentication later
        /// </summary>
        public string AuthToken { get; set; }
    }
}
