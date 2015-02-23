using IoT.Framework.Azure.DataAccess;
using IoT.Framework.Azure.TableOperations;
using IoT.Framework.Exceptions;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Objects.Operations.Azure.DataAccess;

namespace IoT.Objects.Operations.Azure
{
    public class SettingOperations : ISettingOperations
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        public SettingOperations(ICloudStorageClientFactory cloudStorageClientFactory)
        {
            _tableEntityOperation = cloudStorageClientFactory.GetTableEntityOperation();
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
    }
}