namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Registration paraemters. The email address and the password properties come from the base entity (<see cref="Login"></see>)
    /// </summary>
    public class Register : Login
    {
        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; }
    }
}