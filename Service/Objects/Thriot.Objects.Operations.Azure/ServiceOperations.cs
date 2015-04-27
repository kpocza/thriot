using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;
using Thriot.Framework.Exceptions;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Objects.Operations.Azure.DataAccess;

namespace Thriot.Objects.Operations.Azure
{
    public class ServiceOperations : IServiceOperations
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        public ServiceOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
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
                ApiKey = serviceTableEntity.ApiKey,
                CompanyId = serviceTableEntity.CompanyId,
                TelemetryDataSinkSettings = serviceTableEntity.TelemetryDataSinkSettings
            };
        }
    }
}