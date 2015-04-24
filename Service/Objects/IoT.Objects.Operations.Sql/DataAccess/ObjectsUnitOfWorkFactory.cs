using IoT.Framework.DataAccess;

namespace IoT.Objects.Operations.Sql.DataAccess
{
    public class ObjectsUnitOfWorkFactory : IObjectsUnitOfWorkFactory
    {
        private readonly string _connectionString;
        private readonly string _connectionProvider;

        public ObjectsUnitOfWorkFactory(IConnectionParametersResolver connectionParametersResolver)
        {
            _connectionString = connectionParametersResolver.ManagementConnectionString;
            _connectionProvider = connectionParametersResolver.ManagementConnectionProvider;
        }

        public IObjectsUnitOfWork Create()
        {
            var unitOfWork = new ObjectsUnitOfWork();
            unitOfWork.Setup(_connectionString, _connectionProvider);

            return unitOfWork;
        }
    }
}
