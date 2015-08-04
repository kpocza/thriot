﻿using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Thriot.Framework;
using Thriot.Framework.DataAccess;
using Thriot.Management.Model;
using Thriot.Management.Operations.Sql.DataAccess;

namespace Thriot.CreateSqlStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var unitOfWorkFactory = (IManagementUnitOfWorkFactory)Activator.CreateInstance(
                Type.GetType(ConfigurationManager.AppSettings["IManagementUnitOfWorkFactory"]), new LocalConnectionParametersResolver());

            using (var managementUnitOfWork = unitOfWorkFactory.Create())
            {
                var script = File.ReadAllText(Path.Combine("CreateScripts", ConfigurationManager.AppSettings["Script"]));
                Console.WriteLine("Executing DB schema update...");
                managementUnitOfWork.ExecuteScript(script);
                Console.WriteLine("DB schema update done.");

                var settingRepository = managementUnitOfWork.GetSettingRepository();

                CreateSettingIfNotExist(settingRepository, Setting.ServiceProfile, ServiceProfile.ServiceProvider.ToString());
                CreateSettingIfNotExist(settingRepository, Setting.EmailActivation, "false");
                CreateSettingIfNotExist(settingRepository, Setting.TelemetrySetupServiceEndpoint, "http://localhost/papi/v1/telemetryDataSinkSetup");
                CreateSettingIfNotExist(settingRepository, Setting.TelemetrySetupServiceApiKey, Crypto.GenerateSafeRandomToken());
                CreateSettingIfNotExist(settingRepository, Setting.MessagingServiceEndpoint, "http://localhost/msvc/v1/messaging");
                CreateSettingIfNotExist(settingRepository, Setting.MessagingServiceApiKey, Crypto.GenerateSafeRandomToken());
                CreateSettingIfNotExist(settingRepository, SettingId.GetConnection("TelemetryConnectionAzure"), "UseDevelopmentStorage=true");
                CreateSettingIfNotExist(settingRepository, SettingId.GetConnection("TelemetryConnectionSql"), ConfigurationManager.AppSettings["TelemetryConnectionSql"]);
                CreateSettingIfNotExist(settingRepository, Setting.WebsiteUrl, "http://localhost");
                CreateSettingIfNotExist(settingRepository, Setting.ManagementApiUrl, "http://localhost/api/v1");
                CreateSettingIfNotExist(settingRepository, Setting.PlatformApiUrl, "http://localhost/papi/v1");
                CreateSettingIfNotExist(settingRepository, Setting.PlatformWsUrl, "ws://localhost:8080");
                CreateSettingIfNotExist(settingRepository, Setting.ReportingApiUrl, "http://localhost/rapi/v1");

                managementUnitOfWork.Commit();
            }
        }

        private static void CreateSettingIfNotExist(SettingRepository settingRepository, SettingId settingId,
            string value)
        {
            //var settings = settingRepository.List(s => s.Category == settingId.Category && s.Config == settingId.Config);
            
            // EF7 workaround
            var settings =
                settingRepository.List(s => s.Category == settingId.Category)
                    .Where(s => s.Config == settingId.Config)
                    .ToList();

            if (settings.Count == 0)
            {
                var setting = new Setting(settingId, value);
                settingRepository.Create(setting);
                Console.WriteLine("Added {0}", settingId);
            }
        }
    }
}
