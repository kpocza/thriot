using System.Web.Http;
using IoT.Framework.Logging;
using IoT.Messaging.Dto;
using IoT.Messaging.Services;
using IoT.Messaging.WebApi.Auth;

namespace IoT.Messaging.WebApi.Controllers
{
    [RoutePrefix("v1/messaging")]
    [MessagingWebApiAuthenticator]
    public class MessagingV1Controller : ApiController, ILoggerOwner
    {
        private readonly MessagingService _messagingService;

        public MessagingV1Controller(MessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        [HttpGet]
        [Route("initialize/{deviceId}")]
        public long Initialize(string deviceId)
        {
            return _messagingService.Initialize(deviceId);
        }

        [HttpPost]
        [Route("enqueue")]
        public DeviceListDto Enqueue(EnqueueMessagesDto enqueueMessages)
        {
            return _messagingService.Enqueue(enqueueMessages);
        }

        [HttpPost]
        [Route("dequeue")]
        public DequeueMessagesDto Dequeue(DeviceListDto deviceList)
        {
            return _messagingService.Dequeue(deviceList);
        }

        [HttpPost]
        [Route("peek")]
        public DequeueMessagesDto Peek(DeviceListDto deviceList)
        {
            return _messagingService.Peek(deviceList);
        }

        [HttpPost]
        [Route("commit")]
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
