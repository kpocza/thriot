using Thriot.Management.Services.Dto;
using Thriot.Management.Model.Operations;
using Thriot.Platform.Services.Client;
using Mgmt = Thriot.Management.Services.Dto;
using TelemetryDataSinkMetadataDto = Thriot.Management.Services.Dto.TelemetryDataSinkMetadataDto;
using TelemetryDataSinksMetadataDto = Thriot.Management.Services.Dto.TelemetryDataSinksMetadataDto;

namespace Thriot.Management.Services
{
    public class TelemetryMetadataService : ManagementServiceBase
    {
        private readonly ITelemetryDataSinkSetupServiceClient _telemetryDataSinkSetupServiceClient;

        public TelemetryMetadataService(ITelemetryDataSinkSetupServiceClient telemetryDataSinkSetupServiceClient,
            ICompanyOperations companyOperations, IAuthenticationContext authenticationContext)
            : base(companyOperations, authenticationContext)
        {
            _telemetryDataSinkSetupServiceClient = telemetryDataSinkSetupServiceClient;
        }

        public TelemetryDataSinksMetadataDto GetIncomingTelemetryDataSinksMetadata()
        {
            Authenticate();

            var result = _telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata();
            return new TelemetryDataSinksMetadataDto
            {
                Incoming =
                    result.Incoming.ConvertAll(
                        i =>
                            new TelemetryDataSinkMetadataDto
                            {
                                Name = i.Name,
                                Description = i.Description,
                                ParametersToInput = i.ParametersToInput
                            })
            };
        }
    }
}
