namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Entity with all properties that are publically exposed by a network
    /// </summary>
    public class Network
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
        /// Parent network id. It is null when the network is directly under the service while it's the unique identifier of the parent network 
        /// when the network instance is a child network of an other network.
        /// </summary>
        public string ParentNetworkId { get; set; }

        /// <summary>
        /// Identifier of the service that contains the network. It's always set regardless if the network is directly under the service or it's a child network of an other network.
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// Identifier of the company that contains the network
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// Crypto-random network api key. All devices can use this along with the device id for authentication that are directly or indirectly under this network.
        /// </summary>
        public string NetworkKey { get; set; }

        /// <summary>
        /// Telemetry data sink instances with parameters set up
        /// </summary>
        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }
    }
}