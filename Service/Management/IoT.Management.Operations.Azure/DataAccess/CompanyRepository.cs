using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;

namespace IoT.Management.Operations.Azure.DataAccess
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
