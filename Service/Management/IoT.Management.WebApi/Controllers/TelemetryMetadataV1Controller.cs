using System.Web.Http;
using IoT.Framework.Logging;
using IoT.Management.Dto;
using IoT.Management.Services;
using IoT.Management.WebApi.Auth;

namespace IoT.Management.WebApi.Controllers
{
    [RoutePrefix("v1/telemetryMetadata")]
    [WebApiAuthenticator]
    public class TelemetryMetadataV1Controller : ApiController, ILoggerOwner
    {
        private readonly TelemetryMetadataService _telemetryMetadataService;
        private readonly IAuthenticationContext _authenticationContext;

        public TelemetryMetadataV1Controller(TelemetryMetadataService telemetryMetadataService, IAuthenticationContext authenticationContext)
        {
            _telemetryMetadataService = telemetryMetadataService;
            _authenticationContext = authenticationContext;
        }

        [Route("")]
        public TelemetryDataSinksMetadataDto Get() // GET: api/v1/telemetryMetadata
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
