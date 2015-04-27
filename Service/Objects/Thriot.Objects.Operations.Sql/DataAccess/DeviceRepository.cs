using System.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Objects.Model;

namespace Thriot.Objects.Operations.Sql.DataAccess
{
    public class DeviceRepository : Repository<Device>
    {
        public DeviceRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
