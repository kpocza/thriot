using Thriot.Framework.DataAccess;

namespace Thriot.Objects.Operations.Sql.DataAccess.Sql
{
    public class ObjectsUnitOfWorkFactorySql : ObjectsUnitOfWorkFactory
    {
        public ObjectsUnitOfWorkFactorySql(IConnectionParametersResolver connectionParametersResolver)
            : base(connectionParametersResolver)
        {
        }

        protected override ObjectsUnitOfWork CreateCore()
        {
            return new ObjectsUnitOfWorkSql();
        }
    }
}
