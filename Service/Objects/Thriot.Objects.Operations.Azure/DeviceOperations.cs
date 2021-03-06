﻿using System.Collections.Generic;
using System.Linq;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;
using Thriot.Framework.Exceptions;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Objects.Operations.Azure.DataAccess;

namespace Thriot.Objects.Operations.Azure
{
    public class DeviceOperations : IPersistedDeviceOperations
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        public DeviceOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
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
                DeviceKey = deviceTableEntity.DeviceKey,
                NetworkId = deviceTableEntity.NetworkId,
                ServiceId = deviceTableEntity.ServiceId,
                CompanyId = deviceTableEntity.CompanyId,
                NumericId = deviceTableEntity.NumericId
            };
        }

        public IEnumerable<Small> ListDevices(IEnumerable<string> ids)
        {
            var list = new List<Small>();
            var allIds = ids.ToList();
            var deviceRepository = new DeviceRepository(_tableEntityOperation);
            var networkRepository = new GenericRepository<NetworkDevicesTableEntity>(_tableEntityOperation, "Network");
            
            // suppose that the different ids are from a small number of networks
            while (allIds.Any())
            {
                var deviceKey = PartionKeyRowKeyPair.CreateFromIdentity(allIds.First());

                var deviceTableEntity = deviceRepository.Get(deviceKey);
                if (deviceTableEntity == null)
                {
                    allIds.RemoveAt(0);
                    continue;
                }

                var networkKey = PartionKeyRowKeyPair.CreateFromIdentity(deviceTableEntity.NetworkId);
                var network = networkRepository.Get(networkKey);

                list.AddRange(network.Devices.Where(d => allIds.Contains(d.Id)));
                var deviceIds = network.Devices.Select(d => d.Id).ToList();
                allIds.RemoveAll(deviceIds.Contains);
            }

            return list;
        }
    }
}