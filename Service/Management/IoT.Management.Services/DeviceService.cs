using AutoMapper;
using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Management.Dto;
using IoT.Management.Model;
using IoT.Management.Model.Operations;
using IoT.ServiceClient.Messaging;

namespace IoT.Management.Services
{
    public class DeviceService : ManagementServiceBase
    {
        private readonly IDeviceOperations _deviceOperations;
        private readonly INetworkOperations _networkOperations;
        private readonly IServiceOperations _serviceOperations;
        private readonly IMessagingService _messagingService;

        public DeviceService(IDeviceOperations deviceOperations, INetworkOperations networkOperations, IServiceOperations serviceOperations, ICompanyOperations companyOperations, IAuthenticationContext authenticationContext, IMessagingService messagingService) :
            base(companyOperations, authenticationContext)
        {
            _deviceOperations = deviceOperations;
            _networkOperations = networkOperations;
            _serviceOperations = serviceOperations;
            _messagingService = messagingService;
        }

        public string Create(DeviceDto deviceDto)
        {
            Authenticate();
            Validator.ValidateId(deviceDto.CompanyId);
            Validator.ValidateId(deviceDto.ServiceId);
            Validator.ValidateId(deviceDto.NetworkId);
            deviceDto.Name = Validator.TrimAndValidateAsName(deviceDto.Name);

            var device = Mapper.Map<Device>(deviceDto);
            AuthorizeCompany(device.Company.Id);

            var service = _serviceOperations.Get(device.Service.Id);

            AuthorizeCompany(service.Company.Id);

            if(service.Company.Id!= device.Company.Id)
                throw new ForbiddenException();

            var parentNetwork = _networkOperations.Get(device.Network.Id);

            if (parentNetwork.Company.Id != device.Company.Id)
                throw new ForbiddenException();

            device.DeviceKey = Crypto.GenerateSafeRandomToken();

            var deviceId = _deviceOperations.Create(device);

            var deviceNumericId = _messagingService.Initialize(deviceId);

            var newDevice = _deviceOperations.Get(deviceId);
            newDevice.NumericId = deviceNumericId;
            _deviceOperations.Update(newDevice);

            return deviceId;
        }

        public DeviceDto Get(string id)
        {
            Authenticate();
            Validator.ValidateId(id);

            var device = _deviceOperations.Get(id);

            AuthorizeCompany(device.Company.Id);

            return Mapper.Map<DeviceDto>(device);
        }

        public void Update(DeviceDto device)
        {
            Authenticate();
            Validator.ValidateId(device.Id);

            var current = _deviceOperations.Get(device.Id);

            AuthorizeCompany(current.Company.Id);

            current.Name = Validator.TrimAndValidateAsName(device.Name);

            _deviceOperations.Update(current);
        }

        public void Delete(string id)
        {
            Authenticate();
            Validator.ValidateId(id);

            var current = _deviceOperations.Get(id);

            AuthorizeCompany(current.Company.Id);

            _deviceOperations.Delete(id);
        }
    }
}
