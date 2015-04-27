using System.Linq;
using Thriot.Framework.Exceptions;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Objects.Operations.Sql.DataAccess;

namespace Thriot.Objects.Operations.Sql
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
