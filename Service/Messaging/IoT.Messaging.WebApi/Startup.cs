using System.Web.Http;
using IoT.Framework;
using IoT.Framework.Web;
using IoT.Framework.Web.ApiExceptions;
using IoT.Framework.Web.Logging;
using IoT.Messaging.WebApi;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))] 

namespace IoT.Messaging.WebApi
{
    public  class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.DependencyResolver = new UnityWebApiResolver(SingleContainer.Instance.Container);
            config.Filters.Add(new LogActionsAttribute());
            config.Filters.Add(new ApiExceptionFilterAttribute());
            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);
        }
    }
}