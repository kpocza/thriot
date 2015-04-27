using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;

namespace Thriot.Objects.Common
{
    public class DeviceAuthenticator : IDeviceAuthenticator
    {
        private readonly IDeviceOperations _deviceOperations;
        private readonly INetworkOperations _networkOperations;
        private readonly IServiceOperations _serviceOperations;

        public DeviceAuthenticator(IDeviceOperations deviceOperations, INetworkOperations networkOperations, IServiceOperations serviceOperations)
        {
            _deviceOperations = deviceOperations;
            _networkOperations = networkOperations;
            _serviceOperations = serviceOperations;
        }

        public bool Authenticate(AuthenticationParameters deviceAuthentication)
        {
            var device = _deviceOperations.Get(deviceAuthentication.Id);

            if (device == null)
                return false;

            if (device.DeviceKey == deviceAuthentication.ApiKey)
                return true;

            var network = _networkOperations.Get(device.NetworkId);
            while (network != null)
            {
                if (network.NetworkKey == deviceAuthentication.ApiKey)
                    return true;

                network = network.ParentNetworkId != null ? _networkOperations.Get(network.ParentNetworkId) : null;
            }

            var service = _serviceOperations.Get(device.ServiceId);

            return service.ApiKey == deviceAuthentication.ApiKey;
        }
    }
}
