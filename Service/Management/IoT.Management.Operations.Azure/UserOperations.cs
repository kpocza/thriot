using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.Management.Operations.Azure.DataAccess;

namespace IoT.Management.Operations.Azure
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
