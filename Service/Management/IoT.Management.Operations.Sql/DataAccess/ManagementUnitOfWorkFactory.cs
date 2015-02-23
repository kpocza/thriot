using IoT.Framework.DataAccess;

namespace IoT.Management.Operations.Sql.DataAccess
{
    public class ManagementUnitOfWorkFactory : IManagementUnitOfWorkFactory
    {
        private readonly string _connectionString;
        private readonly string _connectionProvider;

        public bool EnableMigrations { get; set; }

        public ManagementUnitOfWorkFactory(IConnectionParametersResolver connectionParametersResolver)
        {
            _connectionString = connectionParametersResolver.ManagementConnectionString;
            _connectionProvider = connectionParametersResolver.ManagementConnectionProvider;

            EnableMigrations = true;
        }

        public IManagementUnitOfWork Create()
        {
            var unitOfWork = new ManagementUnitOfWork();
            unitOfWork.Setup(_connectionString, _connectionProvider, EnableMigrations);

            return unitOfWork;
        }
    }
}
