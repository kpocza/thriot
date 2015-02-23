using System.ComponentModel.DataAnnotations;

namespace IoT.Management.Model
{
    public class LoginUser
    {
        [MaxLength(128)]
        [Key]
        public string Email { get; set; }

        [MaxLength(64)]
        [Required]
        public string PasswordHash { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string Salt { get; set; }

        [StringLength(32, MinimumLength = 32)]
        [Required]
        public string UserId { get; set; }
    }
}
