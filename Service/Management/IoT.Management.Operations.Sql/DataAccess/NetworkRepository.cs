using System.Data.Entity;
using IoT.Framework.Sql;
using IoT.Management.Model;

namespace IoT.Management.Operations.Sql.DataAccess
{
    public class NetworkRepository : Repository<Network>
    {
        public NetworkRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
