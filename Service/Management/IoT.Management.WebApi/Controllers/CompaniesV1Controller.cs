using System.Collections.Generic;
using System.Web.Http;
using IoT.Framework.Logging;
using IoT.Management.Dto;
using IoT.Management.Services;
using IoT.Management.WebApi.Auth;

namespace IoT.Management.WebApi.Controllers
{
    [RoutePrefix("v1/companies")]
    [WebApiAuthenticator]
    public class CompaniesV1Controller : ApiController, ILoggerOwner
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

        [Route("")]
        public IEnumerable<SmallDto> Get() // GET: api/v1/companies
        {
            var companies = _userService.ListCompanies();

            return companies;
        }

        [Route("{id}")]
        public CompanyDto Get(string id) // GET: api/v1/companies/5
        {
            return _companyService.Get(id);
        }

        [Route("")]
        public string Post(CompanyDto companyDto) // POST: api/v1/companies
        {
            return _companyService.Create(companyDto.Name);
        }

        [Route("")]
        public void Put(CompanyDto companyDto) // PUT: api/v1/companies
        {
            _companyService.Update(companyDto);
        }

        [Route("{id}")]
        public void Delete(string id) // DELETE: api/v1/companies/5
        {
            _companyService.Delete(id);
        }

        [Route("{id}/services")]
        [HttpGet]
        public IEnumerable<SmallDto> GetServices(string id) // GET: api/v1/companies/5/services
        {
            return _companyService.ListServices(id);
        }

        [Route("{id}/users")]
        [HttpGet]
        public IEnumerable<SmallUserDto> GetUsers(string id) // GET: api/v1/companies/5/users
        {
            return _companyService.ListUsers(id);
        }

        [Route("{id}/incomingTelemetryDataSinks")]
        public void Post(string id, [FromBody] List<TelemetryDataSinkParametersDto> telemetryDataSinkParametersDto)
            // POST: api/v1/companies/5/incomingTelemetryDataSinks
        {
            _companyService.UpdateIncomingTelemetryDataSinks(id, telemetryDataSinkParametersDto);
        }

        [Route("adduser")]
        [HttpPost]
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
