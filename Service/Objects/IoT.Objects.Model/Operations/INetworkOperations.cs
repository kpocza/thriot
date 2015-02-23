using System.Collections.Generic;

namespace IoT.Objects.Model.Operations
{
    public interface INetworkOperations
    {
        Network Get(string id);

        IEnumerable<Small> ListDevices(string id);
    }
}
