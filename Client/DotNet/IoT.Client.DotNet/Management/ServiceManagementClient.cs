using System.Collections.Generic;
using System.Net;

namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// This class is responsible for managing services and strictly service-related artifacts
    /// </summary>
    public class ServiceManagementClient : SpecificManagementClient
    {
        internal ServiceManagementClient(IRestConnection restConnection) : base(restConnection)
        {
        }

        /// <summary>
        /// Get the full detail of a service
        /// 
        /// Send GET request to the APIROOT/services/id Url
        /// </summary>
        /// <param name="id">Unique identifier of a service</param>
        /// <returns>Service details</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public Service Get(string id)
        {
            var response = RestConnection.Get("services/" + id);

            return JsonSerializer.Deserialize<Service>(response);
        }

        /// <summary>
        /// Create a new service
        /// 
        /// Send POST request to the APIROOT/services Url
        /// </summary>
        /// <param name="service">New serice instance</param>
        /// <returns>Unique id of the service</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public string Create(Service service)
        {
            var response = RestConnection.Post("services", JsonSerializer.Serialize(service));

            return JsonSerializer.Deserialize<string>(response);
        }

        /// <summary>
        /// Update service. This method is used to update basic properties of the service (like name). 
        /// To update the telemetry data sink please use the <seealso cref="UpdateIncomingTelemetryDataSinks"/> method.
        /// 
        /// Send PUT request to the APIROOT/services Url
        /// </summary>
        /// <param name="service">Service instance</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void Update(Service service)
        {
            RestConnection.Put("services", JsonSerializer.Serialize(service));
        }

        /// <summary>
        /// Delete the service with the given id. The service must be empty.
        /// 
        /// Send DELETE request to the APIROOT/services/id Url
        /// </summary>
        /// <param name="id">Unique identifier of the service</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void Delete(string id)
        {
            RestConnection.Delete("services/" + id);
        }

        /// <summary>
        /// List networks that are set up in the service
        /// 
        /// Send GET request to the APIROOT/services/id/networks Url
        /// </summary>
        /// <param name="id">Unique identifier of the service</param>
        /// <returns>List of networks</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public IEnumerable<Small> ListNetworks(string id)
        {
            var items = RestConnection.Get("services/" + id + "/networks");

            return JsonSerializer.Deserialize<List<Small>>(items);
        }

        /// <summary>
        /// Update the telemetry data sinks for the service. These telemetry data sinks can be overriden at network level
        /// by the <see cref="NetworkManagementClient.UpdateIncomingTelemetryDataSinks"/> method. If there is no override then all the devices under the service will use the telemetry data
        /// sinks configured by this method.
        /// 
        /// Send POST request to the APIROOT/services/id/incomingTelmetryDataSinks Url
        /// </summary>
        /// <param name="id">Unique identifier of the service</param>
        /// <param name="telemetryDataSinkParameters">List of telemetry data sinks</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void UpdateIncomingTelemetryDataSinks(string id, IEnumerable<TelemetryDataSinkParameters> telemetryDataSinkParameters)
        {
            RestConnection.Post("services/" + id + "/incomingTelemetryDataSinks", JsonSerializer.Serialize(telemetryDataSinkParameters, true));
        }
    }
}
