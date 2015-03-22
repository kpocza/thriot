namespace IoT.Client.DotNet.Management
{
    /// <summary>
    /// Login entity for loggin in user
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}