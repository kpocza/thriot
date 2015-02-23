using System.Collections.Generic;

namespace IoT.Client.DotNet.Management
{
    public class ServiceManagementClient : SpecificManagementClient
    {
        internal ServiceManagementClient(IRestConnection restConnection): base(restConnection)
        {
        }

        public Service Get(string id)
        {
            var response = RestConnection.Get("services/" + id);

            return JsonSerializer.Deserialize<Service>(response);
        }

        public string Create(Service service)
        {
            var response = RestConnection.Post("services", JsonSerializer.Serialize(service));

            return JsonSerializer.Deserialize<string>(response);
        }

        public void Update(Service service)
        {
            RestConnection.Put("services", JsonSerializer.Serialize(service));
        }

        public void Delete(string id)
        {
            RestConnection.Delete("services/" + id);
        }

        public IEnumerable<Small> ListNetworks(string id)
        {
            var items = RestConnection.Get("services/" + id + "/networks");

            return JsonSerializer.Deserialize<List<Small>>(items);
        }

        public void UpdateIncomingTelemetryDataSinks(string id, IEnumerable<TelemetryDataSinkParameters> telemetryDataSinkParameters)
        {
            RestConnection.Post("services/" + id + "/incomingTelemetryDataSinks", JsonSerializer.Serialize(telemetryDataSinkParameters, true));
        }
    }
}
