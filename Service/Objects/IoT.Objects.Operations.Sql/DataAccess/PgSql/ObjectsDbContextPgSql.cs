using System.Data.Common;
using System.Data.Entity;
using IoT.Objects.Model;

namespace IoT.Objects.Operations.Sql.DataAccess.PgSql
{
    public class ObjectsDbContextPgSql : ObjectsDbContext
    {
        public ObjectsDbContextPgSql(DbConnection dbConnection, bool ownsConnections) : base(dbConnection, ownsConnections)
        {
            Database.SetInitializer<ObjectsDbContextPgSql>(null);

            // dirty hack to make unit tests work
            // ensure that ef dlls are copied to the right place
            var _ = typeof(Npgsql.NpgsqlServices);
        }

        public ObjectsDbContextPgSql()
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().Property(p => p.Id).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.Id).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.CompanyId).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.ServiceId).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.NetworkId).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.Id).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.CompanyId).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.ServiceId).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.Id).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.CompanyId).HasColumnType("varchar").IsFixedLength();
        }
    }
}
