using AutoMapper;
using IoT.Objects.Model;
using IoT.Platform.Services.Telemetry.Dtos;
using IoT.Platform.Services.Telemetry.Metadata;

namespace IoT.Platform.Services.Telemetry
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
