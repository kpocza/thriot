using System.Linq;
using IoT.Framework.Exceptions;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Objects.Operations.Sql.DataAccess;

namespace IoT.Objects.Operations.Sql
{
    public class SettingOperations : ISettingOperations
    {
        private readonly IObjectsUnitOfWorkFactory _platformUnitOfWorkFactory;

        public SettingOperations(IObjectsUnitOfWorkFactory platformUnitOfWorkFactory)
        {
            _platformUnitOfWorkFactory = platformUnitOfWorkFactory;
        }

        public Setting Get(SettingId id)
        {
            using (var unitOfWork = _platformUnitOfWorkFactory.Create())
            {
                var setting = unitOfWork.GetSettingRepository().List(s => s.Category == id.Category && s.Config == id.Config);

                if(setting.Count() != 1)
                    throw new NotFoundException();

                return setting.Single();
            }
        }
    }
}
