using System.Web.Http;
using Thriot.Framework.Logging;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [RoutePrefix("v1/devices")]
    [WebApiAuthorize]
    public class DevicesV1Controller : ApiController, IUserPrincipalContext, ILoggerOwner
    {
        private readonly DeviceService _deviceService;
        private readonly IAuthenticationContext _authenticationContext;

        public DevicesV1Controller(DeviceService deviceService, IAuthenticationContext authenticationContext)
        {
            _deviceService = deviceService;
            _authenticationContext = authenticationContext;
        
            _deviceService.AuthenticationContext.SetUserPrincipalContext(this);
            _authenticationContext.SetUserPrincipalContext(this);
        }

        [Route("{id}")]
        public DeviceDto Get(string id) // GET: api/v1/devices/5
        {
            return _deviceService.Get(id);
        }

        [Route("")]
        public string Post(DeviceDto deviceDto) // POST: api/v1/devices
        {
            return _deviceService.Create(deviceDto);
        }

        [Route("")]
        public void Put(DeviceDto deviceDto) // PUT: api/v1/devices
        {
            _deviceService.Update(deviceDto);
        }

        [Route("{id}")]
        public void Delete(string id) // DELETE: api/v1/devices/5
        {
            _deviceService.Delete(id);
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
