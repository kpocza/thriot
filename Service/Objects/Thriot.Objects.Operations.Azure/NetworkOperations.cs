using System.Collections.Generic;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;
using Thriot.Framework.Exceptions;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Objects.Operations.Azure.DataAccess;

namespace Thriot.Objects.Operations.Azure
{
    public class NetworkOperations : IPersistedNetworkOperations
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