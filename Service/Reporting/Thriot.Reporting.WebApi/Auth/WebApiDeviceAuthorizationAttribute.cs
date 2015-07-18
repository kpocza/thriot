using Microsoft.AspNet.Mvc;
using Thriot.Framework.Logging;
using Thriot.Framework.Mvc;
using Thriot.Objects.Model;

namespace Thriot.Reporting.WebApi.Auth
{
    public class WebApiDeviceAuthorizationAttribute : AuthorizationFilterAttribute
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public override void OnAuthorization(AuthorizationContext context)
        {
            const string deviceIdHeader = "X-DeviceId";
            const string devicekeyHeader = "X-DeviceKey";
            var fields = HeaderParser.ParseAllOrNothing(context.HttpContext.Request.Headers, deviceIdHeader, devicekeyHeader);

            if (fields != null)
            {
                var deviceAuthenticator = (IDeviceAuthenticator)context.HttpContext.RequestServices.GetService(typeof(IDeviceAuthenticator));

                var deviceId = fields[deviceIdHeader];
                var deviceKey = fields[devicekeyHeader];

                if (deviceAuthenticator.Authenticate(new AuthenticationParameters(deviceId, deviceKey)))
                {
                    new DeviceAuthenticationContext().RegisterContextDevice(context.HttpContext, deviceId);
                    return;
                }
            }

            context.Result = new HttpUnauthorizedResult();

            Logger.Warning("Unauthorized. IP: {0}. Request Url: {1}. DeviceId: {2}",
                context.HttpContext.GetClientIpAddress(),
                context.HttpContext.Request.Path,
                fields != null ? fields[deviceIdHeader] : "N/A");
        }
    }
}