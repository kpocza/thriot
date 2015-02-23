using IoT.Framework;
using IoT.Framework.Azure.DataAccess;
using IoT.Objects.Model;

namespace IoT.Objects.Operations.Azure.DataAccess
{
    public class NetworkTableEntity : PreparableTableEntity
    {
        public string Name { get; set; }

        public string ServiceId { get; set; }

        public string CompanyId { get; set; }

        public string ParentNetworkId { get; set; }

        public string NetworkKey { get; set; }

        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }
        public string TelemetryDataSinkSettingsStorage
        {
            get { return Serializers.ToJsonString(TelemetryDataSinkSettings ?? new TelemetryDataSinkSettings()); }
            set { TelemetryDataSinkSettings = Serializers.FromJsonString<TelemetryDataSinkSettings>(value); }
        }
    }
}
