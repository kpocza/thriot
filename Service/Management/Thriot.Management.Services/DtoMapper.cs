using AutoMapper;
using Thriot.Management.Services.Dto;
using Thriot.Management.Model;
using Thriot.ServiceClient.TelemetrySetup;

namespace Thriot.Management.Services
{
    public static class DtoMapper
    {
        public static void Setup()
        {
            Mapper.CreateMap<Company, CompanyDto>();
            Mapper.CreateMap<Service, ServiceDto>();
            Mapper.CreateMap<Network, NetworkDto>();
            Mapper.CreateMap<Device, DeviceDto>();
            Mapper.CreateMap<User, UserDto>();
            Mapper.CreateMap<Small, SmallDto>();
            Mapper.CreateMap<SmallUser, SmallUserDto>();

            Mapper.CreateMap<RegisterDto, User>();
            Mapper.CreateMap<CompanyDto, Company>();
            Mapper.CreateMap<ServiceDto, Service>()
                .AfterMap((sdto, s) => s.Company = new Company() { Id = sdto.CompanyId });
            Mapper.CreateMap<NetworkDto, Network>()
                .AfterMap((dgdto, d) => d.Company = new Company() { Id = dgdto.CompanyId })
                .AfterMap((dgdto, d) => d.Service = new Service() { Id = dgdto.ServiceId })
                .AfterMap(
                    (ndto, d) =>
                        d.ParentNetwork =
                            ndto.ParentNetworkId != null ? new Network() { Id = ndto.ParentNetworkId } : null);
            Mapper.CreateMap<DeviceDto, Device>()
                .AfterMap((dgdto, d) => d.Company = new Company() { Id = dgdto.CompanyId })
                .AfterMap((dgdto, d) => d.Service = new Service() { Id = dgdto.ServiceId })
                .AfterMap(
                    (ndto, d) => d.Network = new Network() { Id = ndto.NetworkId });

            Mapper.CreateMap<TelemetryDataSinkParameters, TelemetryDataSinkParametersDto>();
            Mapper.CreateMap<TelemetryDataSinkParametersDto, TelemetryDataSinkParameters>()
                .AfterMap((tdto, t) => t.SinkName = t.SinkName.ToLowerInvariant());
            Mapper.CreateMap<TelemetryDataSinkSettings, TelemetryDataSinkSettingsDto>()
                .AfterMap((t, tdto) => tdto.Incoming.ForEach(i => i.SinkName = i.SinkName.ToLowerInvariant()));
            Mapper.CreateMap<TelemetryDataSinkSettingsDto, TelemetryDataSinkSettings>();
            Mapper.CreateMap<TelemetryDataSinkParametersDto, TelemetryDataSinkParametersRemoteDto>();
        }
    }
}
