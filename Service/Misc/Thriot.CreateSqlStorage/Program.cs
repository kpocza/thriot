using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using Thriot.Framework;
using Thriot.Management.Model;
using Thriot.Management.Operations.Sql.DataAccess;

namespace Thriot.CreateSqlStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            var unitOfWorkFactory = SingleContainer.Instance.Resolve<IManagementUnitOfWorkFactory>();

            using (var managementUnitOfWork = unitOfWorkFactory.Create())
            {
                var script = File.ReadAllText("CreateScripts\\" + ConfigurationManager.AppSettings["Script"]);
                Console.WriteLine("Executing DB schema update...");
                managementUnitOfWork.ExecuteScript(script);
                Console.WriteLine("DB schema update done.");

                var settingRepository = managementUnitOfWork.GetSettingRepository();

                CreateSettingIfNotExist(settingRepository, Setting.ServiceProfile, ServiceProfile.ServiceProvider.ToString());
                CreateSettingIfNotExist(settingRepository, Setting.EmailActivation, "false");
                CreateSettingIfNotExist(settingRepository, Setting.TelemetrySetupServiceEndpoint, "http://localhost:12345/papi/v1/telemetryDataSinkSetup");
                CreateSettingIfNotExist(settingRepository, Setting.TelemetrySetupServiceApiKey, Crypto.GenerateSafeRandomToken());
                CreateSettingIfNotExist(settingRepository, Setting.MessagingServiceEndpoint, "http://localhost:12345/msvc/v1/messaging");
                CreateSettingIfNotExist(settingRepository, Setting.MessagingServiceApiKey, Crypto.GenerateSafeRandomToken());
                CreateSettingIfNotExist(settingRepository, SettingId.GetConnection("TelemetryConnectionAzure"), "UseDevelopmentStorage=true");
                CreateSettingIfNotExist(settingRepository, SettingId.GetConnection("TelemetryConnectionSql"), @"Server=.\SQLEXPRESS;Database=ThriotTelemetry;Trusted_Connection=True;");
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
                Console.WriteLine("Added {0}", settingId);
            }
        }
    }
}
