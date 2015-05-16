namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Reset password parametres
    /// </summary>
    public class ResetPassword
    {
        /// <summary>
        /// Unique user id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Reset password confirmation code
        /// </summary>
        public string ConfirmationCode { get; set; }

        /// <summary>
        /// New proposed password
        /// </summary>
        public string Password { get; set; }
    }
}
