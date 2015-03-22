namespace IoT.Client.DotNet.Management
{
    /// <summary>
    /// Entity with all properties that are publically exposed by a device
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Human readable name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Enclosing network id
        /// </summary>
        public string NetworkId { get; set; }

        /// <summary>
        /// Enclosing service id
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// Enclosing company id
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// Crypto-random device api key
        /// </summary>
        public string DeviceKey { get; set; }
    }
}