namespace Thriot.Platform.Services.Client
{
    public interface ITelemetryDataSinkSetupServiceClient
    {
        void Setup(string serviceUrl, string apiKey);

        TelemetryDataSinksMetadataDtoClient GetTelemetryDataSinksMetadata();

        void PrepareAndValidateIncoming(TelemetryDataSinksParametersDtoClient telemetryDataSinkParameters);
    }
}
