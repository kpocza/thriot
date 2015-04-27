using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;

namespace Thriot.Management.Operations.Azure.DataAccess
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
