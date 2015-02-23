using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using IoT.Framework.Logging;
using IoT.Platform.Model.Messaging;
using IoT.Platform.Services.Messaging;
using IoT.Platform.WebApi.Auth;
using IoT.Platform.WebApi.Models;

namespace IoT.Platform.WebApi.Controllers
{
    [RoutePrefix("v1/messages")]
    [WebApiDeviceAuthenticator]
    public class MessagesV1Controller : ApiController, ILoggerOwner
    {
        private readonly MessagingService _messagingService;
        private readonly AuthenticationContext _authenticationContext;

        public MessagesV1Controller(MessagingService messagingService, AuthenticationContext authenticationContext)
        {
            _messagingService = messagingService;
            _authenticationContext = authenticationContext;
        }

        [Route("sendto/{id}")]
        [HttpPost]
        public HttpResponseMessage PostSendToDevice(string id, [FromBody]string base64Payload) // POST: v1/messages/sendto
        {
            var payload = Encoding.UTF8.GetString(Convert.FromBase64String(base64Payload));

            var senderDeviceId = _authenticationContext.GetContextDevice(this.Request);

            var result = _messagingService.RecordOutgoingMessage(senderDeviceId, id, payload);

            if(result == OutgoingState.Fail || result == OutgoingState.Throttled)
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [Route("forget")]
        public HttpResponseMessage Get() // GET: v1/messages/forget
        {
            var deviceId = _authenticationContext.GetContextDevice(this.Request);

            var message = _messagingService.ReceiveAndForgetOutgoingMessage(deviceId);

            if (message.State == OutgoingState.Fail || message.State == OutgoingState.Throttled)
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);

            if (message.Message == null)
                return Request.CreateResponse(HttpStatusCode.OK, (string)null);

            return Request.CreateResponse(HttpStatusCode.OK, new OutgoingMessageDto(message.Message));
        }

        [Route("peek")]
        [HttpGet]
        public HttpResponseMessage Peek() // GET: v1/messages/peek
        {
            var deviceId = _authenticationContext.GetContextDevice(this.Request);

            var message = _messagingService.Peek(deviceId);

            if (message.State == OutgoingState.Fail || message.State == OutgoingState.Throttled)
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);

            if (message.Message == null)
                return Request.CreateResponse(HttpStatusCode.OK, (string)null);

            return Request.CreateResponse(HttpStatusCode.OK, new OutgoingMessageDto(message.Message));
        }

        [Route("commit")]
        [HttpPost]
        public HttpResponseMessage Commit() // GET: v1/messages/commit
        {
            var deviceId = _authenticationContext.GetContextDevice(this.Request);

            var state = _messagingService.Commit(deviceId);

            if (state == OutgoingState.Fail || state == OutgoingState.Throttled)
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        private static readonly ILogger _logger = LoggerFactory.GetCurrentClassLogger();

        public ILogger Logger
        {
            get { return _logger; }
        }

        public string UserDefinedLogValue
        {
            get { return _authenticationContext.GetContextDevice(this.Request); }
        }
    }
}
