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
    public class WebApiNetworkAuthenticatorAttribute : Attribute, IAuthenticationFilter
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public bool AllowMultiple
        {
            get { return false; }
        }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            const string networkIdHeader = "X-NetworkId";
            const string networkkeyHeader = "X-NetworkKey";
            var fields = HeaderParser.ParseAllOrNothing(context.Request.Headers, networkIdHeader, networkkeyHeader);

            if (fields != null)
            {
                var networkAuthenticator =
                    (INetworkAuthenticator)
                        context.ActionContext.ControllerContext.Configuration.DependencyResolver.GetService(
                            typeof(INetworkAuthenticator));

                var networkId = fields[networkIdHeader];
                var networkKey = fields[networkkeyHeader];

                if (networkAuthenticator.Authenticate(new AuthenticationParameters(networkId, networkKey)))
                {
                    new NetworkAuthenticationContext().RegisterContextNetwork(context.Request, networkId);

                    return Task.FromResult(0);
                }
            }

            context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);

            Logger.Warning("Unauthorized. IP: {0}. Request Url: {1}. NetworkId: {2}",
                context.Request.GetClientIpAddress(),
                context.Request.RequestUri.AbsoluteUri,
                fields != null ? fields[networkIdHeader] : "N/A");
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}