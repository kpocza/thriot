using Microsoft.AspNet.Mvc;
using Thriot.Framework.Logging;
using Thriot.Messaging.Dto;
using Thriot.Messaging.Services;
//using Thriot.Messaging.WebApi.Auth;

namespace Thriot.Messaging.WebApi.Controllers
{
    [Route("v1/messaging")]
  //  [MessagingWebApiAuthenticator]
    public class MessagingV1Controller : Controller, ILoggerOwner
    {
        private readonly MessagingService _messagingService;

        public MessagingV1Controller(MessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        [HttpGet("initialize/{deviceId}")]
        public long Initialize(string deviceId)
        {
            return _messagingService.Initialize(deviceId);
        }

        [HttpPost("enqueue")]
        public DeviceListDto Enqueue(EnqueueMessagesDto enqueueMessages)
        {
            return _messagingService.Enqueue(enqueueMessages);
        }

        [HttpPost("dequeue")]
        public DequeueMessagesDto Dequeue(DeviceListDto deviceList)
        {
            return _messagingService.Dequeue(deviceList);
        }

        [HttpPost("peek")]
        public DequeueMessagesDto Peek(DeviceListDto deviceList)
        {
            return _messagingService.Peek(deviceList);
        }

        [HttpPost("commit")]
        public DeviceListDto Commit(DeviceListDto deviceList)
        {
            return _messagingService.Commit(deviceList);
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger
        {
            get { return _logger; }
        }

        public string UserDefinedLogValue {
            get { return null; }
        }
    }
}
