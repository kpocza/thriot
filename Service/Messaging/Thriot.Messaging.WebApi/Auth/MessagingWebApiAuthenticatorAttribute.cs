//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Thriot.Framework.Logging;

//namespace Thriot.Messaging.WebApi.Auth
//{
//    public class MessagingWebApiAuthenticatorAttribute : Attribute, IAuthenticationFilter
//    {
//        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

//        public bool AllowMultiple
//        {
//            get { return false; }
//        }

//        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
//        {
//            IEnumerable<string> values;

//            var headers = context.Request.Headers;

//            if (headers.TryGetValues("X-MessagingServiceApiKey", out values) && values.Count() == 1)
//            {
//                var apiKey = values.Single();

//                EnsureReferenceApiKey(context);

//                if (apiKey == _referenceApiKey)
//                {
//                    return Task.FromResult(0);
//                }
//            }

//            context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);

//            Logger.Error("Unauthorized. IP: {0}. Request Url: {1}", context.Request.GetClientIpAddress(), context.Request.RequestUri.AbsoluteUri);
            
//            return Task.FromResult(0);
//        }

//        private static void EnsureReferenceApiKey(HttpAuthenticationContext context)
//        {
//            if (_referenceApiKey == null)
//            {
//                var settingOperations =
//                    (ISettingOperations)
//                        context.ActionContext.ControllerContext.Configuration.DependencyResolver.GetService(
//                            typeof (ISettingOperations));

//                _referenceApiKey = settingOperations.Get(Setting.MessagingServiceApiKey).Value;
//            }
//        }

//        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
//        {
//            return Task.FromResult(0);
//        }

//        private static string _referenceApiKey;
//    }
//}