using IoT.Management.Model.Operations;
using IoT.ServiceClient.TelemetrySetup;
using Mgmt = IoT.Management.Dto;

namespace IoT.Management.Services
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

        public Mgmt.TelemetryDataSinksMetadataDto GetIncomingTelemetryDataSinksMetadata()
        {
            Authenticate();

            var result = _telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata();
            return new Mgmt.TelemetryDataSinksMetadataDto
            {
                Incoming =
                    result.Incoming.ConvertAll(
                        i =>
                            new Mgmt.TelemetryDataSinkMetadataDto
                            {
                                Name = i.Name,
                                Description = i.Description,
                                ParametersToInput = i.ParametersToInput
                            })
            };
        }
    }
}
