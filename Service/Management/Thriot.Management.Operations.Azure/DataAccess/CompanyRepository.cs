using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;

namespace Thriot.Management.Operations.Azure.DataAccess
{
    public class CompanyRepository : Repository<CompanyTableEntity>
    {
        public CompanyRepository(ITableEntityOperation tableEntityOperation) : base(tableEntityOperation)
        {
        }

        protected override string TableName
        {
            get { return "Company"; }
        }
    }
}
