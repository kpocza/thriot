using System;
using IoT.Framework;
using IoT.Framework.Azure.DataAccess;
using IoT.Framework.DataAccess;
using IoT.Management.Model;

namespace IoT.CreateAzureStorage
{
    class Program
    {
        static void Main(string[] args)
        {
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
        }

        private static void CreateSettingIfNotExist(GenericRepository<SettingTableEntity> settingRepository,
            SettingId settingId, string value)
        {
            var partitionKeyRowKeyPair = new PartionKeyRowKeyPair(settingId.Category, settingId.Config);

            if (settingRepository.Get(partitionKeyRowKeyPair) == null)
            {
                var settingTableEntity = new SettingTableEntity(settingId, value);
                settingRepository.Create(settingTableEntity);
                Console.WriteLine("Added {0}", settingId);
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
    }
}
