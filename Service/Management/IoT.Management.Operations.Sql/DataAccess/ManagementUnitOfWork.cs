using System.Data.Common;
using System.Data.Entity;
using IoT.Framework.Sql;

namespace IoT.Management.Operations.Sql.DataAccess
{
    public class ManagementUnitOfWork : UnitOfWork, IManagementUnitOfWork
    {
        protected override DbContext GetDbContext(string connectionString, string providerName, bool enableMigrations)
        {
            var dbProviderFactory = DbProviderFactories.GetFactory(providerName);
            var dbConnection = dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = connectionString;

            return new ManagementDbContext(dbConnection, true, enableMigrations);
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
    }
}
