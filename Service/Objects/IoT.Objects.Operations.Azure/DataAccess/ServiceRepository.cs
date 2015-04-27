using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;

namespace Thriot.Objects.Operations.Azure.DataAccess
{
    public class ServiceRepository : Repository<ServiceTableEntity>
    {
        public ServiceRepository(ITableEntityOperation tableEntityOperation)
            : base(tableEntityOperation)
        {
        }

        protected override string TableName
        {
            get { return "Service"; }
        }
    }
}
