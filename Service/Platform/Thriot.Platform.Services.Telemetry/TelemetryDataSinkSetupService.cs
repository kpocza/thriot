using System.Collections.Generic;
using AutoMapper;
using Thriot.Objects.Model;
using Thriot.Platform.Services.Telemetry.Dtos;
using Thriot.Platform.Services.Telemetry.Metadata;

namespace Thriot.Platform.Services.Telemetry
{
    public class TelemetryDataSinkSetupService
    {
        private readonly ITelemetryDataSinkMetadataRegistry _telemetryDataSinkMetadataRegistry;
        private readonly TelemetryDataSinkPreparator _telemetryDataSinkPreparator;

        public TelemetryDataSinkSetupService(ITelemetryDataSinkMetadataRegistry telemetryDataSinkMetadataRegistry, TelemetryDataSinkPreparator telemetryDataSinkPreparator)
        {
            _telemetryDataSinkMetadataRegistry = telemetryDataSinkMetadataRegistry;
            _telemetryDataSinkPreparator = telemetryDataSinkPreparator;
        }

        public TelemetryDataSinksMetadataDto GetTelemetryDataSinksMetadata()
        {
            var incoming = Mapper.Map<List<TelemetryDataSinkMetadataDto>>(_telemetryDataSinkMetadataRegistry.Incoming);

            return new TelemetryDataSinksMetadataDto {Incoming = incoming};
        }

        public void PrepareAndValidateIncoming(List<TelemetryDataSinkParametersRemoteDto> telemetryDataSinkParametersList)
        {
            var parameters = Mapper.Map<List<TelemetryDataSinkParameters>>(telemetryDataSinkParametersList);

            _telemetryDataSinkPreparator.PrepareAndValidateIncoming(parameters);
        }
    }
}
