using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Thriot.Framework.Web.Logging;
using Thriot.Framework.Logging;

namespace Thriot.Management.WebApi.Auth
{
    public class WebApiAuthenticatorAttribute : Attribute, IAuthenticationFilter
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public bool AllowMultiple
        {
            get { return false; }
        }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var authHeader = context.Request.Headers.Authorization;

            if (authHeader != null && authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase))
            {
                var authTokenHandler =
                    (AuthTokenHandler)
                        context.ActionContext.ControllerContext.Configuration.DependencyResolver.GetService(
                            typeof(AuthTokenHandler));

                var authToken = authTokenHandler.ExtractToken(authHeader.Parameter);

                if (authToken != null)
                {
                    var principal = authTokenHandler.GenerateContextUser(authToken);

                    if (principal != null)
                    {
                        context.Principal = principal;
                        return Task.FromResult(0);
                    }
                }
            }

            context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);

            Logger.Error("Unauthorized. IP: {0}. Request Url: {1}", context.Request.GetClientIpAddress(), context.Request.RequestUri.AbsoluteUri);

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result);
            return Task.FromResult(0);
        }

        public class ResultWithChallenge : IHttpActionResult
        {
            private readonly IHttpActionResult _next;

            public ResultWithChallenge(IHttpActionResult next)
            {
                _next = next;
            }

            public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var res = await _next.ExecuteAsync(cancellationToken);

                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    res.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic"));
                }

                return res;
            }
        }
    }
}