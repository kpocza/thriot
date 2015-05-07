using System.Net;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using Thriot.Framework;
using Thriot.Framework.Web;
using Thriot.Framework.Web.ApiExceptions;
using Thriot.Framework.Web.Logging;
using Thriot.Management.Model.Exceptions;

[assembly: OwinStartup(typeof(Thriot.Management.WebApi.Startup))] 

namespace Thriot.Management.WebApi
{
    public  class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
            
            var config = new HttpConfiguration();
            config.DependencyResolver = new UnityWebApiResolver(SingleContainer.Instance.Container);
            config.Filters.Add(new LogActionsAttribute());
            config.Filters.Add(new ApiExceptionFilterAttribute());
            config.MapHttpAttributeRoutes();

            app.UseCors(CorsOptions.AllowAll);

            app.UseWebApi(config);

            ApiExceptionRegistry.AddItem(typeof(ActivationRequiredException), HttpStatusCode.Forbidden);
            ApiExceptionRegistry.AddItem(typeof(ActivationException), HttpStatusCode.Forbidden);
            ApiExceptionRegistry.AddItem(typeof(ConfirmationException), HttpStatusCode.Forbidden);

            AutoMapperConfig.Register();
            MailTemplateConfig.Register();
            TelemetrySetupServiceConfig.Register();
            MessagingServiceConfig.Register();
        }
    }
}