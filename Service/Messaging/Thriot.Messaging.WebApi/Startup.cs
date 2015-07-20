using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using Thriot.Framework.Mvc.ApiExceptions;
using Thriot.Framework.Mvc.Logging;

namespace Thriot.Messaging.WebApi
{
    public class Startup
    {
        private readonly IApplicationEnvironment _appEnv;

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            _appEnv = appEnv;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder(_appEnv.ApplicationBasePath);
            configurationBuilder.AddJsonFile("services.json");
            configurationBuilder.AddJsonFile("connectionstring.json");

            var configuration = configurationBuilder.Build();

            services.AddMvc().Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new LogActionsAttribute());
                options.Filters.Add(new ApiExceptionFilterAttribute());
            });

            services.AddSingleton<Services.Caching.IMessageCache, Services.Caching.MessageCache>();
            services.AddSingleton<Services.Storage.IConnectionStringResolver, ConnectionStringResolver>();
            services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.DataAccess.ConnectionParametersResolver>();
            services.AddTransient<Services.MessagingService>();
            services.AddSingleton<IConfiguration>(_ => configuration);

            foreach (var extraService in Framework.ServicesResolver.Resolve(configuration, "Services"))
            {
                services.AddTransient(extraService.Key, extraService.Value);
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
