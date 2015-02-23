using IoT.Management.Services;

namespace IoT.Management.WebApi
{
    public static class AutoMapperConfig
    {
        public static void Register()
        {
            DtoMapper.Setup();
        }
    }
}