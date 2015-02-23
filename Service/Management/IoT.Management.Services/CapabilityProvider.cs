using IoT.Management.Model;

namespace IoT.Management.Services
{
    public class CapabilityProvider : ICapabilityProvider
    {
        private readonly ISettingProvider _settingProvider;

        public CapabilityProvider(ISettingProvider settingProvider)
        {
            _settingProvider = settingProvider;
        }

        public bool CanCreateCompany
        {
            get
            {
                return
                    _settingProvider.ServiceProfile == ServiceProfile.ServiceProvider
                    ||
                    _settingProvider.PrebuiltCompany == null;
            }
        }

        public bool CanDeleteCompany
        {
            get
            {
                return _settingProvider.ServiceProfile == ServiceProfile.ServiceProvider;
            }
        }

        public bool CanCreateService
        {
            get
            {
                return
                    _settingProvider.ServiceProfile == ServiceProfile.ServiceProvider
                    ||
                    _settingProvider.ServiceProfile == ServiceProfile.SingleCompany
                    ||
                    _settingProvider.PrebuiltService == null;
            }
        }

        public bool CanDeleteService
        {
            get
            {
                return
                    _settingProvider.ServiceProfile == ServiceProfile.ServiceProvider
                    ||
                    _settingProvider.ServiceProfile == ServiceProfile.SingleCompany;
            }
        }
    }
}
