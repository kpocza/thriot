using System.Web.Http;
using Thriot.Framework.Logging;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [RoutePrefix("v1/telemetryMetadata")]
    [WebApiAuthorize]
    public class TelemetryMetadataV1Controller : ApiController, IUserPrincipalContext, ILoggerOwner
    {
        private readonly TelemetryMetadataService _telemetryMetadataService;
        private readonly IAuthenticationContext _authenticationContext;

        public TelemetryMetadataV1Controller(TelemetryMetadataService telemetryMetadataService, IAuthenticationContext authenticationContext)
        {
            _telemetryMetadataService = telemetryMetadataService;
            _authenticationContext = authenticationContext;

            _telemetryMetadataService.AuthenticationContext.SetUserPrincipalContext(this);
            _authenticationContext.SetUserPrincipalContext(this);
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
