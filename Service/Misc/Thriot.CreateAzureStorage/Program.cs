using System;
using System.Collections.Generic;
using System.IO;
using Thriot.Framework;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.DataAccess;
using Thriot.Management.Model;

namespace Thriot.CreateAzureStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            SettingReolver.Instance.Initialize(args);

            var localConnectionParametersResolver = new LocalConnectionParametersResolver();
            
            var management = new AzureCloudStorageClientFactory(localConnectionParametersResolver);
            var tableEntityOperations = management.GetTableEntityOperation();

            tableEntityOperations.EnsureTable("LoginUser");;
            tableEntityOperations.EnsureTable("User");
            tableEntityOperations.EnsureTable("Company");
            tableEntityOperations.EnsureTable("Service");
            tableEntityOperations.EnsureTable("Network");
            tableEntityOperations.EnsureTable("Device");
            tableEntityOperations.EnsureTable("Setting");

            var settingRepository = new GenericRepository<SettingTableEntity>(tableEntityOperations, "Setting");

            CreateSettingIfNotExist(settingRepository, Setting.ServiceProfile, "runtime.serviceprofile", ServiceProfile.ServiceProvider.ToString());
            CreateSettingIfNotExist(settingRepository, Setting.EmailActivation, "runtime.emailactivation", "false");
            CreateSettingIfNotExist(settingRepository, Setting.TelemetrySetupServiceEndpoint, "microservice.telemetrysetupserviceendpoint", "http://localhost:8001/v1/telemetryDataSinkSetup");
            CreateSettingIfNotExist(settingRepository, Setting.TelemetrySetupServiceApiKey, null, Crypto.GenerateSafeRandomToken());
            CreateSettingIfNotExist(settingRepository, Setting.MessagingServiceEndpoint, "microservice.messagingserviceendpoint", "http://localhost:8003/v1/messaging");
            CreateSettingIfNotExist(settingRepository, Setting.MessagingServiceApiKey, null, Crypto.GenerateSafeRandomToken());
            CreateSettingIfNotExist(settingRepository, SettingId.GetConnection("TelemetryConnectionAzure"), "telemetry.connectionazure", "UseDevelopmentStorage=true");
            CreateSettingIfNotExist(settingRepository, SettingId.GetConnection("TelemetryConnectionSql"), "telemetry.connectionsql", @"Server=.\SQLEXPRESS;Database=ThriotTelemetry;Trusted_Connection=True;");
            CreateSettingIfNotExist(settingRepository, Setting.WebsiteUrl, "publicurl.web", "http://localhost:7999");
            CreateSettingIfNotExist(settingRepository, Setting.ManagementApiUrl, "publicurl.management", "http://localhost:8000/v1");
            CreateSettingIfNotExist(settingRepository, Setting.PlatformApiUrl, "publicurl.platformapi", "http://localhost:8001/v1");
            CreateSettingIfNotExist(settingRepository, Setting.PlatformWsUrl, "publicurl.platformwebsocket", "ws://localhost:8080");
            CreateSettingIfNotExist(settingRepository, Setting.ReportingApiUrl, "publicurl.reportingapi", "http://localhost:8002/v1");

            CreateOrUpdateSetting(settingRepository, new SettingId("Version", "Database"), "1");
        }

        private static void CreateSettingIfNotExist(GenericRepository<SettingTableEntity> settingRepository,
            SettingId settingId, string configurationSlot, string defaultValue)
        {
            var partitionKeyRowKeyPair = new PartionKeyRowKeyPair(settingId.Category, settingId.Config);

            if (settingRepository.Get(partitionKeyRowKeyPair) == null)
            {
                var value = SettingReolver.Instance.Resolve(configurationSlot, defaultValue);

                var settingTableEntity = new SettingTableEntity(settingId, value);
                settingRepository.Create(settingTableEntity);
                Console.WriteLine("Added {0}", settingId);
            }
        }

        private static void CreateOrUpdateSetting(GenericRepository<SettingTableEntity> settingRepository, SettingId settingId, string value)
        {
            var partitionKeyRowKeyPair = new PartionKeyRowKeyPair(settingId.Category, settingId.Config);

            var settingTableEntity = settingRepository.Get(partitionKeyRowKeyPair);

            if (settingTableEntity == null)
            {
                settingTableEntity = new SettingTableEntity(settingId, value);
                settingRepository.Create(settingTableEntity);
                Console.WriteLine("Added {0}", settingId);
            }
            else
            {
                if (settingTableEntity.Value != value)
                {
                    settingTableEntity.Value = value;
                    settingRepository.Update(settingTableEntity);
                    Console.WriteLine("Updated {0}", settingId);
                }
            }
        }

        public class SettingTableEntity : PreparableTableEntity
        {
            public string Value { get; set; }

            public SettingTableEntity()
            {
            }

            public SettingTableEntity(SettingId settingId, string value)
            {
                PartitionKey = settingId.Category;
                RowKey = settingId.Config;

                Value = value;
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
