namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Activation parameters. Used for the Activate method of user management client
    /// </summary>
    public class Activate
    {
        /// <summary>
        /// Unique user id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Activation code
        /// </summary>
        public string ActivationCode { get; set; }
    }
}
