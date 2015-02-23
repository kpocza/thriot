namespace IoT.Client.DotNet.Management
{
    public class TelemetryDataSinksMetadataClient : SpecificManagementClient
    {
        internal TelemetryDataSinksMetadataClient(IRestConnection restConnection)
            : base(restConnection)
        {
        }

        public TelemetryDataSinksMetadata Get()
        {
            var response = RestConnection.Get("telemetryMetadata");

            return JsonSerializer.Deserialize<TelemetryDataSinksMetadata>(response);
        }
    }
}
