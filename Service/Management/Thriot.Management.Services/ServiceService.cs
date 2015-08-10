using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Services.Dto;
using Thriot.Management.Model;
using Thriot.Management.Model.Operations;
using Thriot.Platform.Services.Client;

namespace Thriot.Management.Services
{
    public class ServiceService : ManagementServiceBase
    {
        private readonly IServiceOperations _serviceOperations;
        private readonly ITelemetryDataSinkSetupServiceClient _telemetryDataSinkSetupServiceClient;
        private readonly ICapabilityProvider _capabilityProvider;

        public ServiceService(IServiceOperations serviceOperations, ICompanyOperations companyOperations, IAuthenticationContext authenticationContext, ITelemetryDataSinkSetupServiceClient telemetryDataSinkSetupServiceClient, ICapabilityProvider capabilityProvider) :
            base(companyOperations, authenticationContext)
        {
            _serviceOperations = serviceOperations;
            _telemetryDataSinkSetupServiceClient = telemetryDataSinkSetupServiceClient;
            _capabilityProvider = capabilityProvider;
        }

        public string Create(ServiceDto serviceDto)
        {
            Authenticate();
            Validator.ValidateId(serviceDto.CompanyId);
            serviceDto.Name = Validator.TrimAndValidateAsName(serviceDto.Name);

            var service = Mapper.Map<Service>(serviceDto);
            AuthorizeCompany(service.Company.Id);

            if (!_capabilityProvider.CanCreateService)
                throw new ForbiddenException();

            service.ApiKey = Crypto.GenerateSafeRandomToken();

            return _serviceOperations.Create(service);
        }

        public ServiceDto Get(string id)
        {
            Authenticate();
            Validator.ValidateId(id);

            var service = _serviceOperations.Get(id);

            AuthorizeCompany(service.Company.Id);

            return Mapper.Map<ServiceDto>(service); ;
        }

        public void Update(ServiceDto service)
        {
            Authenticate();
            Validator.ValidateId(service.Id);

            var current = _serviceOperations.Get(service.Id);

            AuthorizeCompany(current.Company.Id);

            current.Name = Validator.TrimAndValidateAsName(service.Name);

            _serviceOperations.Update(current);
        }

        public void Delete(string id)
        {
            Authenticate();
            Validator.ValidateId(id);

            var current = _serviceOperations.Get(id);

            AuthorizeCompany(current.Company.Id);

            if (!_capabilityProvider.CanDeleteService)
                throw new ForbiddenException();

            if (_serviceOperations.ListNetworks(id).Any())
                throw new NotEmptyException();

            _serviceOperations.Delete(id);
        }

        public IList<SmallDto> ListNetworks(string serviceId)
        {
            Authenticate();
            Validator.ValidateId(serviceId);

            var service = _serviceOperations.Get(serviceId);
            AuthorizeCompany(service.Company.Id);

            var networks = _serviceOperations.ListNetworks(serviceId);


            return Mapper.Map<IList<Small>, IList<SmallDto>>(networks);
        }

        public void UpdateIncomingTelemetryDataSinks(string id, List<TelemetryDataSinkParametersDto> telemetryDataSinkParametersDtos)
        {
            Authenticate();
            Validator.ValidateId(id);

            var current = _serviceOperations.Get(id);

            AuthorizeCompany(current.Company.Id);

            var telemetryDataSinksParametersRemote = new TelemetryDataSinksParametersDtoClient
            {
                Incoming = Mapper.Map<List<TelemetryDataSinkParametersDtoClient>>(telemetryDataSinkParametersDtos)
            };
            _telemetryDataSinkSetupServiceClient.PrepareAndValidateIncoming(telemetryDataSinksParametersRemote);

            var telemetryDataSinkParameters = Mapper.Map<List<TelemetryDataSinkParameters>>(telemetryDataSinkParametersDtos);

            current.TelemetryDataSinkSettings.Incoming = telemetryDataSinkParameters;
            _serviceOperations.Update(current);
        }
    }
}
