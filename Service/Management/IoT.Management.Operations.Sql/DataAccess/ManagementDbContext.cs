using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using IoT.Framework.Sql;
using IoT.Management.Model;

namespace IoT.Management.Operations.Sql.DataAccess
{
    public class ManagementDbContext : DbContext
    {
        public ManagementDbContext(DbConnection dbConnection, bool ownsConnections, bool enableMigration = true) : base(dbConnection, ownsConnections)
        {
            if (enableMigration)
            {
                Database.SetInitializer(
                    new MigrateDatabaseToLatestVersion<ManagementDbContext, Migrations.Configuration>(true));
            }
            // dirty hack to make unit tests work
            // ensure that ef dlls are copied to the right place
            var _ = typeof (System.Data.Entity.SqlServer.SqlProviderServices);

            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }

        public ManagementDbContext()
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<TelemetryDataSinkSettings>();

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Add(new ForeignKeyNamingConvention());
            modelBuilder.Entity<Company>().Property(p => p.Id).HasColumnType("varchar");
            modelBuilder.Entity<Device>().Property(p => p.Id).HasColumnType("varchar");
            modelBuilder.Entity<Device>().Property(p => p.DeviceKey).HasColumnType("varchar");
            modelBuilder.Entity<Network>().Property(p => p.Id).HasColumnType("varchar");
            modelBuilder.Entity<Network>().Property(p => p.NetworkKey).HasColumnType("varchar");
            modelBuilder.Entity<Service>().Property(p => p.Id).HasColumnType("varchar");
            modelBuilder.Entity<Service>().Property(p => p.ApiKey).HasColumnType("varchar");
            modelBuilder.Entity<User>().Property(p => p.Id).HasColumnType("varchar");
            modelBuilder.Entity<User>().Property(p => p.ActivationCode).HasColumnType("varchar");
            modelBuilder.Entity<LoginUser>().Property(p => p.PasswordHash).HasColumnType("varchar");
            modelBuilder.Entity<LoginUser>().Property(p => p.Salt).HasColumnType("varchar");
            modelBuilder.Entity<Setting>().Property(p => p.Category).HasColumnType("varchar");
            modelBuilder.Entity<Setting>().Property(p => p.Config).HasColumnType("varchar");
            modelBuilder.Entity<Setting>().HasKey(s => new {s.Category, s.Config});
        }

        public DbSet<Device> Devices { get; set; }

        public DbSet<Network> Networks { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<LoginUser> LoginUsers { get; set; }
        
        public DbSet<Setting> Settings { get; set; }
    }
}
