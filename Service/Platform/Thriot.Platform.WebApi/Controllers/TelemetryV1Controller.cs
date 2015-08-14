using Microsoft.AspNet.Mvc;
using Newtonsoft.Json.Linq;
using Thriot.Framework.Logging;
using Thriot.Platform.Services.Telemetry;
using Thriot.Platform.Services.Telemetry.Recording;
using Thriot.Platform.WebApi.Auth;

namespace Thriot.Platform.WebApi.Controllers
{
    [Route("v1/telemetry")]
    [WebApiDeviceAuthorization]
    public class TelemetryV1Controller : Controller, ILoggerOwner
    {
        private readonly ITelemetryDataService _telemetryDataService;
        private readonly AuthenticationContext _authenticationContext;

        public TelemetryV1Controller(ITelemetryDataService telemetryDataService, AuthenticationContext authenticationContext)
        {
            _telemetryDataService = telemetryDataService;
            _authenticationContext = authenticationContext;
        }

        [HttpPost]
        public void Post([FromBody]JToken message) // POST: v1/telemetry
        {
            var deviceId = _authenticationContext.GetContextDevice(this.Context.User);

            _telemetryDataService.RecordTelemetryData(deviceId, message);
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger => _logger;

        public string UserDefinedLogValue => _authenticationContext.GetContextDevice(this.Context.User);
    }
}
