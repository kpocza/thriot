using System.Collections.Generic;

namespace Thriot.Objects.Model.Operations
{
    public interface IDeviceOperations
    {
        Device Get(string id);

        IEnumerable<Small> ListDevices(IEnumerable<string> ids);
    }

    public interface IPersistedDeviceOperations : IDeviceOperations { }
}
