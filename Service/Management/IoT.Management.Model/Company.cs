using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IoT.Framework;

namespace IoT.Management.Model
{
    public class Company : IEntity, ITelemetrySinkSettingsOwner
    {
        [StringLength(32, MinimumLength = 32)]
        [Key]
        public string Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }

        [MaxLength(2048)]
        public string TelemetryDataSinkSettingsJson
        {
            get { return Serializers.ToJsonString(TelemetryDataSinkSettings); }
            set { TelemetryDataSinkSettings = Serializers.FromJsonString<TelemetryDataSinkSettings>(value); }
        }

        public ICollection<Service> Services { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
