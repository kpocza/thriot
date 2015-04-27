using System.Collections.Generic;
using Thriot.Framework;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;
using Thriot.Framework.Exceptions;
using Thriot.Management.Model;
using Thriot.Management.Model.Operations;
using Thriot.Management.Operations.Azure.DataAccess;

namespace Thriot.Management.Operations.Azure
{
    public class ServiceOperations : IServiceOperations
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        public ServiceOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
        }

        public string Create(Service service)
        {
            var serviceIdentity = Identity.Next();

            var serviceKey = PartionKeyRowKeyPair.CreateFromIdentity(serviceIdentity);
            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(service.Company.Id);

            var serviceRepository = new ServiceRepository(_tableEntityOperation);
            var serviceTableEntity = new ServiceTableEntity(serviceKey, service.Name, service.Company.Id, service.ApiKey);
            serviceRepository.Create(serviceTableEntity);

            TransientErrorHandling.Run(() =>
            {
                var companyRepository = new CompanyRepository(_tableEntityOperation);
                var company = companyRepository.Get(companyKey);

                company.Services.Add(new Small() { Id = serviceIdentity, Name = service.Name });
                companyRepository.Update(company);
            });

            return serviceIdentity;
        }

        public Service Get(string id)
        {
            var serviceKey = PartionKeyRowKeyPair.CreateFromIdentity(id);

            var serviceRepository = new ServiceRepository(_tableEntityOperation);

            var serviceTableEntity = serviceRepository.Get(serviceKey);

            if (serviceTableEntity == null)
                throw new NotFoundException();

            return new Service
            {
                Id = id,
                Name = serviceTableEntity.Name,
                Company = new Company() {Id = serviceTableEntity.CompanyId},
                TelemetryDataSinkSettings = serviceTableEntity.TelemetryDataSinkSettings,
                ApiKey = serviceTableEntity.ApiKey
            };
        }

        public void Update(Service service)
        {
            var serviceKey = PartionKeyRowKeyPair.CreateFromIdentity(service.Id);

            var serviceRepository = new ServiceRepository(_tableEntityOperation);

            var serviceTableEntity = serviceRepository.Get(serviceKey);
            if (serviceTableEntity == null)
                throw new NotFoundException();

            serviceTableEntity.Name = service.Name;
            serviceTableEntity.ApiKey = service.ApiKey;
            serviceTableEntity.TelemetryDataSinkSettings = service.TelemetryDataSinkSettings;

            serviceRepository.Update(serviceTableEntity);

            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(serviceTableEntity.CompanyId);

            TransientErrorHandling.Run(() =>
            {
                var companyRepository = new CompanyRepository(_tableEntityOperation);

                var company = companyRepository.Get(companyKey);
                for (var idx = 0; idx < company.Services.Count; idx++)
                {
                    if (company.Services[idx].Id == service.Id)
                    {
                        company.Services[idx].Name = service.Name;
                        break;
                    }
                }
                companyRepository.Update(company);
            });
        }

        public void Delete(string id)
        {
            var serviceKey = PartionKeyRowKeyPair.CreateFromIdentity(id);

            var serviceRepository = new ServiceRepository(_tableEntityOperation);

            var serviceTableEntity = serviceRepository.Get(serviceKey);
            if (serviceTableEntity == null)
                throw new NotFoundException();

            var companyKey = PartionKeyRowKeyPair.CreateFromIdentity(serviceTableEntity.CompanyId);

            serviceRepository.Delete(serviceTableEntity);

            TransientErrorHandling.Run(() =>
            {
                var companyRepository = new CompanyRepository(_tableEntityOperation);

                var company = companyRepository.Get(companyKey);
                for (var idx = 0; idx < company.Services.Count; idx++)
                {
                    if (company.Services[idx].Id == id)
                    {
                        company.Services.RemoveAt(idx);
                        break;
                    }
                }
                companyRepository.Update(company);
            });
        }

        public IList<Small> ListNetworks(string serviceId)
        {
            var serviceKey = PartionKeyRowKeyPair.CreateFromIdentity(serviceId);

            var serviceRepository = new ServiceRepository(_tableEntityOperation);

            var service = serviceRepository.Get(serviceKey);

            if (service == null)
                throw new NotFoundException();

            return service.Networks;
        }
    }
}