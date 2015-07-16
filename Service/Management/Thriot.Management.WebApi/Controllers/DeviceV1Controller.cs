using Microsoft.AspNet.Mvc;
using Thriot.Framework.Logging;
using Thriot.Management.Dto;
using Thriot.Management.Services;
using Thriot.Management.WebApi.Auth;

namespace Thriot.Management.WebApi.Controllers
{
    [Route("v1/devices")]
    [WebApiAuthorize]
    public class DevicesV1Controller : Controller, ILoggerOwner
    {
        private readonly DeviceService _deviceService;
        private readonly IAuthenticationContext _authenticationContext;

        public DevicesV1Controller(DeviceService deviceService, IAuthenticationContext authenticationContext)
        {
            _deviceService = deviceService;
            _authenticationContext = authenticationContext;
        }

        [HttpGet("{id}")]
        public DeviceDto GetDevice(string id) // GET: api/v1/devices/5
        {
            return _deviceService.Get(id);
        }

        [HttpPost]
        public string CreateDevice([FromBody]DeviceDto deviceDto) // POST: api/v1/devices
        {
            return _deviceService.Create(deviceDto);
        }

        [HttpPut]
        public void UpdateDevice([FromBody]DeviceDto deviceDto) // PUT: api/v1/devices
        {
            _deviceService.Update(deviceDto);
        }

        [HttpDelete("{id}")]
        public void DeleteDevice(string id) // DELETE: api/v1/devices/5
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
