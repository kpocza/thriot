using System.Web.Http;
using Thriot.Framework.Logging;
using Thriot.Platform.Services.Telemetry.Dtos;
using Thriot.Platform.WebApi.Auth;
using TelemetryDataSinkSetupService = Thriot.Platform.Services.Telemetry.TelemetryDataSinkSetupService;

namespace Thriot.Platform.WebApi.Controllers
{
    [RoutePrefix("v1/telemetryDataSinkSetup")]
    [TelemetryWebApiAuthenticator]
    public class TelemetryDataSinkSetupServiceV1Controller : ApiController, ILoggerOwner
    {
        private readonly Services.Telemetry.TelemetryDataSinkSetupService _telemetryDataSinkSetupService;

        public TelemetryDataSinkSetupServiceV1Controller(Services.Telemetry.TelemetryDataSinkSetupService telemetryDataSinkSetupService)
        {
            _telemetryDataSinkSetupService = telemetryDataSinkSetupService;
        }

        [Route("metadata")]
        [HttpGet]
        public TelemetryDataSinksMetadataDto GetMetadata() // GET: v1/telemetryDataSinkSetup/metadata
        {
            return _telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata();
        }

        [Route("prepareAndValidate")]
        [HttpPost]
        public void PrepareAndValidate(TelemetryDataSinksParametersRemoteDto telemetryDataSinksParameters) // GET: v1/telemetryDataSinkSetup/prepareAndValidate
        {
            _telemetryDataSinkSetupService.PrepareAndValidateIncoming(telemetryDataSinksParameters.Incoming);
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger
        {
            get { return _logger; }
        }

        public string UserDefinedLogValue
        {
            get { return null; }
        }
    }
}
