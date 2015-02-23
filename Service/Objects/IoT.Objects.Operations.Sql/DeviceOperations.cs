using System.Collections.Generic;
using System.Linq;
using IoT.Framework.Exceptions;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Objects.Operations.Sql.DataAccess;

namespace IoT.Objects.Operations.Sql
{
    public class DeviceOperations : IDeviceOperations
    {
        private readonly IObjectsUnitOfWorkFactory _platformUnitOfWorkFactory;

        public DeviceOperations(IObjectsUnitOfWorkFactory platformUnitOfWorkFactory)
        {
            _platformUnitOfWorkFactory = platformUnitOfWorkFactory;
        }

        public Device Get(string id)
        {
            using (var unitOfWork = _platformUnitOfWorkFactory.Create())
            {
                var device = unitOfWork.GetDeviceRepository().Get(id);

                if(device == null)
                    throw new NotFoundException();

                return device;
            }
        }

        public IEnumerable<Small> ListDevices(IEnumerable<string> ids)
        {
            const int BATCH = 500;
            using (var unitOfWork = _platformUnitOfWorkFactory.Create())
            {
                var list = new List<Small>();

                for (int idx = 0; idx < ids.Count(); idx+=BATCH)
                {
                    var idBlock = ids.Skip(idx).Take(BATCH).ToList();
                    var devices = unitOfWork.GetDeviceRepository().List(d => idBlock.Contains(d.Id));
                    list.AddRange(devices.Select(d => new Small { Id = d.Id, Name = d.Name }));
                }

                return list;
            }
        }
    }
}
