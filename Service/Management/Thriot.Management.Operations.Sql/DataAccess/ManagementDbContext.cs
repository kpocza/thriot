using System;
using Microsoft.Data.Entity;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public abstract class ManagementDbContext : DbContext
    {
        protected ManagementDbContext()
        {
//            Configuration.LazyLoadingEnabled = false;
  //          Configuration.ProxyCreationEnabled = false;
    //        Configuration.ValidateOnSaveEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<TelemetryDataSinkSettings>();

            modelBuilder.Entity<Company>().Property(p => p.Id).HasColumnType("char");//.IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.Id).HasColumnType("char");//.IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.DeviceKey).HasColumnType("varchar");//.IsFixedLength();

            modelBuilder.Entity<Network>().Property(p => p.Id).HasColumnType("char");//.IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.NetworkKey).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.ServiceId).HasColumnType("char");//.IsFixedLength();
            modelBuilder.Entity<Network>().HasOne(n => n.Service).WithMany(s => s.Networks).ForeignKey(n => n.ServiceId);
            modelBuilder.Entity<Network>().HasOne(n => n.ParentNetwork);

            modelBuilder.Entity<Service>().Property(p => p.Id).HasColumnType("char");//.IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.ApiKey).HasColumnType("varchar");//.IsFixedLength();

            modelBuilder.Entity<User>().Property(p => p.Id).HasColumnType("char");//.IsFixedLength();
            modelBuilder.Entity<User>().Property(p => p.ActivationCode).HasColumnType("varchar");

            modelBuilder.Entity<LoginUser>().Property(p => p.PasswordHash).HasColumnType("varchar");
            modelBuilder.Entity<LoginUser>().Property(p => p.Salt).HasColumnType("varchar");
            modelBuilder.Entity<LoginUser>().Property(p => p.UserId).HasColumnType("char");//.IsFixedLength();

            modelBuilder.Entity<Setting>().Property(p => p.Category).HasColumnType("varchar");
            modelBuilder.Entity<Setting>().Property(p => p.Config).HasColumnType("varchar");
            modelBuilder.Entity<Setting>().HasKey(s => new { s.Category, s.Config });

            modelBuilder.Entity<UserCompany>().Property(p => p.UserId).HasColumnType("char").HasColumnName("UserId").HasSqlServerColumnName("UserId");//.IsFixedLength();
            modelBuilder.Entity<UserCompany>().Property(p => p.CompanyId).HasColumnType("char").HasColumnName("CompanyId").HasSqlServerColumnName("CompanyId");//.IsFixedLength();
            modelBuilder.Entity<UserCompany>().HasKey(uc => new { uc.UserId, uc.CompanyId });
        }

        public DbSet<Device> Devices { get; set; }

        public DbSet<Network> Networks { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<LoginUser> LoginUsers { get; set; }
        
        public DbSet<Setting> Settings { get; set; }

        // WORKAROUND: EF7 beta6 lack of many-to-many relations support
        public DbSet<UserCompany> UserCompanies { get; set; }
    }
}
