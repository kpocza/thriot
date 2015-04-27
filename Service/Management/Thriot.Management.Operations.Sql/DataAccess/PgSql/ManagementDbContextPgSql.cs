using System.Data.Common;
using System.Data.Entity;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess.PgSql
{
    public class ManagementDbContextPgSql : ManagementDbContext
    {
        public ManagementDbContextPgSql(DbConnection dbConnection, bool ownsConnections)
            : base(dbConnection, ownsConnections)
        {
            Database.SetInitializer<ManagementDbContextPgSql>(null);
            
            // dirty hack to make unit tests work
            // ensure that ef dlls are copied to the right place
            var _ = typeof(Npgsql.NpgsqlServices);
        }

        public ManagementDbContextPgSql()
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().Property(p => p.Id).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.Id).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.Id).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.Id).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<User>().Property(p => p.Id).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<LoginUser>().Property(p => p.UserId).HasColumnType("varchar").IsFixedLength();
        }
    }
}
