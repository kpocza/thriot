using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Dnx.Runtime;
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

            Framework.Logging.NLogLogger.SetConfiguration(
                System.IO.Path.Combine(System.IO.Path.Combine(appEnv.ApplicationBasePath, "config"), "web.nlog"));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder(_appEnv.ApplicationBasePath);
            configurationBuilder.AddJsonFile("config/services.json");
            configurationBuilder.AddJsonFile("config/servicesmsg.json");
            configurationBuilder.AddJsonFile("config/connectionstring.json");
            configurationBuilder.AddJsonFile("config/connectionstringmsg.json");

            var configuration = configurationBuilder.Build();

            services.AddMvc();
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new LogActionsAttribute());
                options.Filters.Add(new ApiExceptionFilterAttribute());
            });

            ConfigureThriotServices(services, configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }

        private void ConfigureThriotServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<Services.Caching.IMessageCache, Services.Caching.MessageCache>();
            services.AddSingleton<Services.Storage.IConnectionStringResolver, ConnectionStringResolver>();
            services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.DataAccess.ConnectionParametersResolver>();
            services.AddTransient<Services.MessagingService>();
            services.AddSingleton(_ => configuration);

            foreach (var extraService in Framework.ConfigurationAdapter.LoadServiceConfiguration(configuration, "Services"))
            {
                services.AddTransient(extraService.Key, extraService.Value);
            }
        }
    }
}
