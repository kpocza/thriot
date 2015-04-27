using System.Web.Http;
using Thriot.Framework.Logging;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [RoutePrefix("v1/info")]
    [WebApiAuthenticator]
    public class InfoV1Controller : ApiController, ILoggerOwner
    {
        private readonly InfoService _infoService;
        private readonly IAuthenticationContext _authenticationContext;

        public InfoV1Controller(InfoService infoService, IAuthenticationContext authenticationContext)
        {
            _infoService = infoService;
            _authenticationContext = authenticationContext;
        }

        [Route("")]
        [HttpGet]
        public InfoDto Get() // GET: api/v1/info
        {
            return _infoService.Get();
        }

        [Route("url")]
        [HttpGet]
        public UrlInfoDto GetUrl() // GET: api/v1/info/url
        {
            return _infoService.GetUrlInfo();
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
