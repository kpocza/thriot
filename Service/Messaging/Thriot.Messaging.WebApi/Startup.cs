using System.Web.Http;
using Thriot.Messaging.WebApi;
using Microsoft.Owin;
using Owin;
using Thriot.Framework;
using Thriot.Framework.Web;
using Thriot.Framework.Web.ApiExceptions;
using Thriot.Framework.Web.Logging;

[assembly: OwinStartup(typeof(Startup))] 

namespace Thriot.Messaging.WebApi
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