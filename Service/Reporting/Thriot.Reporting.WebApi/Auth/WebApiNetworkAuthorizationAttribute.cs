using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Thriot.Framework.Logging;
using Thriot.Framework.Mvc;
using Thriot.Objects.Model;

namespace Thriot.Reporting.WebApi.Auth
{
    public class WebApiNetworkAuthorizationAttribute : AuthorizationFilterAttribute
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public override void OnAuthorization(AuthorizationContext context)
        {
            const string networkIdHeader = "X-NetworkId";
            const string networkkeyHeader = "X-NetworkKey";
            var fields = HeaderParser.ParseAllOrNothing(context.HttpContext.Request.Headers, networkIdHeader, networkkeyHeader);

            if (fields != null)
            {
                var networkAuthenticator = (INetworkAuthenticator)context.HttpContext.RequestServices.GetService(typeof(INetworkAuthenticator));

                var networkId = fields[networkIdHeader];
                var networkKey = fields[networkkeyHeader];

                if (networkAuthenticator.Authenticate(new AuthenticationParameters(networkId, networkKey)))
                {
                    new NetworkAuthenticationContext().RegisterContextNetwork(context.HttpContext, networkId);
                    return;
                }
            }

            context.Result = new HttpUnauthorizedResult();

            Logger.Warning("Unauthorized. IP: {0}. Request Url: {1}. NetworkId: {2}",
                context.HttpContext.GetClientIpAddress(),
                context.HttpContext.Request.Path,
                fields != null ? fields[networkIdHeader] : "N/A");
        }
    }
}