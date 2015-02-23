using System.Web.Http;
using IoT.Framework.Logging;
using IoT.Platform.Services.Telemetry.Dtos;
using IoT.Platform.WebApi.Auth;
using TelemetryDataSinkSetupService = IoT.Platform.Services.Telemetry.TelemetryDataSinkSetupService;

namespace IoT.Platform.WebApi.Controllers
{
    [RoutePrefix("v1/telemetryDataSinkSetup")]
    [TelemetryWebApiAuthenticator]
    public class TelemetryDataSinkSetupServiceV1Controller : ApiController, ILoggerOwner
    {
        private readonly TelemetryDataSinkSetupService _telemetryDataSinkSetupService;

        public TelemetryDataSinkSetupServiceV1Controller(TelemetryDataSinkSetupService telemetryDataSinkSetupService)
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
