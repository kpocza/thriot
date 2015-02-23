using IoT.Objects.Model;
using IoT.Objects.Model.Operations;

namespace IoT.Objects.Common.CachingOperations
{
    public class CompanyOperations : CachingBase<Company>, ICompanyOperations
    {
        private readonly ICompanyOperations _companyOperations;

        protected override string Prefix
        {
            get { return "Company"; }
        }

        public CompanyOperations(ICompanyOperations companyOperations)
        {
            _companyOperations = companyOperations;
        }

        public Company Get(string id)
        {
            return Get(id, internalId => _companyOperations.Get((string)internalId));
        }
    }
}
