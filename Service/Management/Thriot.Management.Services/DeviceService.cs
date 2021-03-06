﻿using System;
using AutoMapper;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Services.Dto;
using Thriot.Management.Model;
using Thriot.Management.Model.Operations;
using Thriot.Messaging.Services.Client;

namespace Thriot.Management.Services
{
    public class DeviceService : ManagementServiceBase
    {
        private readonly IDeviceOperations _deviceOperations;
        private readonly INetworkOperations _networkOperations;
        private readonly IServiceOperations _serviceOperations;
        private readonly IMessagingServiceClient _messagingServiceClient;

        public DeviceService(IDeviceOperations deviceOperations, INetworkOperations networkOperations, IServiceOperations serviceOperations, ICompanyOperations companyOperations, IAuthenticationContext authenticationContext, IMessagingServiceClient messagingServiceClient) :
            base(companyOperations, authenticationContext)
        {
            _deviceOperations = deviceOperations;
            _networkOperations = networkOperations;
            _serviceOperations = serviceOperations;
            _messagingServiceClient = messagingServiceClient;
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

            try
            {
                var deviceNumericId = _messagingServiceClient.Initialize(deviceId);

                var newDevice = _deviceOperations.Get(deviceId);
                newDevice.NumericId = deviceNumericId;
                _deviceOperations.Update(newDevice);
            }
            catch
            {
                try
                {
                    _deviceOperations.Delete(deviceId);
                }
                catch
                {
                    // best effort solution the remove not complete device
                }

                throw;
            }

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
