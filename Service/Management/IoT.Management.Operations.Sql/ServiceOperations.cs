using System.Collections.Generic;
using System.Linq;
using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.Management.Operations.Sql.DataAccess;

namespace IoT.Management.Operations.Sql
{
    public class ServiceOperations : IServiceOperations
    {
        private readonly IManagementUnitOfWorkFactory _managementUnitOfWorkFactory;

        public ServiceOperations(IManagementUnitOfWorkFactory managementUnitOfWorkFactory)
        {
            _managementUnitOfWorkFactory = managementUnitOfWorkFactory;
        }

        public string Create(Service service)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var serviceIdentity = Identity.NextIncremental();

                service.Id = serviceIdentity;
                service.Networks = null;
                service.Company = unitOfWork.GetCompanyRepository().Get(service.Company.Id);
                service.TelemetryDataSinkSettings = new TelemetryDataSinkSettings();

                unitOfWork.GetServiceRepository().Create(service);

                unitOfWork.Commit();

                return serviceIdentity;
            }
        }

        public Service Get(string id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var service = unitOfWork.GetServiceRepository().Get(id, s => s.Company);

                if(service == null)
                    throw new NotFoundException();

                return service;
            }
        }

        public void Update(Service service)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var serviceEntity = unitOfWork.GetServiceRepository().Get(service.Id);

                if (serviceEntity == null)
                    throw new NotFoundException();

                serviceEntity.ApiKey = service.ApiKey;
                serviceEntity.TelemetryDataSinkSettings = service.TelemetryDataSinkSettings;
                serviceEntity.Name = service.Name;

                unitOfWork.Commit();
            }
        }

        public void Delete(string id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var serviceRepository = unitOfWork.GetServiceRepository();

                var serviceEntity = serviceRepository.Get(id);

                if (serviceEntity == null)
                    throw new NotFoundException();

                serviceRepository.Delete(serviceEntity);

                unitOfWork.Commit();
            }
        }

        public IList<Small> ListNetworks(string serviceId)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var service = unitOfWork.GetServiceRepository().Get(serviceId);
                if (service == null)
                    throw new NotFoundException();

                var directChildren = unitOfWork.GetNetworkRepository().List(n => n.Service.Id == serviceId && n.ParentNetwork == null);

                return directChildren.Select(n => new Small { Id = n.Id, Name = n.Name }).ToList();
            }
        }
    }
}
