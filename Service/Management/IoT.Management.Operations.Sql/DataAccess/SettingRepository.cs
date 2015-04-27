using System.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public class SettingRepository : GenericQueryRepository<Setting>
    {
        public SettingRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
