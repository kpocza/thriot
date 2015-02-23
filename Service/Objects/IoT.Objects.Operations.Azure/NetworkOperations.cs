using System.Collections.Generic;
using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;
using IoT.Framework.Exceptions;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Objects.Operations.Azure.DataAccess;

namespace IoT.Objects.Operations.Azure
{
    public class NetworkOperations : INetworkOperations
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        public NetworkOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
        }

        public Network Get(string id)
        {
            var networkKey = PartionKeyRowKeyPair.CreateFromIdentity(id);

            var networkRepository = new NetworkRepository(_tableEntityOperation);

            var networkTableEntity = networkRepository.Get(networkKey);

            if (networkTableEntity == null)
                throw new NotFoundException();

            return new Network
            {
                Id = id,
                ParentNetworkId = networkTableEntity.ParentNetworkId,
                ServiceId = networkTableEntity.ServiceId,
                CompanyId = networkTableEntity.CompanyId,
                TelemetryDataSinkSettings = networkTableEntity.TelemetryDataSinkSettings,
                NetworkKey = networkTableEntity.NetworkKey
            };
        }

        public IEnumerable<Small> ListDevices(string id)
        {
            var networkKey = PartionKeyRowKeyPair.CreateFromIdentity(id);
            var networkRepository = new GenericRepository<NetworkDevicesTableEntity>(_tableEntityOperation, "Network");
            
            var network = networkRepository.Get(networkKey);

            return network.Devices;
        }
    }
}