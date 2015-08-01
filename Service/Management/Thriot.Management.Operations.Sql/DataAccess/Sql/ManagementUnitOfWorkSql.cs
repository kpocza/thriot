using System;

namespace Thriot.Management.Operations.Sql.DataAccess.Sql
{
    public class ManagementUnitOfWorkSql : ManagementUnitOfWork
    {
        protected override ManagementDbContext GetDbContextCore(string connectionString)
        {
            return new ManagementDbContextSql(connectionString);
        }
    }
}
