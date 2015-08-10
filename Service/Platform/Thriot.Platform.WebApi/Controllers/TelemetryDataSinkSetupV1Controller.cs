using Microsoft.AspNet.Mvc;
using Thriot.Framework.Logging;
using Thriot.Platform.Services.Telemetry.Dtos;
using Thriot.Platform.WebApi.Auth;

namespace Thriot.Platform.WebApi.Controllers
{
    [Route("v1/telemetryDataSinkSetup")]
    [TelemetryWebApiAuthorization]
    public class TelemetryDataSinkSetupServiceV1Controller : Controller, ILoggerOwner
    {
        private readonly Services.Telemetry.TelemetryDataSinkSetupService _telemetryDataSinkSetupService;

        public TelemetryDataSinkSetupServiceV1Controller(Services.Telemetry.TelemetryDataSinkSetupService telemetryDataSinkSetupService)
        {
            _telemetryDataSinkSetupService = telemetryDataSinkSetupService;
        }

        [HttpGet("metadata")]
        public TelemetryDataSinksMetadataDto GetMetadata() // GET: v1/telemetryDataSinkSetup/metadata
        {
            return _telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata();
        }

        [HttpPost("prepareAndValidate")]
        public void PrepareAndValidate([FromBody]TelemetryDataSinksParametersDto telemetryDataSinksParameters) // GET: v1/telemetryDataSinkSetup/prepareAndValidate
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
