namespace IoT.Client.DotNet.Management
{
    public class DeviceManagementClient : SpecificManagementClient
    {
        internal DeviceManagementClient(IRestConnection restConnection) : base(restConnection)
        {
        }

        public Device Get(string id)
        {
            var response = RestConnection.Get("devices/" + id);

            return JsonSerializer.Deserialize<Device>(response);
        }

        public string Create(Device device)
        {
            var response = RestConnection.Post("devices", JsonSerializer.Serialize(device));

            return JsonSerializer.Deserialize<string>(response);
        }

        public void Update(Device device)
        {
            RestConnection.Put("devices", JsonSerializer.Serialize(device));
        }

        public void Delete(string id)
        {
            RestConnection.Delete("devices/" + id);
        }
    }
}
