using System;
using System.Collections.Concurrent;
using IoT.Framework.Exceptions;
using IoT.Management.Model;
using IoT.Management.Model.Operations;

namespace IoT.Management.Services
{
    public class SettingProvider : ISettingProvider
    {
        private readonly ISettingOperations _settingOperations;
        private readonly ConcurrentDictionary<string, string> _settings;

        public SettingProvider(ISettingOperations settingOperations)
        {
            _settingOperations = settingOperations;
            _settings = new ConcurrentDictionary<string, string>();
        }

        #region Runtime settings

        public ServiceProfile ServiceProfile
        {
            get
            {
                var value = ReadThrough(Setting.ServiceProfile);
                return (ServiceProfile)Enum.Parse(typeof(ServiceProfile), value);
            }
        }

        public bool EmailActivation
        {
            get
            {
                var value = ReadThrough(Setting.EmailActivation).ToLowerInvariant();
                return value == "true";
            }
        }

        #endregion

        #region Microservice endpoints and api keys
        
        public string TelemetrySetupServiceApiKey
        {
            get
            {
                return ReadThrough(Setting.TelemetrySetupServiceApiKey);
            }
        }

        public string TelemetrySetupServiceEndpoint
        {
            get
            {
                return ReadThrough(Setting.TelemetrySetupServiceEndpoint);
            }
        }

        public string MessagingServiceApiKey
        {
            get
            {
                return ReadThrough(Setting.MessagingServiceApiKey);
            }
        }

        public string MessagingServiceEndpoint
        {
            get
            {
                return ReadThrough(Setting.MessagingServiceEndpoint);
            }
        }

        #endregion

        #region Urls

        public string WebsiteUrl
        {
            get
            {
                return ReadThrough(Setting.WebsiteUrl);
            }
        }

        public string ManagementApiUrl
        {
            get
            {
                return ReadThrough(Setting.ManagementApiUrl);
            }
        }

        public string PlatformApiUrl
        {
            get
            {
                return ReadThrough(Setting.PlatformApiUrl);
            }
        }

        public string PlatformWsUrl
        {
            get
            {
                return ReadThrough(Setting.PlatformWsUrl);
            }
        }

        public string ReportingApiUrl
        {
            get
            {
                return ReadThrough(Setting.ReportingApiUrl);
            }
        }

        #endregion

        #region Different ServiceProfile settings

        public string PrebuiltCompany
        {
            get
            {
                try
                {
                    return ReadThrough(Setting.PrebuiltCompany);
                }
                catch (NotFoundException)
                {
                    return null;
                }
            }
            set
            {
                _settingOperations.Create(new Setting(Setting.PrebuiltCompany, value));
            }
        }

        public string PrebuiltService
        {
            get
            {
                try
                {
                    return ReadThrough(Setting.PrebuiltService);
                }
                catch (NotFoundException)
                {
                    return null;
                }
            }
            set
            {
                _settingOperations.Create(new Setting(Setting.PrebuiltService, value));
            }
        }

        public string UserForPrebuiltEntity
        {
            get
            {
                try
                {
                    return ReadThrough(Setting.UserForPrebuiltEntity);
                }
                catch (NotFoundException)
                {
                    return null;
                }
            }
            set
            {
                _settingOperations.Create(new Setting(Setting.UserForPrebuiltEntity, value));
            }
        } 

        #endregion

        private string ReadThrough(SettingId key)
        {
            return _settings.GetOrAdd(key.ToString(), (k) => _settingOperations.Get(key).Value);
        }
    }
}
