namespace Thriot.Management.Operations.Sql.DataAccess.PgSql
{
    public class ManagementUnitOfWorkPgSql : ManagementUnitOfWork
    {
        protected override ManagementDbContext GetDbContextCore(string connectionString)
        {
            return new ManagementDbContextPgSql(connectionString);
        }
    }
}
