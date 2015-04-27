using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;

namespace Thriot.Management.Operations.Azure.DataAccess
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
