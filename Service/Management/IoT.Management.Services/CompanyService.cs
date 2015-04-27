using System;
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
    public class CompanyService : ManagementServiceBase
    {
        private readonly ITelemetryDataSinkSetupService _telemetryDataSinkSetupService;
        private readonly ICapabilityProvider _capabilityProvider;

        public CompanyService(ICompanyOperations companyOperations, IAuthenticationContext authenticationContext, ITelemetryDataSinkSetupService telemetryDataSinkSetupService, ICapabilityProvider capabilityProvider)
            : base(companyOperations, authenticationContext)
        {
            _telemetryDataSinkSetupService = telemetryDataSinkSetupService;
            _capabilityProvider = capabilityProvider;
        }

        public string Create(string name)
        {
            Authenticate();

            if(!_capabilityProvider.CanCreateCompany)
                throw new ForbiddenException();

            var company = new Company
            {
                Name = Validator.TrimAndValidateAsName(name),
            };

            return _companyOperations.Create(company, _userId);
        }

        public CompanyDto Get(string id)
        {
            Authenticate();
            Validator.ValidateId(id);
            AuthorizeCompany(id);

            var company = _companyOperations.Get(id);
            return Mapper.Map<CompanyDto>(company);
        }

        public void Update(CompanyDto company)
        {
            Authenticate();
            Validator.ValidateId(company.Id);
            AuthorizeCompany(company.Id);

            var current = _companyOperations.Get(company.Id);

            current.Name = Validator.TrimAndValidateAsName(company.Name);

            _companyOperations.Update(current);
        }

        public void Delete(string id)
        {
            Authenticate();
            Validator.ValidateId(id);
            AuthorizeCompany(id);

            if (!_capabilityProvider.CanDeleteCompany)
                throw new ForbiddenException();

            if (_companyOperations.ListServices(id).Any())
                throw new NotEmptyException();

            _companyOperations.Delete(id);
        }

        public void AddUser(CompanyUserDto companyUser)
        {
            Authenticate();
            Validator.ValidateId(companyUser.CompanyId);
            Validator.ValidateId(companyUser.UserId);
            AuthorizeCompany(companyUser.CompanyId);

            _companyOperations.AddUser(companyUser.CompanyId, companyUser.UserId);
        }

        public IList<SmallUserDto> ListUsers(string id)
        {
            Authenticate();
            Validator.ValidateId(id);
            AuthorizeCompany(id);

            var users = _companyOperations.ListUsers(id);

            return Mapper.Map<IList<SmallUser>, IList<SmallUserDto>>(users);
        }

        public IList<SmallDto> ListServices(string id)
        {
            Authenticate();
            Validator.ValidateId(id);
            AuthorizeCompany(id);

            var companies = _companyOperations.ListServices(id);
            return Mapper.Map<IList<Small>, IList<SmallDto>>(companies);
        }

        public void UpdateIncomingTelemetryDataSinks(string id, List<TelemetryDataSinkParametersDto> telemetryDataSinkParametersDtos)
        {
            Authenticate();
            Validator.ValidateId(id);
            AuthorizeCompany(id);

            var telemetryDataSinksParametersRemote = new TelemetryDataSinksParametersRemoteDto
            {
                Incoming = Mapper.Map<List<TelemetryDataSinkParametersRemoteDto>>(telemetryDataSinkParametersDtos)
            };
            _telemetryDataSinkSetupService.PrepareAndValidateIncoming(telemetryDataSinksParametersRemote);

            var current = _companyOperations.Get(id);

            var telemetryDataSinkParameters = Mapper.Map<List<TelemetryDataSinkParameters>>(telemetryDataSinkParametersDtos);

            current.TelemetryDataSinkSettings.Incoming = telemetryDataSinkParameters;
            _companyOperations.Update(current);
        }
    }
}
