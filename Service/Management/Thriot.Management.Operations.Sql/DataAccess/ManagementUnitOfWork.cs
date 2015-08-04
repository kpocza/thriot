using Microsoft.Data.Entity;
using Thriot.Framework.Sql;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public abstract class ManagementUnitOfWork : UnitOfWork, IManagementUnitOfWork
    {
        protected override DbContext GetDbContext(string connectionString, string providerName)
        {
            return GetDbContextCore(connectionString);
        }

        protected abstract ManagementDbContext GetDbContextCore(string connectionString);

        public void ExecuteScript(string script)
        {
            DbContext.Database.ExecuteSqlCommand(script);
        }

        public DeviceRepository GetDeviceRepository()
        {
            return new DeviceRepository(DbContext);
        }

        public NetworkRepository GetNetworkRepository()
        {
            return new NetworkRepository(DbContext);
        }

        public ServiceRepository GetServiceRepository()
        {
            return new ServiceRepository(DbContext);
        }

        public CompanyRepository GetCompanyRepository()
        {
            return new CompanyRepository(DbContext);
        }

        public UserRepository GetUserRepository()
        {
            return new UserRepository(DbContext);
        }

        public LoginUserRepository GetLoginUserRepository()
        {
            return new LoginUserRepository(DbContext);
        }

        public SettingRepository GetSettingRepository()
        {
            return new SettingRepository(DbContext);
        }

        public UserCompanyRepository GetUserCompanyRepository()
        {
            return new UserCompanyRepository(DbContext);
        }
    }
}
