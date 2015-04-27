using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Dto;
using Thriot.Management.Model;
using Thriot.Management.Model.Operations;
using Thriot.ServiceClient.TelemetrySetup;

namespace Thriot.Management.Services
{
    public class NetworkService : ManagementServiceBase
    {
        private readonly INetworkOperations _networkOperations;
        private readonly IServiceOperations _serviceOperations;
        private readonly ITelemetryDataSinkSetupService _telemetryDataSinkSetupService;

        public NetworkService(INetworkOperations networkOperations, IServiceOperations serviceOperations, ICompanyOperations companyOperations, IAuthenticationContext authenticationContext, ITelemetryDataSinkSetupService telemetryDataSinkSetupService) :
            base(companyOperations, authenticationContext)
        {
            _networkOperations = networkOperations;
            _serviceOperations = serviceOperations;
            _telemetryDataSinkSetupService = telemetryDataSinkSetupService;
        }

        public string Create(NetworkDto networkDto)
        {
            Authenticate();
            Validator.ValidateId(networkDto.CompanyId);
            Validator.ValidateId(networkDto.ServiceId);
            if(networkDto.ParentNetworkId!= null)
                Validator.ValidateId(networkDto.ParentNetworkId);
            networkDto.Name = Validator.TrimAndValidateAsName(networkDto.Name);
            
            var network = Mapper.Map<Network>(networkDto);
            AuthorizeCompany(network.Company.Id);

            var service = _serviceOperations.Get(network.Service.Id);

            AuthorizeCompany(service.Company.Id);

            if(service.Company.Id!= network.Company.Id)
                throw new ForbiddenException();

            if (network.ParentNetwork != null)
            {
                var parentNetwork = _networkOperations.Get(network.ParentNetwork.Id);

                if (parentNetwork.Company.Id != network.Company.Id)
                    throw new ForbiddenException();
            }
            network.NetworkKey = Crypto.GenerateSafeRandomToken();

            return _networkOperations.Create(network);
        }

        public NetworkDto Get(string id)
        {
            Authenticate();
            Validator.ValidateId(id);

            var network = _networkOperations.Get(id);

            AuthorizeCompany(network.Company.Id);

            return Mapper.Map<NetworkDto>(network);
        }

        public void Update(NetworkDto network)
        {
            Authenticate();
            Validator.ValidateId(network.Id);

            var current = _networkOperations.Get(network.Id);

            AuthorizeCompany(current.Company.Id);

            current.Name = Validator.TrimAndValidateAsName(network.Name);

            _networkOperations.Update(current);
        }

        public void Delete(string id)
        {
            Authenticate();
            Validator.ValidateId(id);

            var current = _networkOperations.Get(id);

            AuthorizeCompany(current.Company.Id);

            if (_networkOperations.ListNetworks(id).Any())
                throw new NotEmptyException();

            if (_networkOperations.ListDevices(id).Any())
                throw new NotEmptyException();

            _networkOperations.Delete(id);
        }

        public IList<SmallDto> ListNetworks(string networkId)
        {
            Authenticate();
            Validator.ValidateId(networkId);

            var network = _networkOperations.Get(networkId);
            AuthorizeCompany(network.Company.Id);

            var networks = _networkOperations.ListNetworks(networkId);

            return Mapper.Map<IList<Small>, IList<SmallDto>>(networks);
        }

        public IList<SmallDto> ListDevices(string networkId)
        {
            Authenticate();
            Validator.ValidateId(networkId);

            var network = _networkOperations.Get(networkId);
            AuthorizeCompany(network.Company.Id);

            var devices = _networkOperations.ListDevices(networkId);
            return Mapper.Map<IList<Small>, IList<SmallDto>>(devices);
        }

        public void UpdateIncomingTelemetryDataSinks(string id, List<TelemetryDataSinkParametersDto> telemetryDataSinkParametersDtos)
        {
            Authenticate();
            Validator.ValidateId(id);

            var current = _networkOperations.Get(id);

            AuthorizeCompany(current.Company.Id);

            var telemetryDataSinksParametersRemote = new TelemetryDataSinksParametersRemoteDto
            {
                Incoming = Mapper.Map<List<TelemetryDataSinkParametersRemoteDto>>(telemetryDataSinkParametersDtos)
            };
            _telemetryDataSinkSetupService.PrepareAndValidateIncoming(telemetryDataSinksParametersRemote);

            var telemetryDataSinkParameters = Mapper.Map<List<TelemetryDataSinkParameters>>(telemetryDataSinkParametersDtos);

            current.TelemetryDataSinkSettings.Incoming = telemetryDataSinkParameters;
            _networkOperations.Update(current);
        }
    }
}
