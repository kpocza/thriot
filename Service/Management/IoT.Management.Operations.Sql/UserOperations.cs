﻿using System.Collections.Generic;
using System.Linq;
using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.Management.Operations.Sql.DataAccess;

namespace IoT.Management.Operations.Sql
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

        public IList<Small> ListCompanies(string userIdentity)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var user = unitOfWork.GetUserRepository().Get(userIdentity, u => u.Companies);

                return user.Companies.Select(c => new Small {Id = c.Id, Name = c.Name}).ToList();
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
