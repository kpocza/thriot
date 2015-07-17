using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Thriot.Framework.Mvc.ApiExceptions;
using Thriot.Framework.Mvc.Logging;

namespace Thriot.Messaging.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("services.json");
            configurationBuilder.AddJsonFile("connectionstring.json");

            services.AddMvc().Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new LogActionsAttribute());
                options.Filters.Add(new ApiExceptionFilterAttribute());
            });

            services.AddSingleton<Services.Caching.IMessageCache, Services.Caching.MessageCache>();
            services.AddSingleton<Services.Storage.IConnectionStringResolver, ConnectionStringResolver>();
            services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.DataAccess.LocalConnectionParametersResolver>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
