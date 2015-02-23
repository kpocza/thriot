using System.Collections.Generic;

namespace IoT.Client.DotNet.Management
{
    public class CompanyManagementClient : SpecificManagementClient
    {
        internal CompanyManagementClient(IRestConnection restConnection) : base(restConnection)
        {
        }

        public IEnumerable<Small> List()
        {
            var items = RestConnection.Get("companies");

            return JsonSerializer.Deserialize<List<Small>>(items);
        }

        public Company Get(string id)
        {
            var response = RestConnection.Get("companies/" + id);

            return JsonSerializer.Deserialize<Company>(response);
        }

        public string Create(Company company)
        {
            var response = RestConnection.Post("companies", JsonSerializer.Serialize(company));

            return JsonSerializer.Deserialize<string>(response);
        }

        public void Update(Company company)
        {
            RestConnection.Put("companies", JsonSerializer.Serialize(company));
        }

        public void Delete(string id)
        {
            RestConnection.Delete("companies/" + id);
        }

        public IEnumerable<Small> ListServices(string id)
        {
            var items = RestConnection.Get("companies/" + id + "/services");

            return JsonSerializer.Deserialize<List<Small>>(items);
        }

        public IEnumerable<SmallUser> ListUsers(string id)
        {
            var items = RestConnection.Get("companies/" + id + "/users");

            return JsonSerializer.Deserialize<List<SmallUser>>(items);
        }

        public void AddUser(CompanyUser companyUser)
        {
            RestConnection.Post("companies/adduser", JsonSerializer.Serialize(companyUser));
        }

        public void UpdateIncomingTelemetryDataSinks(string id, IEnumerable<TelemetryDataSinkParameters> telemetryDataSinkParameters)
        {
            RestConnection.Post("companies/" + id + "/incomingTelemetryDataSinks", JsonSerializer.Serialize(telemetryDataSinkParameters, true));
        }
    }
}
