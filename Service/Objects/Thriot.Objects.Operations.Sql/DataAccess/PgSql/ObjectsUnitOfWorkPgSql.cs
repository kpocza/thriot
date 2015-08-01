namespace Thriot.Objects.Operations.Sql.DataAccess.PgSql
{
    public class ObjectsUnitOfWorkPgSql : ObjectsUnitOfWork
    {
        protected override ObjectsDbContext GetDbContextCore(string connectionString)
        {
            return new ObjectsDbContextPgSql(connectionString);
        }
    }
}
