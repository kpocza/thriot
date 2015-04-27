using AutoMapper;
using Thriot.Objects.Model;
using Thriot.Platform.Services.Telemetry.Dtos;
using Thriot.Platform.Services.Telemetry.Metadata;

namespace Thriot.Platform.Services.Telemetry
{
    public static class DtoMapper
    {
        public static void Setup()
        {
            Mapper.CreateMap<TelemetryDataSinkMetadata, TelemetryDataSinkMetadataDto>().AfterMap((entity, dto) => dto.TypeName = entity.Type.AssemblyQualifiedName);
            Mapper.CreateMap<TelemetryDataSinkParametersRemoteDto, TelemetryDataSinkParameters>();
        }
    }
}
