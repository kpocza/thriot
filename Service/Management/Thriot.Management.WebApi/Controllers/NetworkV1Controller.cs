using System.Collections.Generic;
using System.Web.Http;
using Thriot.Framework.Logging;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [RoutePrefix("v1/networks")]
    [WebApiAuthorize]
    public class NetworksV1Controller : ApiController, IUserPrincipalContext, ILoggerOwner
    {
        private readonly NetworkService _networkService;
        private readonly IAuthenticationContext _authenticationContext;

        public NetworksV1Controller(NetworkService networkService, IAuthenticationContext authenticationContext)
        {
            _networkService = networkService;
            _authenticationContext = authenticationContext;

            _networkService.AuthenticationContext.SetUserPrincipalContext(this);
            _authenticationContext.SetUserPrincipalContext(this);
        }

        [Route("{id}")]
        public NetworkDto Get(string id) // GET: api/v1/networks/5
        {
            return _networkService.Get(id);
        }

        [Route("")]
        public string Post(NetworkDto networkDto) // POST: api/v1/networks
        {
            return _networkService.Create(networkDto);
        }

        [Route("")]
        public void Put(NetworkDto networkDto) // PUT: api/v1/networks
        {
            _networkService.Update(networkDto);
        }

        [Route("{id}")]
        public void Delete(string id) // DELETE: api/v1/networks/5
        {
            _networkService.Delete(id);
        }

        [Route("{id}/networks")]
        [HttpGet]
        public IEnumerable<SmallDto> GetNetworks(string id) // GET: api/v1/networks/5/networks
        {
            return _networkService.ListNetworks(id);
        }

        [Route("{id}/devices")]
        [HttpGet]
        public IEnumerable<SmallDto> GetDevices(string id) // GET: api/v1/networks/5/devices
        {
            return _networkService.ListDevices(id);
        }

        [Route("{id}/incomingTelemetryDataSinks")]
        public void Post(string id, [FromBody]List<TelemetryDataSinkParametersDto> telemetryDataSinkParametersDto) // POST: api/v1/networks/5/incomingTelemetryDataSinks
        {
            _networkService.UpdateIncomingTelemetryDataSinks(id, telemetryDataSinkParametersDto);
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
