using IoT.Framework.DataAccess;

namespace IoT.Management.Operations.Sql.DataAccess.Sql
{
    public class ManagementUnitOfWorkFactorySql : ManagementUnitOfWorkFactory
    {
        public ManagementUnitOfWorkFactorySql(IConnectionParametersResolver connectionParametersResolver) : base(connectionParametersResolver)
        {
        }

        public override IManagementUnitOfWork CreateCore()
        {
            return new ManagementUnitOfWorkSql();
        }
    }
}
