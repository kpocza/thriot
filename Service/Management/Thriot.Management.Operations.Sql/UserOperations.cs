using System.Collections.Generic;
using System.Linq;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Model;
using Thriot.Management.Model.Operations;
using Thriot.Management.Operations.Sql.DataAccess;

namespace Thriot.Management.Operations.Sql
{
    public class UserOperations : IUserOperations
    {
        private readonly IManagementUnitOfWorkFactory _managementUnitOfWorkFactory;

        public UserOperations(IManagementUnitOfWorkFactory managementUnitOfWorkFactory)
        {
            _managementUnitOfWorkFactory = managementUnitOfWorkFactory;
        }

        public string Create(User user, string passwordHash, string salt)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var userIdentity = Identity.NextIncremental();

                user.Id = userIdentity;
                user.Companies = null;
                unitOfWork.GetUserRepository().Create(user);

                var loginUser = new LoginUser
                {
                    Email = user.Email,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    UserId = userIdentity
                };

                unitOfWork.GetLoginUserRepository().Create(loginUser);

                unitOfWork.Commit();

                return userIdentity;
            }
        }

        public bool IsExists(string email)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                return unitOfWork.GetLoginUserRepository().GetByEmail(email) != null;
            }
        }

        public User Get(string id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var user = unitOfWork.GetUserRepository().Get(id);
                if(user == null)
                    throw new NotFoundException();

                return user;
            }
        }

        public void Update(User user)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var userRepository = unitOfWork.GetUserRepository();

                var userEntity = userRepository.Get(user.Id);

                userEntity.Name = user.Name;
                userEntity.Activated = user.Activated;
                userEntity.ActivationCode = user.ActivationCode;

                unitOfWork.Commit();
            }
        }

        public void Update(LoginUser loginUser)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var loginUserEntity = unitOfWork.GetLoginUserRepository().GetByEmail(loginUser.Email);

                loginUserEntity.PasswordHash = loginUser.PasswordHash;
                loginUserEntity.Salt = loginUser.Salt;

                unitOfWork.Commit();
            }
        }

        public IList<Small> ListCompanies(string userIdentity)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var companyIds = unitOfWork.GetUserCompanyRepository().List(uc => uc.UserId == userIdentity).Select(uc => uc.CompanyId).ToArray();

                var companies = unitOfWork.GetCompanyRepository().List(c => companyIds.Contains(c.Id));

                return companies.Select(c => new Small {Id = c.Id, Name = c.Name}).ToList();
            }
        }

        public LoginUser GetLoginUser(string email)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var loginUser = unitOfWork.GetLoginUserRepository().GetByEmail(email);

                if(loginUser == null)
                    throw new NotFoundException();

                return loginUser;
            }
        }
    }
}
