using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;

namespace IoT.Management.Operations.Azure.DataAccess
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
