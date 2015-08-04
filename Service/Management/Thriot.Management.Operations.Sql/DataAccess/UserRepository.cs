using Microsoft.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
