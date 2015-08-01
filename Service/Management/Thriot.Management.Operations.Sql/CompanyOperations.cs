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

                var userCompany = new UserCompany { CompanyId = companyIdentity, UserId = user.Id };

                company.Id = companyIdentity;
                company.Services = null;
                company.TelemetryDataSinkSettings = new TelemetryDataSinkSettings();
                company.Users = new List<UserCompany>() {userCompany};


                unitOfWork.GetCompanyRepository().Create(company);
                unitOfWork.GetUserCompanyRepository().Create(userCompany);

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
                companyRepository.Delete(companyEntity);

                var userCompanyRepository = unitOfWork.GetUserCompanyRepository();
                var userCompanies = userCompanyRepository.List(c => c.CompanyId == id);
                userCompanies.ToList().ForEach(uc => userCompanyRepository.Delete(uc));

                unitOfWork.Commit();
            }
        }

        public IList<SmallUser> ListUsers(string companyId)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var userIds = unitOfWork.GetUserCompanyRepository().List(uc => uc.CompanyId == companyId).Select(uc => uc.UserId).ToArray();
                if (!userIds.Any())
                    throw new NotFoundException();

                var users = unitOfWork.GetUserRepository().List(u => userIds.Contains(u.Id));

                return users.Select(u => new SmallUser {Id = u.Id, Name = u.Name, Email = u.Email }).ToList();
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

                var userCompanyRepository = unitOfWork.GetUserCompanyRepository();

                var userCompanies = userCompanyRepository.List(uc => uc.CompanyId == companyId);

                if (userCompanies.All(uc => uc.UserId != user.Id))
                {
                    var userCompany = new UserCompany {CompanyId = company.Id, UserId = user.Id};
                    company.Users.Add(userCompany);

                    userCompanyRepository.Create(userCompany);
                }

                unitOfWork.Commit();
            }
        }
    }
}
