using System.Collections.Generic;

namespace IoT.Objects.Model.Operations
{
    public interface IDeviceOperations
    {
        Device Get(string id);

        IEnumerable<Small> ListDevices(IEnumerable<string> ids);
    }
}
