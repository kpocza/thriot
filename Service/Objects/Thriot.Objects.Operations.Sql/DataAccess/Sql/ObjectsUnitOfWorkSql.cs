namespace Thriot.Objects.Operations.Sql.DataAccess.Sql
{
    public class ObjectsUnitOfWorkSql : ObjectsUnitOfWork
    {
        protected override ObjectsDbContext GetDbContextCore(string connectionString)
        {
            return new ObjectsDbContextSql(connectionString);
        }
    }
}
