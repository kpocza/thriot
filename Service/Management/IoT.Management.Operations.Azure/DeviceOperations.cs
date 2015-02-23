using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.Management.Operations.Azure.DataAccess;

namespace IoT.Management.Operations.Azure
{
    public class DeviceOperations : IDeviceOperations
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        public DeviceOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
        }

        public string Create(Device device)
        {
            var deviceIdentity = Identity.Next();

            var deviceKey = PartionKeyRowKeyPair.CreateFromIdentity(deviceIdentity);

            var deviceRepository = new DeviceRepository(_tableEntityOperation);
            var networkRepository = new NetworkRepository(_tableEntityOperation);

            var deviceTableEntity = new DeviceTableEntity(deviceKey, device.Name, device.Network.Id, device.Service.Id,
                device.Company.Id, device.DeviceKey, device.NumericId);

            deviceRepository.Create(deviceTableEntity);

            TransientErrorHandling.Run(() =>
            {
                var parentNetworkKey = PartionKeyRowKeyPair.CreateFromIdentity(device.Network.Id);

                var parentNetwork = networkRepository.Get(parentNetworkKey);
                parentNetwork.Devices.Add(new Small() {Id = deviceIdentity, Name = device.Name});
                networkRepository.Update(parentNetwork);
            });

            return deviceIdentity;
        }

        public Device Get(string id)
        {
            var deviceKey = PartionKeyRowKeyPair.CreateFromIdentity(id);

            var deviceRepository = new DeviceRepository(_tableEntityOperation);

            var deviceTableEntity = deviceRepository.Get(deviceKey);

            if (deviceTableEntity == null)
                throw new NotFoundException();

            return new Device
            {
                Id = id,
                Name = deviceTableEntity.Name,
                Network = new Network() { Id = deviceTableEntity.NetworkId },
                Service = new Service() { Id = deviceTableEntity.ServiceId },
                Company = new Company() { Id = deviceTableEntity.CompanyId },
                DeviceKey =  deviceTableEntity.DeviceKey,
                NumericId = deviceTableEntity.NumericId
            };
        }

        public void Update(Device device)
        {
            var deviceKey = PartionKeyRowKeyPair.CreateFromIdentity(device.Id);

            var deviceRepository = new DeviceRepository(_tableEntityOperation);
            var networkRepository = new NetworkRepository(_tableEntityOperation);

            var deviceTableEntity = deviceRepository.Get(deviceKey);
            if (deviceTableEntity == null)
                throw new NotFoundException();

            deviceTableEntity.Name = device.Name;
            deviceTableEntity.DeviceKey = device.DeviceKey;
            deviceTableEntity.NumericId = device.NumericId;

            deviceRepository.Update(deviceTableEntity);

            TransientErrorHandling.Run(() =>
            {
                var parentNetworkKey = PartionKeyRowKeyPair.CreateFromIdentity(deviceTableEntity.NetworkId);

                var parentNetwork = networkRepository.Get(parentNetworkKey);
                for (var idx = 0; idx < parentNetwork.Devices.Count; idx++)
                {
                    if (parentNetwork.Devices[idx].Id == device.Id)
                    {
                        parentNetwork.Devices[idx].Name = device.Name;
                        break;
                    }
                }
                networkRepository.Update(parentNetwork);
            });
        }

        public void Delete(string id)
        {
            var deviceKey = PartionKeyRowKeyPair.CreateFromIdentity(id);

            var deviceRepository = new DeviceRepository(_tableEntityOperation);
            var networkRepository = new NetworkRepository(_tableEntityOperation);

            var deviceTableEntity = deviceRepository.Get(deviceKey);
            if (deviceTableEntity == null)
                throw new NotFoundException();

            var parentNetworkKey = PartionKeyRowKeyPair.CreateFromIdentity(deviceTableEntity.NetworkId);

            deviceRepository.Delete(deviceTableEntity);

            TransientErrorHandling.Run(() =>
            {
                var parentNetwork = networkRepository.Get(parentNetworkKey);
                for (var idx = 0; idx < parentNetwork.Devices.Count; idx++)
                {
                    if (parentNetwork.Devices[idx].Id == id)
                    {
                        parentNetwork.Devices.RemoveAt(idx);
                        break;
                    }
                }
                networkRepository.Update(parentNetwork);
            });
        }
    }
}