using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;

namespace IoT.Objects.Operations.Azure.DataAccess
{
    public class NetworkRepository : Repository<NetworkTableEntity>
    {
        public NetworkRepository(ITableEntityOperation tableEntityOperation)
            : base(tableEntityOperation)
        {
        }

        protected override string TableName
        {
            get { return "Network"; }
        }
    }
}
