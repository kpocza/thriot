using System.Web.Http;
using Newtonsoft.Json.Linq;
using Thriot.Framework.Logging;
using Thriot.Platform.Services.Telemetry;
using Thriot.Platform.WebApi.Auth;

namespace Thriot.Platform.WebApi.Controllers
{
    [RoutePrefix("v1/telemetry")]
    [WebApiDeviceAuthenticator]
    public class TelemetryV1Controller : ApiController, ILoggerOwner
    {
        private readonly TelemetryDataService _telemetryDataService;
        private readonly AuthenticationContext _authenticationContext;

        public TelemetryV1Controller(TelemetryDataService telemetryDataService, AuthenticationContext authenticationContext)
        {
            _telemetryDataService = telemetryDataService;
            _authenticationContext = authenticationContext;
        }

        [Route("")]
        public void Post(JToken message) // POST: v1/telemetry
        {
            var deviceId = _authenticationContext.GetContextDevice(this.Request);

            _telemetryDataService.RecordTelemetryData(deviceId, message);
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger
        {
            get { return _logger; }
        }

        public string UserDefinedLogValue
        {
            get { return _authenticationContext.GetContextDevice(this.Request); }
        }
    }
}
