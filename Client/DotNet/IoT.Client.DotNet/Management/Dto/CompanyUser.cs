namespace IoT.Client.DotNet.Management
{
    /// <summary>
    /// Company-user pair. Used for manipulating users who can access a given company
    /// </summary>
    public class CompanyUser
    {
        /// <summary>
        /// Unique identifier of a company
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// Unique identifier of a user
        /// </summary>
        public string UserId { get; set; }
    }
}
