using System.Net;

namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// This class is responsible for managing devices
    /// </summary>
    public class DeviceManagementClient : SpecificManagementClient
    {
        internal DeviceManagementClient(IRestConnection restConnection) : base(restConnection)
        {
        }

        /// <summary>
        /// Get the full detail of a device
        /// 
        /// Send GET request to the APIROOT/devices/id Url
        /// </summary>
        /// <param name="id">Unique identifier of a device</param>
        /// <returns>Device details</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public Device Get(string id)
        {
            var response = RestConnection.Get("devices/" + id);

            return JsonSerializer.Deserialize<Device>(response);
        }

        /// <summary>
        /// Create a new device
        /// 
        /// Send POST request to the APIROOT/devices Url
        /// </summary>
        /// <param name="device">New device instance</param>
        /// <returns>Unique id of the device</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public string Create(Device device)
        {
            var response = RestConnection.Post("devices", JsonSerializer.Serialize(device));

            return JsonSerializer.Deserialize<string>(response);
        }

        /// <summary>
        /// Update device. This method is used to update basic properties of the device (like name). 
        /// 
        /// Send PUT request to the APIROOT/devices Url
        /// </summary>
        /// <param name="device">Device instance</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void Update(Device device)
        {
            RestConnection.Put("devices", JsonSerializer.Serialize(device));
        }

        /// <summary>
        /// Delete the device with the given id.
        /// 
        /// Send DELETE request to the APIROOT/devices/id Url
        /// </summary>
        /// <param name="id">Unique identifier of the device</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void Delete(string id)
        {
            RestConnection.Delete("devices/" + id);
        }
    }
}
