using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;

namespace IoT.Objects.Operations.Azure.DataAccess
{
    public class SettingRepository : Repository<SettingTableEntity>
    {
        public SettingRepository(ITableEntityOperation tableEntityOperation)
            : base(tableEntityOperation)
        {
        }

        protected override string TableName
        {
            get { return "Setting"; }
        }
    }
}
