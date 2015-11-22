using System.ComponentModel.DataAnnotations;

namespace Thriot.Management.Model
{
    // WORKAROUND: Entity Framework 7 lacks support of many-to-many relationships
    public class UserCompany
    {
        [StringLength(32, MinimumLength = 32)]
        public string UserId { get; set; }

        [StringLength(32, MinimumLength = 32)]
        public string CompanyId { get; set; }
    }
}
