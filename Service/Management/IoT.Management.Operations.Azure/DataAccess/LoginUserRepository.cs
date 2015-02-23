using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;

namespace IoT.Management.Operations.Azure.DataAccess
{
    public class LoginUserRepository : Repository<LoginUserTableEntity>
    {
        public LoginUserRepository(ITableEntityOperation tableEntityOperation)
            : base(tableEntityOperation)
        {
        }

        protected override string TableName
        {
            get { return "LoginUser"; }
        }
    }
}
