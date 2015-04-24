using System.Data.Common;
using System.Data.Entity;
using IoT.Framework.Sql;

namespace IoT.Objects.Operations.Sql.DataAccess
{
    public class ObjectsUnitOfWork : UnitOfWork, IObjectsUnitOfWork
    {
        protected override DbContext GetDbContext(string connectionString, string providerName)
        {
            var dbProviderFactory = DbProviderFactories.GetFactory(providerName);
            var dbConnection = dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = connectionString;

            return new ObjectsDbContext(dbConnection, true);
        }

        public CompanyRepository GetCompanyRepository()
        {
            return new CompanyRepository(DbContext);
        }

        public ServiceRepository GetServiceRepository()
        {
            return new ServiceRepository(DbContext);
        }

        public NetworkRepository GetNetworkRepository()
        {
            return new NetworkRepository(DbContext);
        }

        public DeviceRepository GetDeviceRepository()
        {
            return new DeviceRepository(DbContext);
        }

        public SettingRepository GetSettingRepository()
        {
            return new SettingRepository(DbContext);
        }
    }
}
