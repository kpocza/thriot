using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;

namespace IoT.Objects.Operations.Azure.DataAccess
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
