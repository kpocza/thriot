using System.Collections.Generic;
using System.Net;

namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// This class is responsible for managing networks and strictly network-related artifacts
    /// </summary>
    public class NetworkManagementClient : SpecificManagementClient
    {
        internal NetworkManagementClient(IRestConnection restConnection) : base(restConnection)
        {
        }

        /// <summary>
        /// Get the full detail of a network
        /// 
        /// Send GET request to the APIROOT/networks/id Url
        /// </summary>
        /// <param name="id">Unique identifier of a network</param>
        /// <returns>Network details</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public Network Get(string id)
        {
            var response = RestConnection.Get("networks/" + id);

            return JsonSerializer.Deserialize<Network>(response);
        }

        /// <summary>
        /// Create a new network
        /// 
        /// Send POST request to the APIROOT/networks Url
        /// </summary>
        /// <param name="network">New network instance</param>
        /// <returns>Unique id of the network</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public string Create(Network network)
        {
            var response = RestConnection.Post("networks", JsonSerializer.Serialize(network));

            return JsonSerializer.Deserialize<string>(response);
        }

        /// <summary>
        /// Update network. This method is used to update basic properties of the network (like name). 
        /// To update the telemetry data sink please use the <seealso cref="UpdateIncomingTelemetryDataSinks"/> method.
        /// 
        /// Send PUT request to the APIROOT/networks Url
        /// </summary>
        /// <param name="network">Network instance</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void Update(Network network)
        {
            RestConnection.Put("networks", JsonSerializer.Serialize(network));
        }

        /// <summary>
        /// Delete the network with the given id. The network must be empty.
        /// 
        /// Send DELETE request to the APIROOT/networks/id Url
        /// </summary>
        /// <param name="id">Unique identifier of the network</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void Delete(string id)
        {
            RestConnection.Delete("networks/" + id);
        }

        /// <summary>
        /// List networks that are child networks of this network
        /// 
        /// Send GET request to the APIROOT/networks/id/networks Url
        /// </summary>
        /// <param name="id">Unique identifier of the network</param>
        /// <returns>List of child networks</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public IEnumerable<Small> ListNetworks(string id)
        {
            var items = RestConnection.Get("networks/" + id + "/networks");

            return JsonSerializer.Deserialize<List<Small>>(items);
        }

        /// <summary>
        /// List devices that are under this network
        /// 
        /// Send GET request to the APIROOT/networks/id/devices Url
        /// </summary>
        /// <param name="id">Unique identifier of the network</param>
        /// <returns>List of devices</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public IEnumerable<Small> ListDevices(string id)
        {
            var items = RestConnection.Get("networks/" + id + "/devices");

            return JsonSerializer.Deserialize<List<Small>>(items);
        }

        /// <summary>
        /// Update the telemetry data sinks for the network. These telemetry data sinks can be overriden at child network level
        /// by the <see cref="NetworkManagementClient.UpdateIncomingTelemetryDataSinks"/> method. If there is no override then all the devices under the network will use the telemetry data
        /// sinks configured by this method.
        /// 
        /// Send POST request to the APIROOT/networks/id/incomingTelmetryDataSinks Url
        /// </summary>
        /// <param name="id">Unique identifier of the network</param>
        /// <param name="telemetryDataSinkParameters">List of telemetry data sinks</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void UpdateIncomingTelemetryDataSinks(string id, IEnumerable<TelemetryDataSinkParameters> telemetryDataSinkParameters)
        {
            RestConnection.Post("networks/" + id + "/incomingTelemetryDataSinks", JsonSerializer.Serialize(telemetryDataSinkParameters, true));
        }
    }
}
