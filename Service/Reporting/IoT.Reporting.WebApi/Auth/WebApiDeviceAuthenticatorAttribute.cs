using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using IoT.Framework.Logging;
using IoT.Framework.Web;
using IoT.Framework.Web.Logging;
using IoT.Objects.Model;

namespace IoT.Reporting.WebApi.Auth
{
    public class WebApiDeviceAuthenticatorAttribute : Attribute, IAuthenticationFilter
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public bool AllowMultiple
        {
            get { return false; }
        }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            const string deviceIdHeader = "X-DeviceId";
            const string devicekeyHeader = "X-DeviceKey";
            var fields = HeaderParser.ParseAllOrNothing(context.Request.Headers, deviceIdHeader, devicekeyHeader);

            if (fields != null)
            {
                var deviceAuthenticator =
                    (IDeviceAuthenticator)
                        context.ActionContext.ControllerContext.Configuration.DependencyResolver.GetService(
                            typeof(IDeviceAuthenticator));

                var deviceId = fields[deviceIdHeader];
                var deviceKey = fields[devicekeyHeader];

                if (deviceAuthenticator.Authenticate(new AuthenticationParameters(deviceId, deviceKey)))
                {
                    new DeviceAuthenticationContext().RegisterContextDevice(context.Request, deviceId);

                    return Task.FromResult(0);
                }
            }

            context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);

            Logger.Warning("Unauthorized. IP: {0}. Request Url: {1}. DeviceId: {2}",
                context.Request.GetClientIpAddress(),
                context.Request.RequestUri.AbsoluteUri,
                fields != null ? fields[deviceIdHeader] : "N/A");

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}