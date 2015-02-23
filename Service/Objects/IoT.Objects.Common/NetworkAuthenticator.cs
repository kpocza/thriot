
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;

namespace IoT.Objects.Common
{
    public class NetworkAuthenticator : INetworkAuthenticator
    {
        private readonly INetworkOperations _networkOperations;
        private readonly IServiceOperations _serviceOperations;

        public NetworkAuthenticator(INetworkOperations networkOperations, IServiceOperations serviceOperations)
        {
            _networkOperations = networkOperations;
            _serviceOperations = serviceOperations;
        }

        public bool Authenticate(AuthenticationParameters networkAuthentication)
        {
            var network = _networkOperations.Get(networkAuthentication.Id);
            var serviceId = network.ServiceId;

            while (network != null)
            {
                if (network.NetworkKey == networkAuthentication.ApiKey)
                    return true;

                network = network.ParentNetworkId != null ? _networkOperations.Get(network.ParentNetworkId) : null;
            }

            var service = _serviceOperations.Get(serviceId);

            return service.ApiKey == networkAuthentication.ApiKey;
        }
    }
}
