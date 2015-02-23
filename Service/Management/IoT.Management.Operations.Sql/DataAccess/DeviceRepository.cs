using System.Data.Entity;
using IoT.Framework.Sql;
using IoT.Management.Model;

namespace IoT.Management.Operations.Sql.DataAccess
{
    public class DeviceRepository : Repository<Device>
    {
        public DeviceRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
