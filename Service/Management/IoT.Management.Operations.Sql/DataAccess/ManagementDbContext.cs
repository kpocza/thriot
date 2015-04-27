using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Thriot.Framework.Sql;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public abstract class ManagementDbContext : DbContext
    {
        protected ManagementDbContext(DbConnection dbConnection, bool ownsConnections) : base(dbConnection, ownsConnections)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }

        protected ManagementDbContext()
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
            modelBuilder.Entity<Device>().Property(p => p.DeviceKey).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.Id).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.NetworkKey).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.Id).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.ApiKey).HasColumnType("varchar").IsFixedLength();
            modelBuilder.Entity<User>().Property(p => p.Id).HasColumnType("char").IsFixedLength();
            modelBuilder.Entity<User>().Property(p => p.ActivationCode).HasColumnType("varchar");
            modelBuilder.Entity<LoginUser>().Property(p => p.PasswordHash).HasColumnType("varchar");
            modelBuilder.Entity<LoginUser>().Property(p => p.Salt).HasColumnType("varchar");
            modelBuilder.Entity<LoginUser>().Property(p => p.UserId).HasColumnType("char").IsFixedLength();
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
