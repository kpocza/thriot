using Microsoft.AspNet.Mvc;
using System;
using System.Net;
using System.Text;
using Thriot.Framework.Logging;
using Thriot.Platform.Model.Messaging;
using Thriot.Platform.Services.Messaging;
using Thriot.Platform.WebApi.Auth;
using Thriot.Platform.WebApi.Models;

namespace Thriot.Platform.WebApi.Controllers
{
    [Route("v1/messages")]
    [WebApiDeviceAuthorization]
    public class MessagesV1Controller : Controller, ILoggerOwner
    {
        private readonly MessagingService _messagingService;
        private readonly AuthenticationContext _authenticationContext;

        public MessagesV1Controller(MessagingService messagingService, AuthenticationContext authenticationContext)
        {
            _messagingService = messagingService;
            _authenticationContext = authenticationContext;
        }

        [HttpPost("sendto/{id}")]
        public IActionResult PostSendToDevice(string id, [FromBody]string base64Payload) // POST: v1/messages/sendto
        {
            var payload = Encoding.UTF8.GetString(Convert.FromBase64String(base64Payload));

            var senderDeviceId = _authenticationContext.GetContextDevice(this.User);

            var result = _messagingService.RecordOutgoingMessage(senderDeviceId, id, payload);

            if(result == OutgoingState.Fail || result == OutgoingState.Throttled)
                return new HttpStatusCodeResult((int)HttpStatusCode.ServiceUnavailable);

            return new NoContentResult();
        }

        [HttpGet("forget")]
        public IActionResult Get() // GET: v1/messages/forget
        {
            var deviceId = _authenticationContext.GetContextDevice(this.User);

            var message = _messagingService.ReceiveAndForgetOutgoingMessage(deviceId);

            if (message.State == OutgoingState.Fail || message.State == OutgoingState.Throttled)
                return new HttpStatusCodeResult((int)HttpStatusCode.ServiceUnavailable);

            if (message.Message == null)
                return Json((string)null);

            return Json(new OutgoingMessageDto(message.Message));
        }

        [HttpGet("peek")]
        public IActionResult Peek() // GET: v1/messages/peek
        {
            var deviceId = _authenticationContext.GetContextDevice(this.User);

            var message = _messagingService.Peek(deviceId);

            if (message.State == OutgoingState.Fail || message.State == OutgoingState.Throttled)
                return new HttpStatusCodeResult((int)HttpStatusCode.ServiceUnavailable);

            if (message.Message == null)
                return Json((string)null);

            return Json(new OutgoingMessageDto(message.Message));
        }

        [HttpPost("commit")]
        public IActionResult Commit() // GET: v1/messages/commit
        {
            var deviceId = _authenticationContext.GetContextDevice(this.User);

            var state = _messagingService.Commit(deviceId);

            if (state == OutgoingState.Fail || state == OutgoingState.Throttled)
                return new HttpStatusCodeResult((int)HttpStatusCode.ServiceUnavailable);

            return new NoContentResult();
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger
        {
            get { return _logger; }
        }

        public string UserDefinedLogValue
        {
            get { return _authenticationContext.GetContextDevice(this.User); }
        }
    }
}
