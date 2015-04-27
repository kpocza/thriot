using Thriot.Framework.Exceptions;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Objects.Operations.Sql.DataAccess;

namespace Thriot.Objects.Operations.Sql
{
    public class ServiceOperations : IServiceOperations
    {
        private readonly IObjectsUnitOfWorkFactory _platformUnitOfWorkFactory;

        public ServiceOperations(IObjectsUnitOfWorkFactory platformUnitOfWorkFactory)
        {
            _platformUnitOfWorkFactory = platformUnitOfWorkFactory;
        }

        public Service Get(string id)
        {
            using (var unitOfWork = _platformUnitOfWorkFactory.Create())
            {
                var service = unitOfWork.GetServiceRepository().Get(id);

                if(service == null)
                    throw new NotFoundException();

                return service;
            }
        }
    }
}
