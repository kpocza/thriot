using System.Collections.Generic;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;

namespace Thriot.Objects.Common.CachingOperations
{
    public class NetworkOperations : CachingBase<Network>, INetworkOperations
    {
        private readonly IPersistedNetworkOperations _networkOperations;

        protected override string Prefix
        {
            get { return "Network"; }
        }

        public NetworkOperations(IPersistedNetworkOperations networkOperations)
        {
            _networkOperations = networkOperations;
        }

        public Network Get(string id)
        {
            return Get(id, internalId => _networkOperations.Get((string)internalId));
        }

        public IEnumerable<Small> ListDevices(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
