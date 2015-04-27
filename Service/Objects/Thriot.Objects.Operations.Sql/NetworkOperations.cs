using System.Collections.Generic;
using System.Linq;
using Thriot.Framework.Exceptions;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Objects.Operations.Sql.DataAccess;

namespace Thriot.Objects.Operations.Sql
{
    public class NetworkOperations : INetworkOperations
    {
        private readonly IObjectsUnitOfWorkFactory _platformUnitOfWorkFactory;

        public NetworkOperations(IObjectsUnitOfWorkFactory platformUnitOfWorkFactory)
        {
            _platformUnitOfWorkFactory = platformUnitOfWorkFactory;
        }
    
        public Network Get(string id)
        {
            using (var unitOfWork = _platformUnitOfWorkFactory.Create())
            {
                var network = unitOfWork.GetNetworkRepository().Get(id);

                if(network == null)
                    throw new NotFoundException();

                return network;
            }
        }

        public IEnumerable<Small> ListDevices(string id)
        {
            using (var unitOfWork = _platformUnitOfWorkFactory.Create())
            {
                return
                    unitOfWork.GetDeviceRepository()
                        .List(d => d.NetworkId == id)
                        .Select(d => new Small {Id = d.Id, Name = d.Name})
                        .ToList();
            }
        }
    }
}
