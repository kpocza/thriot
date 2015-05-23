using System.Collections.Generic;
using System.Web.Http;
using Thriot.Framework.Logging;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [RoutePrefix("v1/services")]
    [WebApiAuthorize]
    public class ServicesV1Controller : ApiController, IUserPrincipalContext, ILoggerOwner
    {
        private readonly ServiceService _serviceService;
        private readonly IAuthenticationContext _authenticationContext;

        public ServicesV1Controller(ServiceService serviceService, IAuthenticationContext authenticationContext)
        {
            _serviceService = serviceService;
            _authenticationContext = authenticationContext;
        
            _serviceService.AuthenticationContext.SetUserPrincipalContext(this);
            _authenticationContext.SetUserPrincipalContext(this);
        }

        [Route("{id}")]
        public ServiceDto Get(string id) // GET: api/v1/services/5
        {
            return _serviceService.Get(id);
        }

        [Route("")]
        public string Post(ServiceDto serviceDto) // POST: api/v1/services
        {
            return _serviceService.Create(serviceDto);
        }

        [Route("")]
        public void Put(ServiceDto serviceDto) // PUT: api/v1/services
        {
            _serviceService.Update(serviceDto);
        }

        [Route("{id}")]
        public void Delete(string id) // DELETE: api/v1/services/5
        {
            _serviceService.Delete(id);
        }

        [Route("{id}/networks")]
        [HttpGet]
        public IEnumerable<SmallDto> GetNetworks(string id) // GET: api/v1/services/5/networks
        {
            return _serviceService.ListNetworks(id);
        }

        [Route("{id}/incomingTelemetryDataSinks")]
        public void Post(string id, [FromBody]List<TelemetryDataSinkParametersDto> telemetryDataSinkParametersDto) // POST: api/v1/services/5/incomingTelemetryDataSinks
        {
            _serviceService.UpdateIncomingTelemetryDataSinks(id, telemetryDataSinkParametersDto);
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
