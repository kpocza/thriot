using System.Collections.Generic;

namespace IoT.Client.DotNet.Management
{
    public class NetworkManagementClient : SpecificManagementClient
    {
        internal NetworkManagementClient(IRestConnection restConnection) : base(restConnection)
        {
        }

        public Network Get(string id)
        {
            var response = RestConnection.Get("networks/" + id);

            return JsonSerializer.Deserialize<Network>(response);
        }

        public string Create(Network network)
        {
            var response = RestConnection.Post("networks", JsonSerializer.Serialize(network));

            return JsonSerializer.Deserialize<string>(response);
        }

        public void Update(Network network)
        {
            RestConnection.Put("networks", JsonSerializer.Serialize(network));
        }

        public void Delete(string id)
        {
            RestConnection.Delete("networks/" + id);
        }

        public IEnumerable<Small> ListNetworks(string id)
        {
            var items = RestConnection.Get("networks/" + id + "/networks");

            return JsonSerializer.Deserialize<List<Small>>(items);
        }

        public IEnumerable<Small> ListDevices(string id)
        {
            var items = RestConnection.Get("networks/" + id + "/devices");

            return JsonSerializer.Deserialize<List<Small>>(items);
        }

        public void UpdateIncomingTelemetryDataSinks(string id, IEnumerable<TelemetryDataSinkParameters> telemetryDataSinkParameters)
        {
            RestConnection.Post("networks/" + id + "/incomingTelemetryDataSinks", JsonSerializer.Serialize(telemetryDataSinkParameters, true));
        }
    }
}
