using System.Collections.Generic;

namespace Thriot.Management.Model.Operations
{
    public interface IServiceOperations
    {
        string Create(Service service);

        Service Get(string id);

        void Update(Service service);

        void Delete(string id);

        IList<Small> ListNetworks(string serviceId);
    }
}
