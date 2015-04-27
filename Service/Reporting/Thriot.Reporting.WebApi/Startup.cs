using System.Net;
using System.Web.Http;
using Thriot.Reporting.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using Thriot.Framework;
using Thriot.Framework.Web;
using Thriot.Framework.Web.ApiExceptions;
using Thriot.Framework.Web.Logging;

[assembly: OwinStartup(typeof(Startup))] 

namespace Thriot.Reporting.WebApi
{
    public  class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServicePointManager.DefaultConnectionLimit = 10000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            var config = new HttpConfiguration();
            config.DependencyResolver = new UnityWebApiResolver(SingleContainer.Instance.Container);
            config.Filters.Add(new LogActionsAttribute());
            config.Filters.Add(new ApiExceptionFilterAttribute());
            config.MapHttpAttributeRoutes();

            app.UseCors(CorsOptions.AllowAll);
            
            app.UseWebApi(config);

            TelemetrySetupServiceConfig.Register();
        }
    }
}