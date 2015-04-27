using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;
using Thriot.Framework.Exceptions;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Objects.Operations.Azure.DataAccess;

namespace Thriot.Objects.Operations.Azure
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