using Microsoft.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Objects.Model;

namespace Thriot.Objects.Operations.Sql.DataAccess
{
    public class SettingRepository : GenericQueryRepository<Setting>
    {
        public SettingRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
