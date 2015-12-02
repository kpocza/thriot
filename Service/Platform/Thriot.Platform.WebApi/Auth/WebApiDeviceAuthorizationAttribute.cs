using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Thriot.Framework.Logging;
using Thriot.Framework.Mvc;
using Thriot.Objects.Model;

namespace Thriot.Platform.WebApi.Auth
{
    public class WebApiDeviceAuthorizationAttribute : AuthorizationFilterAttribute
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public override void OnAuthorization(AuthorizationContext context)
        {
            const string deviceIdHeader = "X-DeviceId";
            const string apiKeyHeader = "X-ApiKey";
            var fields = HeaderParser.ParseAllOrNothing(context.HttpContext.Request.Headers, deviceIdHeader, apiKeyHeader);

            if (fields != null)
            {
                var deviceAuthenticator = (IDeviceAuthenticator)context.HttpContext.RequestServices.GetService(typeof(IDeviceAuthenticator));

                var deviceId = fields[deviceIdHeader];
                var apiKey = fields[apiKeyHeader];

                if (deviceAuthenticator.Authenticate(new AuthenticationParameters(deviceId, apiKey)))
                {
                    new AuthenticationContext().RegisterContextDevice(context.HttpContext, deviceId);
                    return;
                }
            }

            context.Result = new HttpUnauthorizedResult();

            Logger.Warning("Unauthorized. IP: {0}. Request Url: {1}. DeviceId: {2}", 
                context.HttpContext.GetClientIpAddress(), 
                context.HttpContext.Request.Path,
                fields!= null ? fields[deviceIdHeader] : "N/A");
        }
    }
}