using System.Collections.Generic;

namespace Thriot.Management.Model.Operations
{
    public interface INetworkOperations
    {
        string Create(Network network);

        Network Get(string id);

        void Update(Network network);

        void Delete(string id);

        IList<Small> ListNetworks(string networkId);

        IList<Small> ListDevices(string networkId);
    }
}
