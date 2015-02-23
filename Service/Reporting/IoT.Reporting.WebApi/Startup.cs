using System.Net;
using System.Web.Http;
using IoT.Framework;
using IoT.Framework.Web;
using IoT.Framework.Web.ApiExceptions;
using IoT.Framework.Web.Logging;
using IoT.Reporting.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(Startup))] 

namespace IoT.Reporting.WebApi
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