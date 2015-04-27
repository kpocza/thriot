using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Thriot.Framework.Sql;
using Thriot.Objects.Model;

namespace Thriot.Objects.Operations.Sql.DataAccess
{
    public abstract class ObjectsDbContext : DbContext
    {
        static ObjectsDbContext()
        {
            var ensureDllIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        protected ObjectsDbContext(DbConnection dbConnection, bool ownsConnections) : base(dbConnection, ownsConnections)
        {
            // dirty hack to make unit tests work
            // ensure that ef dlls are copied to the right place
            var _ = typeof (System.Data.Entity.SqlServer.SqlProviderServices);

            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }

        protected ObjectsDbContext()
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<TelemetryDataSinkSettings>();

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Add(new ForeignKeyNamingConvention());

            modelBuilder.Entity<Company>().Property(p => p.Id).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.Id).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.DeviceKey).HasColumnType("varchar");
            modelBuilder.Entity<Device>().Property(p => p.CompanyId).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.ServiceId).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.NetworkId).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.Id).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.NetworkKey).HasColumnType("varchar");
            modelBuilder.Entity<Network>().Property(p => p.CompanyId).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.ServiceId).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.ParentNetworkId).HasColumnType("varchar");
            modelBuilder.Entity<Service>().Property(p => p.Id).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.CompanyId).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.ApiKey).HasColumnType("varchar");
            modelBuilder.Entity<Setting>().Property(p => p.Category).HasColumnType("varchar");
            modelBuilder.Entity<Setting>().Property(p => p.Config).HasColumnType("varchar");
            modelBuilder.Entity<Setting>().HasKey(s => new { s.Category, s.Config });
        }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<Network> Networks { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Setting> Settings { get; set; }
    }
}
