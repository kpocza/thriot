using Microsoft.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public class DeviceRepository : Repository<Device>
    {
        public DeviceRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
