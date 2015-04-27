using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;

namespace Thriot.Objects.Operations.Azure.DataAccess
{
    public class DeviceRepository : Repository<DeviceTableEntity>
    {
        public DeviceRepository(ITableEntityOperation tableEntityOperation)
            : base(tableEntityOperation)
        {
        }

        protected override string TableName
        {
            get { return "Device"; }
        }
    }
}
