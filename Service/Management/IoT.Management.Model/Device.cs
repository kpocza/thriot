using System.ComponentModel.DataAnnotations;
using IoT.Framework;

namespace IoT.Management.Model
{
    public class Device : IEntity
    {
        [StringLength(32, MinimumLength = 32)]
        [Key]
        public string Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [Required]
        public Service Service { get; set; }

        [Required]
        public Company Company { get; set; }

        [Required]
        public Network Network { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string DeviceKey { get; set; }

        public long NumericId { get; set; }
    }
}
