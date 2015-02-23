using System.Linq;
using IoT.Framework.Exceptions;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.Management.Operations.Sql.DataAccess;

namespace IoT.Management.Operations.Sql
{
    public class SettingOperations : ISettingOperations
    {
        private readonly IManagementUnitOfWorkFactory _managementUnitOfWorkFactory;

        public SettingOperations(IManagementUnitOfWorkFactory managementUnitOfWorkFactory)
        {
            _managementUnitOfWorkFactory = managementUnitOfWorkFactory;
        }

        public void Create(Setting setting)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                unitOfWork.GetSettingRepository().Create(setting);

                unitOfWork.Commit();
            }
        }

        public Setting Get(SettingId id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var setting = unitOfWork.GetSettingRepository().List(s => s.Category == id.Category && s.Config == id.Config);

                if(setting.Count() != 1)
                    throw new NotFoundException();

                return setting.Single();
            }
        }

        public void Update(Setting setting)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var settingEntity = unitOfWork.GetSettingRepository().List(s => s.Category == setting.Category && s.Config == setting.Config);

                if (settingEntity.Count() != 1)
                    throw new NotFoundException();

                settingEntity.Single().Value = setting.Value;

                unitOfWork.Commit();
            }
        }

        public void Delete(SettingId id)
        {
            using (var unitOfWork = _managementUnitOfWorkFactory.Create())
            {
                var settingRepository = unitOfWork.GetSettingRepository();
                var settingEntity = settingRepository.List(s => s.Category == id.Category && s.Config == id.Config);

                if (settingEntity.Count() != 1)
                    throw new NotFoundException();

                settingRepository.Delete(settingEntity.Single());

                unitOfWork.Commit();
            }
        }
    }
}
