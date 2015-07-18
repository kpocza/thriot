using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Thriot.Framework.Mvc;
using Thriot.Framework.Logging;
using Thriot.Objects.Model.Operations;
using Thriot.Objects.Model;

namespace Thriot.Messaging.WebApi.Auth
{
    public class MessagingWebApiAuthorizationAttribute : AuthorizationFilterAttribute
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public override void OnAuthorization(AuthorizationContext context)
        {
            string[] values;

            var headers = context.HttpContext.Request.Headers;

            if (headers.TryGetValue("X-MessagingServiceApiKey", out values) && values.Count() == 1)
            {
                var apiKey = values.Single();

                EnsureReferenceApiKey(context.HttpContext.RequestServices);

                if (apiKey == _referenceApiKey)
                {
                    return;
                }
            }

            context.Result = new HttpUnauthorizedResult();

            Logger.Error("Unauthorized. IP: {0}. Request Url: {1}", context.HttpContext.GetClientIpAddress(), context.HttpContext.Request.Path);
        }

        private static void EnsureReferenceApiKey(IServiceProvider serviceProvider)
        {
            if (_referenceApiKey == null)
            {
                var settingOperations = (ISettingOperations)serviceProvider.GetService(typeof(ISettingOperations));

                _referenceApiKey = settingOperations.Get(Setting.MessagingServiceApiKey).Value;
            }
        }

        private static string _referenceApiKey;
    }
}