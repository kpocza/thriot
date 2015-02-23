using IoT.Framework;
using IoT.Framework.Azure.DataAccess;
using IoT.Objects.Model;

namespace IoT.Objects.Operations.Azure.DataAccess
{
    public class CompanyTableEntity : PreparableTableEntity
    {
        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }
        public string TelemetryDataSinkSettingsStorage
        {
            get { return Serializers.ToJsonString(TelemetryDataSinkSettings ?? new TelemetryDataSinkSettings()); }
            set { TelemetryDataSinkSettings = Serializers.FromJsonString<TelemetryDataSinkSettings>(value); }
        }
    }
}
