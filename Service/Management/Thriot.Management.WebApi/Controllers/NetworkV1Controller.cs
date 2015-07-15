using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Thriot.Framework.Logging;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [Route("v1/networks")]
    [WebApiAuthorize]
    public class NetworksV1Controller : Controller, ILoggerOwner
    {
        private readonly NetworkService _networkService;
        private readonly IAuthenticationContext _authenticationContext;

        public NetworksV1Controller(NetworkService networkService, IAuthenticationContext authenticationContext)
        {
            _networkService = networkService;
            _authenticationContext = authenticationContext;
        }

        [HttpGet("{id}")]
        public NetworkDto Get(string id) // GET: api/v1/networks/5
        {
            return _networkService.Get(id);
        }

        [HttpPost]
        public string CreateNetwork(NetworkDto networkDto) // POST: api/v1/networks
        {
            return _networkService.Create(networkDto);
        }

        [HttpPut]
        public void UpdateNetwork(NetworkDto networkDto) // PUT: api/v1/networks
        {
            _networkService.Update(networkDto);
        }

        [HttpDelete("{id}")]
        public void DeleteNetwork(string id) // DELETE: api/v1/networks/5
        {
            _networkService.Delete(id);
        }

        [HttpGet("{id}/networks")]
        public IEnumerable<SmallDto> GetNetworks(string id) // GET: api/v1/networks/5/networks
        {
            return _networkService.ListNetworks(id);
        }

        [HttpGet("{id}/devices")]
        public IEnumerable<SmallDto> GetDevices(string id) // GET: api/v1/networks/5/devices
        {
            return _networkService.ListDevices(id);
        }

        [HttpPost("{id}/incomingTelemetryDataSinks")]
        public void UpdateIncomingTelemetryDataSinks(string id, [FromBody]List<TelemetryDataSinkParametersDto> telemetryDataSinkParametersDto) // POST: api/v1/networks/5/incomingTelemetryDataSinks
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
