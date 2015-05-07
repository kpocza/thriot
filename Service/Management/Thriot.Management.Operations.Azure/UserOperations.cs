using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thriot.Framework;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;
using Thriot.Framework.Exceptions;
using Thriot.Management.Model;
using Thriot.Management.Model.Operations;
using Thriot.Management.Operations.Azure.DataAccess;

namespace Thriot.Management.Operations.Azure
{
    public class UserOperations : IUserOperations
    {        
        private readonly ITableEntityOperation _tableEntityOperation;

        public UserOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
        }

        public string Create(User user, string passwordHash, string salt)
        {
            var userIdentity = Identity.Next();

            var userRepository = new UserRepository(_tableEntityOperation);
            var loginUserRepository = new LoginUserRepository(_tableEntityOperation);
           
            var partitionKeyRowKeyPair = PartionKeyRowKeyPair.CreateFromIdentity(userIdentity);

            loginUserRepository.Create(new LoginUserTableEntity(PartitionKey(user.Email), user.Email, passwordHash, salt, userIdentity));

            TransientErrorHandling.Run(() => userRepository.Create(new UserTableEntity(partitionKeyRowKeyPair, user.Name, user.Email,
                new List<Small>(), user.Activated, user.ActivationCode)));

            return userIdentity;
        }

        public bool IsExists(string email)
        {
            var loginUserRepository = new LoginUserRepository(_tableEntityOperation);

            var loginUser = loginUserRepository.Get(new PartionKeyRowKeyPair(PartitionKey(email), email));

            return loginUser != null;
        }

        public User Get(string id)
        {
            var userKey = PartionKeyRowKeyPair.CreateFromIdentity(id);

            var userRepository = new UserRepository(_tableEntityOperation);

            var userTableEntity = userRepository.Get(userKey);
            if (userTableEntity == null)
                throw new NotFoundException();

            return new User
            {
                Id = id,
                Email = userTableEntity.Email,
                Name = userTableEntity.Name,
                Activated =  userTableEntity.Activated,
                ActivationCode = userTableEntity.ActivationCode
            };
        }

        public void Update(User user)
        {
            var userKey = PartionKeyRowKeyPair.CreateFromIdentity(user.Id);

            var userRepository = new UserRepository(_tableEntityOperation);

            var userEntity = userRepository.Get(userKey);

            userEntity.Name = user.Name;
            userEntity.Activated = user.Activated;
            userEntity.ActivationCode = user.ActivationCode;

            userRepository.Update(userEntity);
        }

        public void Update(LoginUser loginUser)
        {
            var loginUserRepository = new LoginUserRepository(_tableEntityOperation);

            var loginUserTableEntity = loginUserRepository.Get(new PartionKeyRowKeyPair(PartitionKey(loginUser.Email), loginUser.Email));

            loginUserTableEntity.PasswordHash = loginUser.PasswordHash;
            loginUserTableEntity.Salt = loginUser.Salt;

            loginUserRepository.Update(loginUserTableEntity);
        }

        public IList<Small> ListCompanies(string userIdentity)
        {
            var userKey = PartionKeyRowKeyPair.CreateFromIdentity(userIdentity);

            var userRepository = new UserRepository(_tableEntityOperation);

            return userRepository.Get(userKey).Companies;
        }

        public LoginUser GetLoginUser(string email)
        {
            var loginUserRepository = new LoginUserRepository(_tableEntityOperation);

            var loginUserTableEntity = loginUserRepository.Get(new PartionKeyRowKeyPair(PartitionKey(email), email));

            if (loginUserTableEntity == null)
                throw new NotFoundException();

            return new LoginUser
            {
                Email = email,
                PasswordHash = loginUserTableEntity.PasswordHash,
                Salt = loginUserTableEntity.Salt,
                UserId = loginUserTableEntity.UserId
            };
        }

        private static string PartitionKey(string data)
        {
            return (Math.Abs(data.Sum(c => (int)c) % 100)).ToString(CultureInfo.InvariantCulture);
        }
    }
}
