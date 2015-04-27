using System.ComponentModel.DataAnnotations;
using Thriot.Framework;

namespace Thriot.Objects.Model
{
    public class Device : IEntity
    {
        [StringLength(32, MinimumLength = 32)]
        [Key]
        public string Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string DeviceKey { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string NetworkId { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string ServiceId { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string CompanyId { get; set; }

        [Required]
        public long NumericId { get; set; }
    }
}
