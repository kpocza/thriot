using Thriot.Framework.Exceptions;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Objects.Operations.Sql.DataAccess;

namespace Thriot.Objects.Operations.Sql
{
    public class CompanyOperations : ICompanyOperations
    {
        private readonly IObjectsUnitOfWorkFactory _platformUnitOfWorkFactory;

        public CompanyOperations(IObjectsUnitOfWorkFactory platformUnitOfWorkFactory)
        {
            _platformUnitOfWorkFactory = platformUnitOfWorkFactory;
        }

        public Company Get(string id)
        {
            using (var unitOfWork = _platformUnitOfWorkFactory.Create())
            {
                var company = unitOfWork.GetCompanyRepository().Get(id);
                if(company == null)
                    throw new NotFoundException();

                return company;
            }
        }
    }
}
