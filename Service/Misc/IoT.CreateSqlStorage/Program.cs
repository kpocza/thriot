using IoT.Framework;
using IoT.Framework.DataAccess;
using IoT.Management.Model;
using IoT.Management.Operations.Sql.DataAccess;

namespace IoT.CreateSqlStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            var localConnectionParametersResolver = new LocalConnectionParametersResolver();
            var unitOfWorkFactory = new ManagementUnitOfWorkFactory(localConnectionParametersResolver);

            using (var managementUnitOfWork = unitOfWorkFactory.Create())
            {
                var settingRepository = managementUnitOfWork.GetSettingRepository();

                CreateSettingIfNotExist(settingRepository, Setting.ServiceProfile, ServiceProfile.ServiceProvider.ToString());
                CreateSettingIfNotExist(settingRepository, Setting.EmailActivation, "false");
                CreateSettingIfNotExist(settingRepository, Setting.TelemetrySetupServiceEndpoint, "http://localhost:12345/papi/v1/telemetryDataSinkSetup");
                CreateSettingIfNotExist(settingRepository, Setting.TelemetrySetupServiceApiKey, Crypto.GenerateSafeRandomToken());
                CreateSettingIfNotExist(settingRepository, Setting.MessagingServiceEndpoint, "http://localhost:12345/msvc/v1/messaging");
                CreateSettingIfNotExist(settingRepository, Setting.MessagingServiceApiKey, Crypto.GenerateSafeRandomToken());
                CreateSettingIfNotExist(settingRepository, SettingId.GetConnection("TelemetryConnectionAzure"), "UseDevelopmentStorage=true");
                CreateSettingIfNotExist(settingRepository, SettingId.GetConnection("TelemetryConnectionSql"), @"Server=.\SQLEXPRESS;Database=IoTTelemetry;Trusted_Connection=True;");
                CreateSettingIfNotExist(settingRepository, Setting.WebsiteUrl, "http://localhost:12345");
                CreateSettingIfNotExist(settingRepository, Setting.ManagementApiUrl, "http://localhost:12345/api/v1");
                CreateSettingIfNotExist(settingRepository, Setting.PlatformApiUrl, "http://localhost:12345/papi/v1");
                CreateSettingIfNotExist(settingRepository, Setting.PlatformWsUrl, "ws://localhost:8080");
                CreateSettingIfNotExist(settingRepository, Setting.ReportingApiUrl, "http://localhost:12345/rapi/v1");

                managementUnitOfWork.Commit();
            }
        }

        private static void CreateSettingIfNotExist(SettingRepository settingRepository, SettingId settingId,
            string value)
        {
            var settings = settingRepository.List(s => s.Category == settingId.Category && s.Config == settingId.Config);

            if (settings.Count == 0)
            {
                var setting = new Setting(settingId, value);
                settingRepository.Create(setting);
            }
        }
    }
}
