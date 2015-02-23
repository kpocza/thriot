using System.Collections.Generic;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;

namespace IoT.Objects.Common.CachingOperations
{
    public class DeviceOperations : CachingBase<Device>, IDeviceOperations
    {
        private readonly IDeviceOperations _deviceOperations;

        protected override string Prefix {
            get { return "Device"; }
        }

        public DeviceOperations(IDeviceOperations deviceOperations)
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
