using Microsoft.AspNet.Mvc;
using Thriot.Framework.Logging;
using Thriot.Management.Services.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [Route("v1/info")]
    [WebApiAuthorize]
    public class InfoV1Controller : Controller, ILoggerOwner
    {
        private readonly InfoService _infoService;
        private readonly IAuthenticationContext _authenticationContext;

        public InfoV1Controller(InfoService infoService, IAuthenticationContext authenticationContext)
        {
            _infoService = infoService;
            _authenticationContext = authenticationContext;
        }

        [HttpGet]
        public InfoDto Get() // GET: api/v1/info
        {
            return _infoService.Get();
        }

        [HttpGet("url")]
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
