using Thriot.Framework;
using Thriot.Management.Model;
using Thriot.Management.Model.Operations;

namespace Thriot.TestHelpers
{
    public static class SettingInitializer
    {
        public static void Init()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var settingOperations = environmentFactory.MgmtSettingOperations;

            SafeEnsureSetting(settingOperations, Setting.ServiceProfile, ServiceProfile.ServiceProvider.ToString());
            SafeEnsureSetting(settingOperations, Setting.EmailActivation, "false");
            SafeEnsureSetting(settingOperations, Setting.TelemetrySetupServiceEndpoint, "http://localhost:12345/papi/v1/telemetryDataSinkSetup");
            SafeEnsureSetting(settingOperations, Setting.TelemetrySetupServiceApiKey, Crypto.GenerateSafeRandomToken());
            SafeEnsureSetting(settingOperations, Setting.MessagingServiceEndpoint, "http://localhost:12345/msvc/v1/messaging");
            SafeEnsureSetting(settingOperations, Setting.MessagingServiceApiKey, Crypto.GenerateSafeRandomToken());

            SafeEnsureSetting(settingOperations, Setting.WebsiteUrl, "http://localhost:12345");
            SafeEnsureSetting(settingOperations, Setting.ManagementApiUrl, "http://localhost:12345/api/v1");
            SafeEnsureSetting(settingOperations, Setting.PlatformApiUrl, "http://localhost:12345/papi/v1");
            SafeEnsureSetting(settingOperations, Setting.PlatformWsUrl, "ws://localhost:8080");
            SafeEnsureSetting(settingOperations, Setting.ReportingApiUrl, "http://localhost:12345/rapi/v1");

            RemoveExtraEntries();
        }

        public static void RemoveExtraEntries()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var settingOperations = environmentFactory.MgmtSettingOperations;

            SafeDeleteSetting(settingOperations, Setting.PrebuiltCompany);
            SafeDeleteSetting(settingOperations, Setting.PrebuiltService);
            SafeDeleteSetting(settingOperations, Setting.UserForPrebuiltEntity);

            settingOperations.Update(new Setting(Setting.ServiceProfile, ServiceProfile.ServiceProvider.ToString()));
        }

        private static void SafeEnsureSetting(ISettingOperations settingOperations, SettingId name, string value)
        {
            try
            {
                settingOperations.Get(name);
            }
            catch
            {
                settingOperations.Create(new Setting(name, value));
            }
        }

        private static void SafeDeleteSetting(ISettingOperations settingOperations, SettingId name)
        {
            try
            {
                settingOperations.Delete(name);
            }
            catch
            {
            }
        }
    }
}
