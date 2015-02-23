using System.Data.Entity;
using IoT.Framework.Sql;
using IoT.Objects.Model;

namespace IoT.Objects.Operations.Sql.DataAccess
{
    public class SettingRepository : GenericQueryRepository<Setting>
    {
        public SettingRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
