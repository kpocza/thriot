namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Contains minimal information about a more complex entity
    /// </summary>
    public class Small
    {
        /// <summary>
        /// Identifier of the entity (generally 32 characters long)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the entity to be able to identify the entity by a human
        /// </summary>
        public string Name { get; set; }
    }
}