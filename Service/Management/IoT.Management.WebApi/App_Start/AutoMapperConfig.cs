using Thriot.Management.Services;

namespace Thriot.Management.WebApi
{
    public static class AutoMapperConfig
    {
        public static void Register()
        {
            DtoMapper.Setup();
        }
    }
}