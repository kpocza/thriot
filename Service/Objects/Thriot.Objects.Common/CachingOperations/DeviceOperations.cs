using System.Collections.Generic;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;

namespace Thriot.Objects.Common.CachingOperations
{
    public class DeviceOperations : CachingBase<Device>, IDeviceOperations
    {
        private readonly IPersistedDeviceOperations _deviceOperations;

        protected override string Prefix {
            get { return "Device"; }
        }

        public DeviceOperations(IPersistedDeviceOperations deviceOperations)
        {
            _deviceOperations = deviceOperations;
        }

        public Device Get(string id)
        {
            return Get(id, internalId => _deviceOperations.Get((string)internalId));
        }

        public IEnumerable<Small> ListDevices(IEnumerable<string> ids)
        {
            throw new System.NotImplementedException();
        }
    }
}
