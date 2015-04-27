using System.Collections.Generic;
using System.Net;

namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// This class is responsible for managing companies and strictly company-related artifacts
    /// </summary>
    public class CompanyManagementClient : SpecificManagementClient
    {
        internal CompanyManagementClient(IRestConnection restConnection) : base(restConnection)
        {
        }

        /// <summary>
        /// List all companies that the currently logged in user can see and access
        /// 
        /// Send GET request to the APIROOT/companies Url
        /// </summary>
        /// <returns>List of the companies with the most important properties (id, name)</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public IEnumerable<Small> List()
        {
            var items = RestConnection.Get("companies");

            return JsonSerializer.Deserialize<List<Small>>(items);
        }

        /// <summary>
        /// Get the full detail of a company
        /// 
        /// Send GET request to the APIROOT/companies/id Url
        /// </summary>
        /// <param name="id">Unique identifier of a company</param>
        /// <returns>Company details</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public Company Get(string id)
        {
            var response = RestConnection.Get("companies/" + id);

            return JsonSerializer.Deserialize<Company>(response);
        }

        /// <summary>
        /// Create a new company
        /// 
        /// Send POST request to the APIROOT/companies Url
        /// </summary>
        /// <param name="company">New company instance</param>
        /// <returns>Unique id of the company</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public string Create(Company company)
        {
            var response = RestConnection.Post("companies", JsonSerializer.Serialize(company));

            return JsonSerializer.Deserialize<string>(response);
        }

        /// <summary>
        /// Update company. This method is used to update basic properties of the company (like name). 
        /// To update the telemetry data sink please use the <seealso cref="UpdateIncomingTelemetryDataSinks"/> method.
        /// 
        /// Send PUT request to the APIROOT/companies Url
        /// </summary>
        /// <param name="company">Company instance</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void Update(Company company)
        {
            RestConnection.Put("companies", JsonSerializer.Serialize(company));
        }

        /// <summary>
        /// Delete the company with the given id. The company must be empty.
        /// 
        /// Send DELETE request to the APIROOT/companies/id Url
        /// </summary>
        /// <param name="id">Unique identifier of the company</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void Delete(string id)
        {
            RestConnection.Delete("companies/" + id);
        }

        /// <summary>
        /// List services that are run by the company
        /// 
        /// Send GET request to the APIROOT/companies/id/services Url
        /// </summary>
        /// <param name="id">Unique identifier of the company</param>
        /// <returns>List of services</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public IEnumerable<Small> ListServices(string id)
        {
            var items = RestConnection.Get("companies/" + id + "/services");

            return JsonSerializer.Deserialize<List<Small>>(items);
        }

        /// <summary>
        /// List of users who can see and have access to the company
        /// 
        /// Send GET request to the APIROOT/companies/id/users Url
        /// </summary>
        /// <param name="id">Unique identifier of the company</param>
        /// <returns>List of users</returns>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public IEnumerable<SmallUser> ListUsers(string id)
        {
            var items = RestConnection.Get("companies/" + id + "/users");

            return JsonSerializer.Deserialize<List<SmallUser>>(items);
        }

        /// <summary>
        /// Add user to the company to provide access to the company for a given user.
        /// Use the <see cref="UserManagementClient.FindUser"/> method to find the user by email address.
        /// 
        /// Send POST request to the APIROOT/companies/adduser Url
        /// </summary>
        /// <param name="companyUser">Company-user pair</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void AddUser(CompanyUser companyUser)
        {
            RestConnection.Post("companies/adduser", JsonSerializer.Serialize(companyUser));
        }

        /// <summary>
        /// Update the telemetry data sinks for the company. These telemetry data sinks can be overriden at service level by the <see cref="ServiceManagementClient.UpdateIncomingTelemetryDataSinks"/> method and
        /// at network level by the <see cref="NetworkManagementClient.UpdateIncomingTelemetryDataSinks"/> method. If there is no override then all the devices under the company will use the telemetry data
        /// sinks configured by this method.
        /// 
        /// Send POST request to the APIROOT/companies/id/incomingTelmetryDataSinks Url
        /// </summary>
        /// <param name="id">Unique identifier of the company</param>
        /// <param name="telemetryDataSinkParameters">List of telemetry data sinks</param>
        /// <exception cref="WebException">This exception indicates some service level error. Please refer to the HTTP error code for more information</exception>
        public void UpdateIncomingTelemetryDataSinks(string id, IEnumerable<TelemetryDataSinkParameters> telemetryDataSinkParameters)
        {
            RestConnection.Post("companies/" + id + "/incomingTelemetryDataSinks", JsonSerializer.Serialize(telemetryDataSinkParameters, true));
        }
    }
}
