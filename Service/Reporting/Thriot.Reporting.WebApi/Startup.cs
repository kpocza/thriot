using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Microsoft.Extensions.PlatformAbstractions;
using Thriot.Framework;
using Thriot.Framework.Mvc.ApiExceptions;
using Thriot.Framework.Mvc.Logging;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Reporting.Services;
using Thriot.Reporting.WebApi.Auth;
using Thriot.Platform.Services.Client;

namespace Thriot.Reporting.WebApi
{
    public class Startup
    {
        private readonly IApplicationEnvironment _appEnv;

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            _appEnv = appEnv;

            AssemblyResolver.Initialize();

            Framework.Logging.NLogLogger.SetConfiguration(
                System.IO.Path.Combine(System.IO.Path.Combine(appEnv.ApplicationBasePath, "config"), "web.nlog"));

            ServicePointManager.DefaultConnectionLimit = 10000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(_appEnv.ApplicationBasePath);
            configurationBuilder.AddJsonFile("config/services.json");
            configurationBuilder.AddJsonFile("config/connectionstring.json");

            var configuration = configurationBuilder.Build();

            services.AddMvc();
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new LogActionsAttribute());
                options.Filters.Add(new ApiExceptionFilterAttribute());
            });
            services.AddCors(
                c =>
                    c.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

            ConfigureThriotServices(services, configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var serviceProvider = app.ApplicationServices;

            var telemetryDataSinkSetupServiceClient = serviceProvider.GetService<ITelemetryDataSinkSetupServiceClient>();
            var settingOperations = serviceProvider.GetService<ISettingOperations>();

            telemetryDataSinkSetupServiceClient.Setup(settingOperations.Get(Setting.TelemetrySetupServiceEndpoint).Value, settingOperations.Get(Setting.TelemetrySetupServiceApiKey).Value);

            app.UseIISPlatformHandler();

            app.UseCors("AllowAll");

            app.UseMvc();
        }

        private void ConfigureThriotServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<DeviceReportingService>();
            services.AddTransient<NetworkReportingService>();
            services.AddTransient<DeviceAuthenticationContext>();
            services.AddTransient<NetworkAuthenticationContext>();
            services.AddSingleton<ITelemetryDataSinkSetupServiceClient, TelemetryDataSinkSetupServiceClient>();
            services.AddSingleton<Framework.DataAccess.IConnectionParametersResolver, Framework.DataAccess.ConnectionParametersResolver>();
            services.AddSingleton<Framework.DataAccess.IDynamicConnectionStringResolver, DynamicConnectionStringResolver>();
            services.AddTransient<ITelemetryDataSinkProcessor, TelemetryDataSinkProcessor>();
            services.AddTransient<IDeviceAuthenticator, Objects.Common.DeviceAuthenticator>();
            services.AddTransient<INetworkAuthenticator, Objects.Common.NetworkAuthenticator>();
            services.AddSingleton(_ => configuration);

            foreach (var extraService in configuration.AsTypeMap("Services"))
            {
                services.AddTransient(extraService.Key, extraService.Value);
            }
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
