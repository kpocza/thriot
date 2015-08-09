namespace Thriot.ServiceClient.TelemetrySetup
{
    public interface ITelemetryDataSinkSetupServiceClient
    {
        void Setup(string serviceUrl, string apiKey);

        TelemetryDataSinksMetadataDto GetTelemetryDataSinksMetadata();

        void PrepareAndValidateIncoming(TelemetryDataSinksParametersRemoteDto telemetryDataSinkParameters);
    }
}
