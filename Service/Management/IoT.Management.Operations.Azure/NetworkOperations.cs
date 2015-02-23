using System.Collections.Generic;
using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.Management.Operations.Azure.DataAccess;

namespace IoT.Management.Operations.Azure
{
    public class NetworkOperations : INetworkOperations
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        public NetworkOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
        }

        public string Create(Network network)
        {
            var networkIdentity = Identity.Next();

            var networkKey = PartionKeyRowKeyPair.CreateFromIdentity(networkIdentity);

            var networkRepository = new NetworkRepository(_tableEntityOperation);
            var networkTableEntity = new NetworkTableEntity(
                networkKey, 
                network.Name,
                network.ParentNetwork != null ? network.ParentNetwork.Id : null, 
                network.Service.Id,
                network.Company.Id, network.NetworkKey);

            networkRepository.Create(networkTableEntity);

            if (network.ParentNetwork == null)
            {
                var serviceKey = PartionKeyRowKeyPair.CreateFromIdentity(network.Service.Id);

                TransientErrorHandling.Run(() =>
                {
                    var serviceRepository = new ServiceRepository(_tableEntityOperation);

                    var service = serviceRepository.Get(serviceKey);
                    service.Networks.Add(new Small() {Id = networkIdentity, Name = network.Name});
                    serviceRepository.Update(service);
                });
            }
            else
            {
                TransientErrorHandling.Run(() =>
                {
                    var parentNetworkKey = PartionKeyRowKeyPair.CreateFromIdentity(network.ParentNetwork.Id);

                    var parentNetwork = networkRepository.Get(parentNetworkKey);
                    parentNetwork.Networks.Add(new Small() {Id = networkIdentity, Name = network.Name});
                    networkRepository.Update(parentNetwork);
                });
            }

            return networkIdentity;
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
                Name = networkTableEntity.Name,
                ParentNetwork =
                    networkTableEntity.ParentNetworkId != null
                        ? new Network() {Id = networkTableEntity.ParentNetworkId}
                        : null,
                Service = new Service() {Id = networkTableEntity.ServiceId},
                Company = new Company() {Id = networkTableEntity.CompanyId},
                TelemetryDataSinkSettings = networkTableEntity.TelemetryDataSinkSettings,
                NetworkKey = networkTableEntity.NetworkKey
            };
        }

        public void Update(Network network)
        {
            var networkKey = PartionKeyRowKeyPair.CreateFromIdentity(network.Id);

            var networkRepository = new NetworkRepository(_tableEntityOperation);

            var networkTableEntity = networkRepository.Get(networkKey);
            if (networkTableEntity == null)
                throw new NotFoundException();

            networkTableEntity.Name = network.Name;
            networkTableEntity.TelemetryDataSinkSettings = network.TelemetryDataSinkSettings;

            networkRepository.Update(networkTableEntity);

            if (networkTableEntity.ParentNetworkId == null)
            {
                var serviceKey = PartionKeyRowKeyPair.CreateFromIdentity(networkTableEntity.ServiceId);

                TransientErrorHandling.Run(() =>
                {
                    var serviceRepository = new ServiceRepository(_tableEntityOperation);

                    var service = serviceRepository.Get(serviceKey);
                    for (var idx = 0; idx < service.Networks.Count; idx++)
                    {
                        if (service.Networks[idx].Id == network.Id)
                        {
                            service.Networks[idx].Name = network.Name;
                            break;
                        }
                    }
                    serviceRepository.Update(service);
                });
            }
            else
            {
                var parentNetworkKey = PartionKeyRowKeyPair.CreateFromIdentity(networkTableEntity.ParentNetworkId);

                TransientErrorHandling.Run(() =>
                {
                    var parentNetwork = networkRepository.Get(parentNetworkKey);
                    for (var idx = 0; idx < parentNetwork.Networks.Count; idx++)
                    {
                        if (parentNetwork.Networks[idx].Id == network.Id)
                        {
                            parentNetwork.Networks[idx].Name = network.Name;
                            break;
                        }
                    }
                    networkRepository.Update(parentNetwork);
                });
            }
        }

        public void Delete(string id)
        {
            var networkKey = PartionKeyRowKeyPair.CreateFromIdentity(id);

            var networkRepository = new NetworkRepository(_tableEntityOperation);

            var networkTableEntity = networkRepository.Get(networkKey);
            if (networkTableEntity == null)
                throw new NotFoundException();

            var serviceKey = PartionKeyRowKeyPair.CreateFromIdentity(networkTableEntity.ServiceId);
            var parentNetworkKey = networkTableEntity.ParentNetworkId != null
                ? PartionKeyRowKeyPair.CreateFromIdentity(networkTableEntity.ParentNetworkId)
                : null;

            networkRepository.Delete(networkTableEntity);

            if (parentNetworkKey == null)
            {
                TransientErrorHandling.Run(() =>
                {
                    var serviceRepository = new ServiceRepository(_tableEntityOperation);

                    var service = serviceRepository.Get(serviceKey);
                    for (var idx = 0; idx < service.Networks.Count; idx++)
                    {
                        if (service.Networks[idx].Id == id)
                        {
                            service.Networks.RemoveAt(idx);
                            break;
                        }
                    }
                    serviceRepository.Update(service);
                });
            }
            else
            {
                TransientErrorHandling.Run(() =>
                {
                    var parentNetwork = networkRepository.Get(parentNetworkKey);
                    for (var idx = 0; idx < parentNetwork.Networks.Count; idx++)
                    {
                        if (parentNetwork.Networks[idx].Id == id)
                        {
                            parentNetwork.Networks.RemoveAt(idx);
                            break;
                        }
                    }
                    networkRepository.Update(parentNetwork);
                });
            }
        }

        public IList<Small> ListNetworks(string networkId)
        {
            var networkKey = PartionKeyRowKeyPair.CreateFromIdentity(networkId);

            var networkRepository = new NetworkRepository(_tableEntityOperation);

            var network = networkRepository.Get(networkKey);

            if (network == null)
                throw new NotFoundException();

            return network.Networks;
        }

        public IList<Small> ListDevices(string networkId)
        {
            var networkKey = PartionKeyRowKeyPair.CreateFromIdentity(networkId);

            var networkRepository = new NetworkRepository(_tableEntityOperation);

            var network = networkRepository.Get(networkKey);

            if (network == null)
                throw new NotFoundException();

            return network.Devices;
        }
    }
}