using IoT.Objects.Model;

namespace IoT.UnitTestHelpers
{
    public static class TestDataCreator
    {
        public static Device Device(string id, string deviceKey, string networkId, string serviceId, string companyId,
            long numericId)
        {
            return new Device
            {
                Id = id,
                DeviceKey = deviceKey,
                NetworkId = networkId,
                ServiceId = serviceId,
                CompanyId = companyId,
                NumericId = numericId
            };
        }

        public static Network Network(string id, string networkKey, string parentNetworkId, string serviceId, string companyId, TelemetryDataSinkSettings telemetryDataSinkSettings)
        {
            return new Network
            {
                Id = id,
                NetworkKey = networkKey,
                ParentNetworkId = parentNetworkId,
                ServiceId = serviceId,
                CompanyId = companyId,
                TelemetryDataSinkSettings = telemetryDataSinkSettings
            };
        }

        public static Service Service(string id, string apiKey, string companyId,
            TelemetryDataSinkSettings telemetryDataSinkSettings)
        {
            return new Service
            {
                Id = id,
                ApiKey = apiKey,
                CompanyId = companyId,
                TelemetryDataSinkSettings = telemetryDataSinkSettings
            };
        }

        public static Company Company(string id, TelemetryDataSinkSettings telemetryDataSinkSettings)
        {
            return new Company
            {
                Id = id,
                TelemetryDataSinkSettings = telemetryDataSinkSettings
            };
        }

        public static Setting Setting(SettingId id, string value)
        {
            return new Setting(id, value);
        }
    }
}
