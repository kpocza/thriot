using Microsoft.Data.Entity;

namespace Thriot.Objects.Operations.Sql.DataAccess.Sql
{
    public class ObjectsDbContextSql : ObjectsDbContext
    {
        private readonly string _connectionString;

        public ObjectsDbContextSql(string connectionString)
        {
            _connectionString = connectionString;
//            Database.SetInitializer<ObjectsDbContextSql>(null);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
