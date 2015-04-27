using System.Collections.Generic;
using System.Linq;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Model;
using Thriot.Management.Model.Operations;
using Thriot.Management.Operations.Sql.DataAccess;

namespace Thriot.Management.Operations.Sql
{
    public class CompanyOperations : ICompanyOperations
    {
        private readonly IManagementUnitOfWorkFactory _managementUnitOfWorkFactory;

        public CompanyOperations(IManagementUnitOfWorkFactory managementUnitOfWorkFactory)
        {
            _managementUnitOfWorkFactory = managementUnitOfWorkFactory;
        }

        public string Create(Company company, string userId)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var user = unitOfWork.GetUserRepository().Get(userId);

                var companyIdentity = Identity.NextIncremental();

                company.Id = companyIdentity;
                company.Services = null;
                company.TelemetryDataSinkSettings = new TelemetryDataSinkSettings();
                company.Users = new List<User>() {user};

                unitOfWork.GetCompanyRepository().Create(company);

                unitOfWork.Commit();

                return companyIdentity;
            }
        }

        public Company Get(string id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var company = unitOfWork.GetCompanyRepository().Get(id);
                if(company == null)
                    throw new NotFoundException();

                return company;
            }
        }

        public void Update(Company company)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var companyEntity = unitOfWork.GetCompanyRepository().Get(company.Id);

                companyEntity.Name = company.Name;
                companyEntity.TelemetryDataSinkSettings = company.TelemetryDataSinkSettings;

                unitOfWork.Commit();
            }
        }

        public void Delete(string id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var companyRepository = unitOfWork.GetCompanyRepository();

                var companyEntity = companyRepository.Get(id, c => c.Users);
                companyEntity.Users.Clear();
                companyRepository.Delete(companyEntity);

                unitOfWork.Commit();
            }
        }

        public IList<SmallUser> ListUsers(string companyId)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var company = unitOfWork.GetCompanyRepository().Get(companyId, c => c.Users);
                if (company == null)
                    throw new NotFoundException();

                return company.Users.Select(u => new SmallUser {Id = u.Id, Name = u.Name, Email = u.Email}).ToList();
            }
        }

        public IList<Small> ListServices(string companyId)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var company = unitOfWork.GetCompanyRepository().Get(companyId, c => c.Services);
                if (company == null)
                    throw new NotFoundException();

                return company.Services.Select(s => new Small { Id = s.Id, Name = s.Name }).ToList();
            }
        }

        public void AddUser(string companyId, string userId)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var companyRepository = unitOfWork.GetCompanyRepository();

                var company = companyRepository.Get(companyId, c => c.Users);
                if (company == null)
                    throw new NotFoundException();

                var user = unitOfWork.GetUserRepository().Get(userId);
                if (user == null)
                    throw new NotFoundException();

                if (company.Users.All(u => u.Id!= user.Id))
                {
                    company.Users.Add(user);
                }

                unitOfWork.Commit();
            }
        }
    }
}
