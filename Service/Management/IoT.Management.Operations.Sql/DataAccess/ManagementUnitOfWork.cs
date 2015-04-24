using System.Data.Common;
using System.Data.Entity;
using System.IO;
using IoT.Framework.Sql;

namespace IoT.Management.Operations.Sql.DataAccess
{
    public abstract class ManagementUnitOfWork : UnitOfWork, IManagementUnitOfWork
    {
        protected override DbContext GetDbContext(string connectionString, string providerName)
        {
            var dbProviderFactory = DbProviderFactories.GetFactory(providerName);
            var dbConnection = dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = connectionString;

            return GetDbContextCore(dbConnection, true);
        }

        protected abstract ManagementDbContext GetDbContextCore(DbConnection dbConnection, bool ownsConnections);

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
    }
}
