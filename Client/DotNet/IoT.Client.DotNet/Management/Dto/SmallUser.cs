namespace IoT.Client.DotNet.Management
{
    /// <summary>
    /// Contains only the most important properties of the user entity
    /// </summary>
    public class SmallUser
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }
    }
}