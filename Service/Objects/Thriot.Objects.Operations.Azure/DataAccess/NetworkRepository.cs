using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;

namespace Thriot.Objects.Operations.Azure.DataAccess
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
