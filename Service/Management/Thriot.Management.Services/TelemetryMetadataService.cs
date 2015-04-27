using Thriot.Management.Dto;
using Thriot.Management.Model.Operations;
using Thriot.ServiceClient.TelemetrySetup;
using Mgmt = Thriot.Management.Dto;
using TelemetryDataSinkMetadataDto = Thriot.Management.Dto.TelemetryDataSinkMetadataDto;
using TelemetryDataSinksMetadataDto = Thriot.Management.Dto.TelemetryDataSinksMetadataDto;

namespace Thriot.Management.Services
{
    public class TelemetryMetadataService : ManagementServiceBase
    {
        private readonly ITelemetryDataSinkSetupService _telemetryDataSinkSetupService;

        public TelemetryMetadataService(ITelemetryDataSinkSetupService telemetryDataSinkSetupService,
            ICompanyOperations companyOperations, IAuthenticationContext authenticationContext)
            : base(companyOperations, authenticationContext)
        {
            _telemetryDataSinkSetupService = telemetryDataSinkSetupService;
        }

        public TelemetryDataSinksMetadataDto GetIncomingTelemetryDataSinksMetadata()
        {
            Authenticate();

            var result = _telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata();
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
