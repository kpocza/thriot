namespace Thriot.ServiceClient.TelemetrySetup
{
    public interface ITelemetryDataSinkSetupService
    {
        void Setup(string serviceUrl, string apiKey);

        TelemetryDataSinksMetadataDto GetTelemetryDataSinksMetadata();

        void PrepareAndValidateIncoming(TelemetryDataSinksParametersRemoteDto telemetryDataSinkParameters);
    }
}
