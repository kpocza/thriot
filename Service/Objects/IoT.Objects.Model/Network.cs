using System.ComponentModel.DataAnnotations;
using Thriot.Framework;

namespace Thriot.Objects.Model
{
    public class Network : IEntity, ITelemetrySinkSettingsOwner
    {
        [StringLength(32, MinimumLength = 32)]
        [Key]
        public string Id { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string NetworkKey { get; set; }

        [StringLength(32, MinimumLength = 32)]
        public string ParentNetworkId { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string ServiceId { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string CompanyId { get; set; }

        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }

        [MaxLength(2048)]
        public string TelemetryDataSinkSettingsJson
        {
            get { return Serializers.ToJsonString(TelemetryDataSinkSettings); }
            set { TelemetryDataSinkSettings = Serializers.FromJsonString<TelemetryDataSinkSettings>(value); }
        }
    }
}
