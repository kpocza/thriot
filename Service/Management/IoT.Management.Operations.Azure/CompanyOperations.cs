using System.Collections.Generic;
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
    public class CompanyOperations : ICompanyOperations
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        public CompanyOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
        }

        public string Create(Company company, string userId)
        {
            var companyIdentity = Identity.Next();

            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(companyIdentity);
            var userKey = PartionKeyRowKeyPair.CreateFromIdentity(userId);

            var userRepository = new UserRepository(_tableEntityOperation);
            var userTableEntity = userRepository.Get(userKey);

            var companyRepository = new CompanyRepository(_tableEntityOperation);
            var companyTableEntity = new CompanyTableEntity(companyKey, company.Name,
                new List<SmallUser>
                {
                    new SmallUser {Id = userId, Name = userTableEntity.Name, Email = userTableEntity.Email}
                });
            companyRepository.Create(companyTableEntity);

            TransientErrorHandling.Run(() =>
            {
                userTableEntity = userRepository.Get(userKey);

                userTableEntity.Companies.Add(new Small {Id = companyIdentity, Name = company.Name});
                userRepository.Update(userTableEntity);
            });

            return companyIdentity;
        }

        public Company Get(string id)
        {
            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(id);

            var companyRepository = new CompanyRepository(_tableEntityOperation);

            var companyTableEntity = companyRepository.Get(companyKey);

            if (companyTableEntity == null)
                throw new NotFoundException();

            return new Company
            {
                Id = id,
                Name = companyTableEntity.Name,
                TelemetryDataSinkSettings = companyTableEntity.TelemetryDataSinkSettings
            };
        }

        public void Update(Company company)
        {
            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(company.Id);

            var companyRepository = new CompanyRepository(_tableEntityOperation);
            
            CompanyTableEntity companyTableEntity = null;

            TransientErrorHandling.Run(() =>
            {
                companyTableEntity = companyRepository.Get(companyKey);
                if (companyTableEntity == null)
                    throw new NotFoundException();

                companyTableEntity.Name = company.Name;
                companyTableEntity.TelemetryDataSinkSettings = company.TelemetryDataSinkSettings;

                companyRepository.Update(companyTableEntity);
            });

            var userIds = companyTableEntity.Users.Select(u => u.Id).ToList();

            TransientErrorHandling.Run(() =>
            {
                var userRepository = new UserRepository(_tableEntityOperation);

                foreach (var userId in userIds)
                {
                    var userKey = PartionKeyRowKeyPair.CreateFromIdentity(userId);

                    var user = userRepository.Get(userKey);
                    user.Companies.Single(c => c.Id == company.Id).Name = company.Name;
                    userRepository.Update(user);
                }
            });
        }

        public void Delete(string id)
        {
            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(id);

            var companyRepository = new CompanyRepository(_tableEntityOperation);
            var companyTableEntity = companyRepository.Get(companyKey);

            if (companyTableEntity == null)
                throw new NotFoundException();

            var userIds = companyTableEntity.Users.Select(u => u.Id).ToList();

            companyRepository.Delete(companyTableEntity);

            TransientErrorHandling.Run(() =>
            {
                var userRepository = new UserRepository(_tableEntityOperation);

                foreach (var userId in userIds)
                {
                    var userKey = PartionKeyRowKeyPair.CreateFromIdentity(userId);

                    var user = userRepository.Get(userKey);
                    user.Companies.Remove(user.Companies.Single(c => c.Id == id));
                    userRepository.Update(user);
                }
            });
        }

        public IList<SmallUser> ListUsers(string companyId)
        {
            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(companyId);

            var companyRepository = new CompanyRepository(_tableEntityOperation);

            var company = companyRepository.Get(companyKey);

            if (company == null)
                throw new NotFoundException();

            return company.Users;
        }

        public IList<Small> ListServices(string companyId)
        {
            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(companyId);

            var companyRepository = new CompanyRepository(_tableEntityOperation);

            var company = companyRepository.Get(companyKey);

            if (company == null)
                throw new NotFoundException();

            return company.Services;
        }

        public void AddUser(string companyId, string userId)
        {
            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(companyId);
            var userKey = PartionKeyRowKeyPair.CreateFromIdentity(userId);

            var companyRepository = new CompanyRepository(_tableEntityOperation);
            var userRepository = new UserRepository(_tableEntityOperation);

            var companyTableEntity = companyRepository.Get(companyKey);
            var userTableEntity = userRepository.Get(userKey);

            if (companyTableEntity == null || userTableEntity == null)
                throw new NotFoundException();

            TransientErrorHandling.Run(() =>
            {
                companyTableEntity = companyRepository.Get(companyKey);

                if (companyTableEntity.Users.All(u => u.Id != userId))
                {
                    companyTableEntity.Users.Add(new SmallUser
                    {
                        Id = userId,
                        Name = userTableEntity.Name,
                        Email = userTableEntity.Email
                    });
                    companyRepository.Update(companyTableEntity);
                }
            });

            TransientErrorHandling.Run(() =>
            {
                userTableEntity = userRepository.Get(userKey);

                if (userTableEntity.Companies.All(c => c.Id != companyId))
                {
                    userTableEntity.Companies.Add(new Small {Id = companyId, Name = companyTableEntity.Name});
                    userRepository.Update(userTableEntity);
                }
            });
        }
    }
}