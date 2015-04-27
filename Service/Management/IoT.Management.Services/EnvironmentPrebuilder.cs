using System.Security.Authentication;
using Thriot.Management.Dto;
using Thriot.Management.Model;

namespace Thriot.Management.Services
{
    public class EnvironmentPrebuilder : IEnvironmentPrebuilder
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly ISettingProvider _settingProvider;
        private readonly CompanyService _companyService;
        private readonly ServiceService _serviceService;

        public EnvironmentPrebuilder(IAuthenticationContext authenticationContext, ISettingProvider settingProvider, CompanyService companyService, ServiceService serviceService)
        {
            _authenticationContext = authenticationContext;
            _settingProvider = settingProvider;
            _companyService = companyService;
            _serviceService = serviceService;
        }

        public void EnsureEnvironment()
        {
            var userId = _authenticationContext.GetContextUser();
            if (userId == null)
                throw new AuthenticationException();

            if (!(_settingProvider.ServiceProfile == ServiceProfile.SingleCompany ||
                _settingProvider.ServiceProfile == ServiceProfile.SingleService))
                return;

            if (_settingProvider.ServiceProfile == ServiceProfile.SingleCompany)
            {
                if (_settingProvider.PrebuiltCompany != null)
                    return;

                var companyId = _companyService.Create("Default Company");

                _settingProvider.UserForPrebuiltEntity = userId;
                _settingProvider.PrebuiltCompany = companyId;
                return;
            }

            if (_settingProvider.ServiceProfile == ServiceProfile.SingleService)
            {
                if (_settingProvider.PrebuiltService != null)
                    return;

                var companyId = _companyService.Create("Default Company");

                var serviceId = _serviceService.Create(new ServiceDto {CompanyId = companyId, Name = "Default Service"});

                _settingProvider.UserForPrebuiltEntity = userId;
                _settingProvider.PrebuiltCompany = companyId;
                _settingProvider.PrebuiltService = serviceId;
                return;
            }

            throw new System.NotImplementedException();
        }
    }
}
