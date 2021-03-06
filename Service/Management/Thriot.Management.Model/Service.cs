﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Thriot.Framework;

namespace Thriot.Management.Model
{
    public class Service : IEntity, ITelemetrySinkSettingsOwner
    {
        [StringLength(32, MinimumLength = 32)]
        [Key]
        public string Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [Required]
        public Company Company { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string ApiKey { get; set; }

        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }

        [MaxLength(2048)]
        public string TelemetryDataSinkSettingsJson
        {
            get { return Serializers.ToJsonString(TelemetryDataSinkSettings); }
            set { TelemetryDataSinkSettings = Serializers.FromJsonString<TelemetryDataSinkSettings>(value); }
        }

        public ICollection<Network> Networks { get; set; }
    }
}
