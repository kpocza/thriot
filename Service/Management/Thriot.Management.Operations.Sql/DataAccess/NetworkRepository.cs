using Microsoft.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public class NetworkRepository : Repository<Network>
    {
        public NetworkRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
