using Thriot.Framework;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Objects.Model;

namespace Thriot.Objects.Operations.Azure.DataAccess
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
