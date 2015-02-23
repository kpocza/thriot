﻿using IoT.Framework.Exceptions;
using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.Management.Operations.Azure.DataAccess;

namespace IoT.Management.Operations.Azure
{
    public class SettingOperations : ISettingOperations
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        public SettingOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
        }

        public void Create(Setting setting)
        {
            var settingKey = new PartionKeyRowKeyPair(setting.Category, setting.Config);

            var settingRepository = new SettingRepository(_tableEntityOperation);
            var settingTableEntity = new SettingTableEntity(settingKey, setting.Value);

            settingRepository.Create(settingTableEntity);
        }

        public Setting Get(SettingId id)
        {
            var settingKey = new PartionKeyRowKeyPair(id.Category, id.Config);

            var settingRepository = new SettingRepository(_tableEntityOperation);
            var settingTableEntity = settingRepository.Get(settingKey);

            if (settingTableEntity == null)
                throw new NotFoundException();

            return new Setting
            {
                Category = id.Category,
                Config = id.Config,
                Value = settingTableEntity.Value
            };
        }

        public void Update(Setting setting)
        {
            var settingKey = new PartionKeyRowKeyPair(setting.Category, setting.Config);

            var settingRepository = new SettingRepository(_tableEntityOperation);
            var settingTableEntity = settingRepository.Get(settingKey);
            
            if (settingTableEntity == null)
                throw new NotFoundException();

            settingTableEntity.Value = setting.Value;

            settingRepository.Update(settingTableEntity);
        }

        public void Delete(SettingId id)
        {
            var settingKey = new PartionKeyRowKeyPair(id.Category, id.Config);
            
            var settingRepository = new SettingRepository(_tableEntityOperation);
            var settingTableEntity = settingRepository.Get(settingKey);
            if (settingTableEntity == null)
                throw new NotFoundException();

            settingRepository.Delete(settingTableEntity);
        }
    }
}