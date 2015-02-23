using System.Security.Authentication;
using IoT.Management.Dto;

namespace IoT.Management.Services
{
    public class InfoService
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly ISettingProvider _settingProvider;
        private readonly ICapabilityProvider _capabilityProvider;

        public InfoService(IAuthenticationContext authenticationContext, ISettingProvider settingProvider, ICapabilityProvider capabilityProvider)
        {
            _authenticationContext = authenticationContext;
            _settingProvider = settingProvider;
            _capabilityProvider = capabilityProvider;
        }

        public InfoDto Get()
        {
            var userId = _authenticationContext.GetContextUser();
            if(userId == null)
                throw new AuthenticationException();

            return new InfoDto
            {
                ServiceProfile = _settingProvider.ServiceProfile.ToString(),
                PrebuiltCompany = _settingProvider.PrebuiltCompany,
                PrebuiltService = _settingProvider.PrebuiltService,

                CanCreateCompany = _capabilityProvider.CanCreateCompany,
                CanDeleteCompany = _capabilityProvider.CanDeleteCompany,
                CanCreateService = _capabilityProvider.CanCreateService,
                CanDeleteService = _capabilityProvider.CanDeleteService
            };
        }

        public UrlInfoDto GetUrlInfo()
        {
            var userId = _authenticationContext.GetContextUser();
            if (userId == null)
                throw new AuthenticationException();

            return new UrlInfoDto
            {
                WebsiteUrl = _settingProvider.WebsiteUrl,
                ManagementApiUrl = _settingProvider.ManagementApiUrl,
                PlatformApiUrl = _settingProvider.PlatformApiUrl,
                PlatformWsUrl = _settingProvider.PlatformWsUrl,
                ReportingApiUrl = _settingProvider.ReportingApiUrl,
            };
        }
    }
}
