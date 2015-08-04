using System.ComponentModel.DataAnnotations;
using Thriot.Framework;

namespace Thriot.Management.Model
{
    // WORKAROUND: Entity Framework 7 beta6 lacks support of many-to-many relationships
    public class UserCompany
    {
        [StringLength(32, MinimumLength = 32)]
        public string UserId { get; set; }

        [StringLength(32, MinimumLength = 32)]
        public string CompanyId { get; set; }
    }
}
