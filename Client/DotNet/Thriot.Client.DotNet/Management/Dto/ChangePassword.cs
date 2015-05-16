namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Change password DTO
    /// </summary>
    public class ChangePassword
    {
        /// <summary>
        /// The user's current password
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Proposed new password
        /// </summary>
        public string NewPassword { get; set; }
    }
}
