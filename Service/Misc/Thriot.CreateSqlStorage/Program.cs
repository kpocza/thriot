using System;
using System.Collections.Generic;
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
            SettingReolver.Instance.Initialize(args);

            var unitOfWorkFactory = (IManagementUnitOfWorkFactory)Activator.CreateInstance(
                Type.GetType(ConfigurationManager.AppSettings["IManagementUnitOfWorkFactory"]), new LocalConnectionParametersResolver());

            using (var managementUnitOfWork = unitOfWorkFactory.Create())
            {
                var script = File.ReadAllText(Path.Combine("CreateScripts", ConfigurationManager.AppSettings["Script"]));
                Console.WriteLine("Executing DB schema update...");
                managementUnitOfWork.ExecuteScript(script);
                Console.WriteLine("DB schema update done.");

                var settingRepository = managementUnitOfWork.GetSettingRepository();

                CreateSettingIfNotExist(settingRepository, Setting.ServiceProfile, "runtime.serviceprofile", ServiceProfile.ServiceProvider.ToString());
                CreateSettingIfNotExist(settingRepository, Setting.EmailActivation, "runtime.emailactivation", "false");
                CreateSettingIfNotExist(settingRepository, Setting.TelemetrySetupServiceEndpoint, "microservice.telemetrysetupserviceendpoint", "http://localhost/papi/v1/telemetryDataSinkSetup");
                CreateSettingIfNotExist(settingRepository, Setting.TelemetrySetupServiceApiKey, null, Crypto.GenerateSafeRandomToken());
                CreateSettingIfNotExist(settingRepository, Setting.MessagingServiceEndpoint, "microservice.messagingserviceendpoint", "http://localhost/msvc/v1/messaging");
                CreateSettingIfNotExist(settingRepository, Setting.MessagingServiceApiKey, null, Crypto.GenerateSafeRandomToken());
                CreateSettingIfNotExist(settingRepository, SettingId.GetConnection("TelemetryConnection"), "telemetry.connection", ConfigurationManager.AppSettings["TelemetryConnection"]);
                CreateSettingIfNotExist(settingRepository, Setting.WebsiteUrl, "publicurl.web", "http://localhost");
                CreateSettingIfNotExist(settingRepository, Setting.ManagementApiUrl, "publicurl.managementapi", "http://localhost/api/v1");
                CreateSettingIfNotExist(settingRepository, Setting.PlatformApiUrl, "publicurl.platformapi", "http://localhost/papi/v1");
                CreateSettingIfNotExist(settingRepository, Setting.PlatformWsUrl, "publicurl.platformwebsocket", "ws://localhost:8080");
                CreateSettingIfNotExist(settingRepository, Setting.ReportingApiUrl, "publicurl.reportingapi", "http://localhost/rapi/v1");

                managementUnitOfWork.Commit();
            }
        }

        private static void CreateSettingIfNotExist(SettingRepository settingRepository, 
            SettingId settingId, string configurationSlot, string defaultValue)
        {
            //var settings = settingRepository.List(s => s.Category == settingId.Category && s.Config == settingId.Config);
            
            // EF7 workaround
            var settings =
                settingRepository.List(s => s.Category == settingId.Category)
                    .Where(s => s.Config == settingId.Config)
                    .ToList();

            if (settings.Count == 0)
            {
                var value = SettingReolver.Instance.Resolve(configurationSlot, defaultValue);

                var setting = new Setting(settingId, value);
                settingRepository.Create(setting);
                Console.WriteLine("Added {0}", settingId);
            }
        }

        class SettingReolver
        {
            private IDictionary<string, Dictionary<string, string>> _settings;

            internal static readonly SettingReolver Instance = new SettingReolver();

            private SettingReolver()
            {
            }

            internal void Initialize(string[] args)
            {
                if (args.Length == 1 && File.Exists(args[0]))
                {
                    _settings = Serializers.FromJsonString<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(args[0]));
                }
            }

            internal string Resolve(string configurationSlot, string defaultValue)
            {
                string value = defaultValue;

                if (_settings == null || configurationSlot == null || !configurationSlot.Contains("."))
                    return value;

                var parts = configurationSlot.Split('.');

                Dictionary<string, string> childDictionary;
                if (_settings.TryGetValue(parts[0], out childDictionary))
                {
                    string configValue;
                    if (childDictionary.TryGetValue(parts[1], out configValue))
                    {
                        value = configValue;
                    }
                }
                return value;
            }
        }
    }
}
