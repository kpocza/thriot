using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Thriot.Framework.Web.Logging;
using Thriot.Framework.Logging;
using Thriot.Framework.Web;
using Thriot.Objects.Model;

namespace Thriot.Platform.WebApi.Auth
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
            const string apiKeyHeader = "X-ApiKey";
            var fields = HeaderParser.ParseAllOrNothing(context.Request.Headers, deviceIdHeader, apiKeyHeader);

            if (fields != null)
            {
                var deviceAuthenticator =
                    (IDeviceAuthenticator)
                        context.ActionContext.ControllerContext.Configuration.DependencyResolver.GetService(
                            typeof(IDeviceAuthenticator));

                var deviceId = fields[deviceIdHeader];
                var apiKey = fields[apiKeyHeader];

                if (deviceAuthenticator.Authenticate(new AuthenticationParameters(deviceId, apiKey)))
                {
                    new AuthenticationContext().RegisterContextDevice(context.Request, deviceId);

                    return Task.FromResult(0);
                }
            }

            context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);

            Logger.Warning("Unauthorized. IP: {0}. Request Url: {1}. DeviceId: {2}", 
                context.Request.GetClientIpAddress(), 
                context.Request.RequestUri.AbsoluteUri,
                fields!= null ? fields[deviceIdHeader] : "N/A");

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}