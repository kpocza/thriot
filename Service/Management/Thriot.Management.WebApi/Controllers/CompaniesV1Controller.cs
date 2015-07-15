using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Thriot.Framework.Logging;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [Route("v1/companies")]
    [WebApiAuthorize]
    public class CompaniesV1Controller : Controller, ILoggerOwner
    {
        private readonly CompanyService _companyService;
        private readonly UserService _userService;
        private readonly IAuthenticationContext _authenticationContext;

        public CompaniesV1Controller(CompanyService companyService, UserService userService, IAuthenticationContext authenticationContext)
        {
            _companyService = companyService;
            _userService = userService;
            _authenticationContext = authenticationContext;
        }

        [HttpGet]
        public IEnumerable<SmallDto> ListCompanies() // GET: api/v1/companies
        {
            var companies = _userService.ListCompanies();

            return companies;
        }

        [HttpGet("{id}")]
        public CompanyDto Get(string id) // GET: api/v1/companies/5
        {
            return _companyService.Get(id);
        }

        [HttpPost]
        public string Create(CompanyDto companyDto) // POST: api/v1/companies
        {
            return _companyService.Create(companyDto.Name);
        }

        [HttpPut]
        public void Update(CompanyDto companyDto) // PUT: api/v1/companies
        {
            _companyService.Update(companyDto);
        }

        [HttpDelete("{id}")]
        public void Delete(string id) // DELETE: api/v1/companies/5
        {
            _companyService.Delete(id);
        }

        [HttpGet("{id}/services")]
        public IEnumerable<SmallDto> GetServices(string id) // GET: api/v1/companies/5/services
        {
            return _companyService.ListServices(id);
        }

        [HttpGet("{id}/users")]
        public IEnumerable<SmallUserDto> GetUsers(string id) // GET: api/v1/companies/5/users
        {
            return _companyService.ListUsers(id);
        }

        [HttpPost("{id}/incomingTelemetryDataSinks")]
        public void UpdateTelemetryDataSinkParameters(string id, [FromBody] List<TelemetryDataSinkParametersDto> telemetryDataSinkParametersDto)
            // POST: api/v1/companies/5/incomingTelemetryDataSinks
        {
            _companyService.UpdateIncomingTelemetryDataSinks(id, telemetryDataSinkParametersDto);
        }

        [HttpPost("adduser")]
        public void AddUser(CompanyUserDto companyUserDto) // POST: api/v1/companies/adduser
        {
            _companyService.AddUser(companyUserDto);
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger
        {
            get { return _logger; }
        }

        public string UserDefinedLogValue
        {
            get { return _authenticationContext.GetContextUser(); }
        }
    }
}
