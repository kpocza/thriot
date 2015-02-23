using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;
using IoT.Framework.Exceptions;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Objects.Operations.Azure.DataAccess;

namespace IoT.Objects.Operations.Azure
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