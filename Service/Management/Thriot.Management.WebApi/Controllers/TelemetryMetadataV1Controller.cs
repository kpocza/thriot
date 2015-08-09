using Microsoft.AspNet.Mvc;
using Thriot.Framework.Logging;
using Thriot.Management.Services.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [Route("v1/telemetryMetadata")]
    [WebApiAuthorize]
    public class TelemetryMetadataV1Controller : Controller, ILoggerOwner
    {
        private readonly TelemetryMetadataService _telemetryMetadataService;
        private readonly IAuthenticationContext _authenticationContext;

        public TelemetryMetadataV1Controller(TelemetryMetadataService telemetryMetadataService, IAuthenticationContext authenticationContext)
        {
            _telemetryMetadataService = telemetryMetadataService;
            _authenticationContext = authenticationContext;
        }

        [HttpGet]
        public TelemetryDataSinksMetadataDto GetTelemetryMetadata() // GET: api/v1/telemetryMetadata
        {
            return _telemetryMetadataService.GetIncomingTelemetryDataSinksMetadata();
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
