using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Thriot.Framework.Logging;
using Thriot.Management.Services.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [Route("v1/services")]
    [WebApiAuthorize]
    public class ServicesV1Controller : Controller, ILoggerOwner
    {
        private readonly ServiceService _serviceService;
        private readonly IAuthenticationContext _authenticationContext;

        public ServicesV1Controller(ServiceService serviceService, IAuthenticationContext authenticationContext)
        {
            _serviceService = serviceService;
            _authenticationContext = authenticationContext;
        }

        [HttpGet("{id}")]
        public ServiceDto GetService(string id) // GET: api/v1/services/5
        {
            return _serviceService.Get(id);
        }

        [HttpPost]
        public IActionResult CreateService([FromBody]ServiceDto serviceDto) // POST: api/v1/services
        {
            return Json(_serviceService.Create(serviceDto));
        }

        [HttpPut]
        public void UpdateService([FromBody]ServiceDto serviceDto) // PUT: api/v1/services
        {
            _serviceService.Update(serviceDto);
        }

        [HttpDelete("{id}")]
        public void DeleteService(string id) // DELETE: api/v1/services/5
        {
            _serviceService.Delete(id);
        }

        [HttpGet("{id}/networks")]
        public IEnumerable<SmallDto> GetNetworks(string id) // GET: api/v1/services/5/networks
        {
            return _serviceService.ListNetworks(id);
        }

        [HttpPost("{id}/incomingTelemetryDataSinks")]
        public void UpdateIncomingTelemetryDataSinks(string id, [FromBody]List<TelemetryDataSinkParametersDto> telemetryDataSinkParametersDto) 
            // POST: api/v1/services/5/incomingTelemetryDataSinks
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
