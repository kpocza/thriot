using System.Collections.Generic;

namespace Thriot.Objects.Model.Operations
{
    public interface INetworkOperations
    {
        Network Get(string id);

        IEnumerable<Small> ListDevices(string id);
    }
}
